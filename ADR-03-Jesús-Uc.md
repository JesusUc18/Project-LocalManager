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

---

### ¿Por qué este estilo resuelve mejor el problema?
 
- **Aislamiento de la lógica de negocio crítica:** las reglas financieras (registrar venta → descontar stock → afectar caja) viven en `Application`, sin mezclarse con código de presentación (Razor/Controllers) ni con detalles de EF Core. Esto reduce el riesgo señalado en el ADR-02 de que un cambio en la pantalla de ventas rompa el cálculo del corte de caja.
- **Regla de dependencia forzada por el compilador:** `Domain` no conoce a `Infrastructure` ni a `Presentation`. Si alguien intenta usar `DbContext` directamente desde `Application`, el proyecto no compila. Esto impide accidentalmente "ensuciar" la lógica de negocio con detalles de SQL Server o ASP.NET.
- **Inversión de dependencias:** `Infrastructure` implementa las interfaces (`IProductoRepository`, etc.) definidas en `Domain`. Esto permite que la lógica de negocio sea independiente del motor de base de datos concreto.
- **Reduce la deuda técnica del ADR-02:** si en el futuro se requiere una Web API para una app móvil, solo se agrega un nuevo proyecto `LocalManager.Api` que reutiliza `Application` y `Domain` sin modificarlos. El MVC actual (`Presentation`) puede coexistir o ser reemplazado sin tocar la lógica de negocio.
- **Testeable:** `Application` puede probarse con pruebas unitarias usando repositorios falsos (mocks de las interfaces de `Domain`), sin necesidad de una base de datos real.

---
 
## Alternativas consideradas
 
| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **Monolito MVC por carpetas (propuesta original del ADR-02)** | No fuerza la separación de responsabilidades: nada impide que un Controller llame directamente a EF Core o que la lógica de negocio termine dispersa entre Controllers y Models. La disciplina depende solo de la voluntad del desarrollador, no del compilador. |
| **Cliente-servidor (React + Web API)** | Ya descartada en ADR-02: duplica el trabajo (dos proyectos, manejo de estado en navegador, CORS, autenticación por tokens) sin aportar valor en esta etapa, dado el plazo de 3 meses y que es un desarrollador único. |
| **Arquitectura de microservicios** | Requiere múltiples servicios desplegados de forma independiente, comunicación entre ellos (HTTP/mensajería) y orquestación. Para un sistema de punto de venta de un solo negocio, esto es sobreingeniería: añade latencia de red y complejidad operativa sin beneficio real a esta escala. |
| **Arquitectura hexagonal completa (Ports & Adapters con múltiples adaptadores)** | Comparte la idea de aislar el dominio, pero típicamente implica más capas de abstracción (puertos de entrada y salida explícitos, múltiples adaptadores intercambiables) de las que el proyecto necesita en 3 meses. La arquitectura en capas con inversión de dependencias logra el mismo aislamiento del dominio con menos artefactos. |
| **Event-driven (basado en eventos/mensajería)** | El flujo del negocio (venta → descuento de stock → registro en caja) requiere consistencia inmediata y transaccional, no consistencia eventual. Introducir un bus de eventos agregaría infraestructura y complejidad de depuración no justificada. |
| **Serverless (funciones)** | No es adecuado para un sistema con estado transaccional fuerte (control de caja por turno, inventario); requeriría rediseñar el acceso a datos para operar sin estado, y la infraestructura de funciones no aporta ventaja para un sistema de uso interno de un negocio local. |
