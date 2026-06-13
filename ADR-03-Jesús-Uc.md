# ADR-03: Estilo Arquitectónico del Sistema (Arquitectura en Capas / Clean Architecture)

| Campo  | Valor |
|--------|-------|
| Autor  | Jesús Uc |
| Fecha  | 12/06/2026 |
| Estado | `APROBADO` |

---

## Contexto

En el ADR-02 se definió que **Local Manager** se construirá con **ASP.NET Core MVC + Entity Framework Core + SQL Server**, siguiendo el patrón MVC dentro de un único proyecto, organizado por carpetas (Controllers, Models, Views, Data).
 
Tras avanzar en el diseño, pues detecté la necesidad de formalizar el **estilo arquitectónico** del sistema, más allá del patrón de presentación (MVC). El objetivo es definir cómo se organiza el código a nivel de **separación de responsabilidades, dependencias entre componentes y aislamiento de la lógica de negocio**, de forma que el sistema sea mantenible, testeable y preparado para evolucionar (por ejemplo, hacia una Web API para una futura app móvil) sin perder el control de las actualizaciones.
 
Restricciones que se mantienen del ADR-02:
 
- **Complejidad de los datos:** entidades fuertemente relacionadas (Venta, Producto, Categoría, Inventario, Caja, Cliente) que requieren consistencia transaccional.
- **Tecnología:** C# / .NET, ASP.NET Core, EF Core, SQL Server (sin cambios).
- **Riesgo identificado en ADR-02:** acoplar la lógica de negocio directamente a los Controllers o al acceso a datos dificultaría el mantenimiento y una futura migración a Web API.

---

## Decisión
 
Se adopta el estilo arquitectónico de **Arquitectura en Capas (Layered Architecture)**, implementado mediante **separación física en proyectos** dentro de la misma solución (.sln), siguiendo los principios de **Clean Architecture** (regla de dependencia hacia el centro).
 
La solución `LocalManager.sln` se organiza en cuatro proyectos:
 
- **LocalManager.Domain** — Entidades del negocio (Producto, Venta, DetalleVenta, Cliente, Categoría, Caja) e interfaces de repositorio (`IVentaRepository`, `IProductoRepository`, etc.). No depende de ningún otro proyecto.
- **LocalManager.Application** — Servicios de negocio (`VentaService`, `InventarioService`, `CajaService`, `ClienteService`) que contienen las reglas (ej. registrar venta y descontar stock como operación atómica). Depende únicamente de `Domain`.
- **LocalManager.Infrastructure** — Implementación de los repositorios definidos en `Domain`, `DbContext` de EF Core, configuración de SQL Server. Depende de `Domain`.
- **LocalManager.Presentation** — Proyecto ASP.NET Core MVC: Controllers, Views (Razor), wwwroot. Depende de `Application` e `Infrastructure` (esta última solo para la configuración de inyección de dependencias en `Program.cs`).
