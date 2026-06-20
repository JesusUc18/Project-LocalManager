using LocalManager.Domain.Entities;

namespace LocalManager.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato de repositorio para Ventas.
    /// CAPA: Domain — define el contrato, Infrastructure lo implementa.
    /// </summary>
    public interface IVentaRepository
    {
        List<Venta> ObtenerTodas();
        Venta? ObtenerPorId(int id);
        void Agregar(Venta venta);
        void AgregarDetalle(DetalleVenta detalle);
        List<Venta> ObtenerPorCaja(int cajaId);
        List<Venta> ObtenerPorFecha(DateTime fecha);
    }
}
