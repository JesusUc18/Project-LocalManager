using LocalManager.Domain.Entities;

namespace LocalManager.Application.Services
{
    /// <summary>
    /// VISTA DE LÓGICA (ADR-02): Módulo de Inventario → Gestión de categorías
    /// VISTA DE PROCESOS (ADR-02): Application → Domain → Repositorio → Infrastructure
    /// CAPA: Application — contiene las reglas de negocio, depende solo de Domain.
    /// </summary>
    public interface ICategoriaService
    {
        List<Categoria> ObtenerTodas();
        Categoria? ObtenerPorId(int id);
        void Crear(Categoria categoria);
        void Actualizar(Categoria categoria);
        void Eliminar(int id);
    }
}
