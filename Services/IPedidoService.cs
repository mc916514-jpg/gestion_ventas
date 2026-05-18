using System;
using System.Collections.Generic;
using GestionComercial.Models;

namespace GestionComercial.Services
{
    public interface IPedidoService
    {
        IEnumerable<Pedido> ObtenerTodos();
        Pedido? ObtenerPorId(int id);
        IEnumerable<Pedido> ObtenerPorUsuarioId(int usuarioId);
        void ProcesarCompra(Pedido pedido);
        decimal ObtenerVentasTotales();
        int ObtenerTotalPedidos();
        IEnumerable<Pedido> ObtenerFiltrados(DateTime? start, DateTime? end, int? categoryId);
    }
}
