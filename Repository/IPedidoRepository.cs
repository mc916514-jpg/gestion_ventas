using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using GestionComercial.Models;

namespace GestionComercial.Repository
{
    public interface IPedidoRepository
    {
        IEnumerable<Pedido> GetAll();
        Pedido? GetById(int id);
        int Insert(Pedido pedido, SqlConnection conn, SqlTransaction trans);
        void InsertDetalle(DetallePedido detalle, SqlConnection conn, SqlTransaction trans);
        IEnumerable<Pedido> GetByUsuarioId(int usuarioId);
        decimal GetTotalSales();
        int GetCount();
        IEnumerable<Pedido> GetFiltered(DateTime? start, DateTime? end, int? categoryId);
    }
}
