using LocalManager.Domain.Entities;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// VISTA DE LÓGICA (ADR-02): Módulo de Ventas → Registro de ventas
    /// VISTA DE PROCESOS (ADR-02): Application → Domain → Repositorio → Infrastructure
    /// CAPA: Application — contiene las reglas de negocio, depende solo de Domain.
    /// 
    /// Transacciones atómicas (ADR-01): 
    /// Si el stock de algún producto es insuficiente, la venta completa se cancela.
    /// </summary>
    public interface IVentaService
    {
        List<Venta> ObtenerTodas();
        Venta? ObtenerPorId(int id);
        (bool exito, string mensaje) Registrar(Venta venta);
        List<Venta> ObtenerPorCaja(int cajaId);
        List<Venta> ObtenerPorFecha(DateTime fecha);
    }
}
