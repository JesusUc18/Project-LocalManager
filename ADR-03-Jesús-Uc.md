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
