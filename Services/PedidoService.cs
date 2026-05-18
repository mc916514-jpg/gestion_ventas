using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using GestionComercial.Data;
using GestionComercial.Models;
using GestionComercial.Repository;

namespace GestionComercial.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepo;
        private readonly IHistorialAccionRepository _audit;
        private readonly DbConnectionFactory _db;

        public PedidoService(IPedidoRepository pedidoRepo, IHistorialAccionRepository audit, DbConnectionFactory db)
        {
            _pedidoRepo = pedidoRepo;
            _audit = audit;
            _db = db;
        }

        public IEnumerable<Pedido> ObtenerTodos()
        {
            return _pedidoRepo.GetAll();
        }

        public Pedido? ObtenerPorId(int id)
        {
            return _pedidoRepo.GetById(id);
        }

        public IEnumerable<Pedido> ObtenerPorUsuarioId(int usuarioId)
        {
            return _pedidoRepo.GetByUsuarioId(usuarioId);
        }

        public void ProcesarCompra(Pedido pedido)
        {
            if (pedido == null || pedido.Detalles.Count == 0)
            {
                throw new ArgumentException("El pedido no contiene detalles de productos.");
            }

            using (var conn = _db.GetConnection())
            {
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Validar stock de cada producto en la transacción
                        foreach (var det in pedido.Detalles)
                        {
                            var stockQuery = "SELECT Stock, Nombre FROM Productos WHERE Id = @Id";
                            int currentStock = 0;
                            string productName = string.Empty;

                            using (var cmdStock = new SqlCommand(stockQuery, conn, trans))
                            {
                                cmdStock.Parameters.Add("@Id", SqlDbType.Int).Value = det.ProductoId;
                                using (var reader = cmdStock.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        currentStock = reader.GetInt32(0);
                                        productName = reader.GetString(1);
                                    }
                                    else
                                    {
                                        throw new Exception($"El producto con ID {det.ProductoId} no existe en el catálogo.");
                                    }
                                }
                            }

                            if (currentStock < det.Cantidad)
                            {
                                throw new InvalidOperationException($"Stock insuficiente para '{productName}'. Stock disponible: {currentStock}, Solicitado: {det.Cantidad}");
                            }

                            // Almacenar el stock actual para restarle luego
                            det.ProductoNombre = productName; // Asignar para el log
                        }

                        // 2. Insertar cabecera del Pedido
                        int newPedidoId = _pedidoRepo.Insert(pedido, conn, trans);
                        pedido.Id = newPedidoId;

                        // 3. Insertar detalles del Pedido y actualizar el stock
                        foreach (var det in pedido.Detalles)
                        {
                            det.PedidoId = newPedidoId;
                            _pedidoRepo.InsertDetalle(det, conn, trans);

                            // Restar stock atómicamente
                            var updateStockQuery = "UPDATE Productos SET Stock = Stock - @Cantidad WHERE Id = @ProductoId";
                            using (var cmdUpdateStock = new SqlCommand(updateStockQuery, conn, trans))
                            {
                                cmdUpdateStock.Parameters.Add("@Cantidad", SqlDbType.Int).Value = det.Cantidad;
                                cmdUpdateStock.Parameters.Add("@ProductoId", SqlDbType.Int).Value = det.ProductoId;
                                cmdUpdateStock.ExecuteNonQuery();
                            }
                        }

                        // 4. Confirmar transacción
                        trans.Commit();

                        // 5. Registrar en el historial de acciones (fuera de la transacción principal)
                        _audit.Insert(new HistorialAccion
                        {
                            Accion = "COMPRA_PROCESADA",
                            Detalle = $"Pedido ID {newPedidoId} registrado con éxito. Total: {pedido.Total:C} para el Usuario ID {pedido.UsuarioId}.",
                            Fecha = DateTime.Now
                        });
                    }
                    catch (Exception ex)
                    {
                        // Rollback automático en caso de cualquier error
                        try
                        {
                            trans.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            // Registrar error de rollback o continuar
                            Console.WriteLine($"Error al hacer rollback: {rollbackEx.Message}");
                        }
                        
                        // Registrar el error en el historial
                        _audit.Insert(new HistorialAccion
                        {
                            Accion = "COMPRA_FALLIDA",
                            Detalle = $"Fallo al procesar pedido: {ex.Message}",
                            Fecha = DateTime.Now
                        });

                        throw; // Volver a lanzar la excepción original para que el controlador la exponga
                    }
                }
            }
        }

        public decimal ObtenerVentasTotales()
        {
            return _pedidoRepo.GetTotalSales();
        }

        public int ObtenerTotalPedidos()
        {
            return _pedidoRepo.GetCount();
        }

        public IEnumerable<Pedido> ObtenerFiltrados(DateTime? start, DateTime? end, int? categoryId)
        {
            return _pedidoRepo.GetFiltered(start, end, categoryId);
        }
    }
}
