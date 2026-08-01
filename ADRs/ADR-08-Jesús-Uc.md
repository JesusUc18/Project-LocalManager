# ADR-08: Migración de persistencia JSON a PostgreSQL (pago de la Deuda técnica 1 del ADR-06)

| Campo  | Valor |
|--------|-------|
| Autor  | Jesús Uc |
| Fecha  | 31/07/2026 |
| Estado | `APROBADO` |

---

## Contexto

El ADR-05 definió el patrón **Strategy** para la persistencia (`IDbContext` con `JsonDbContext` y una futura `SqlDbContext`), y el ADR-06 documentó como **Deuda técnica 1** que esa segunda estrategia nunca se implementó: la bandera `UseJsonPersistence` no seleccionaba nada distinto, y `LocalManager.Api/appsettings.json` tenía una ruta absoluta de la máquina del autor.

Este ADR documenta el pago de esa deuda: la implementación real de la estrategia SQL usando **Entity Framework Core 8 sobre PostgreSQL** (en vez de SQL Server, previsto originalmente en el ADR-01), y su verificación en un entorno local con pgAdmin.

## Decisión

Se eligió **PostgreSQL** sobre SQL Server por tres razones prácticas para este proyecto individual:

- **Costo en la nube:** casi todos los proveedores con capa gratuita para estudiantes (Render, Railway, Supabase, Neon) ofrecen PostgreSQL gratis; una instancia de SQL Server gratuita en la nube es mucho más limitada o inexistente fuera de Azure.
- **Multiplataforma:** PostgreSQL corre igual en Windows, Linux (contenedor de despliegue) y macOS, sin depender del ecosistema Microsoft para la base de datos.
- **Compatibilidad con EF Core sin cambiar el resto del sistema:** al cambiar solo el *proveedor* de Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL` en vez de `Microsoft.EntityFrameworkCore.SqlServer`), el `AppDbContext` ya definido en el ADR-05 no necesitó cambios, y ningún repositorio, servicio ni controlador se modificó — confirmando en la práctica que el patrón Strategy definido en el ADR-05 cumple su propósito.

## Cambios realizados

1. **`LocalManager.Infrastructure`**: se reemplazó el paquete `Microsoft.EntityFrameworkCore.SqlServer` por `Npgsql.EntityFrameworkCore.PostgreSQL`.
2. **`SqlDbContext.cs` (nuevo):** implementa `IDbContext` envolviendo `AppDbContext`, cerrando la segunda estrategia que el ADR-05 dejó prevista y el ADR-06 marcó como pendiente.
3. **`Program.cs`** (Presentation y Api): la rama `else` del selector de estrategia ahora registra `AppDbContext` con `UseNpgsql(...)` y `SqlDbContext` como `IDbContext`, en vez de caer siempre en `JsonDbContext` como antes.
4. **`appsettings.json`** (ambos proyectos): se agregó `ConnectionStrings:DefaultConnection` y se cambió `UseJsonPersistence` a `false` por defecto, resolviendo también la ruta absoluta hardcodeada que señalaba la Deuda técnica 1.
5. **Migraciones EF Core:** se generó la migración `InicialPostgres` (`dotnet ef migrations add` / `dotnet ef database update`), que crea las 6 tablas del dominio con sus llaves foráneas e índices directamente desde las entidades de `Domain`, sin SQL escrito a mano.

## Consecuencias

**✅ Lo que se gana:**

- Se cierra formalmente la Deuda técnica 1 del ADR-06: la bandera `UseJsonPersistence` ahora sí selecciona una estrategia distinta, y ya no hay rutas absolutas de una máquina específica en el repositorio.
- El sistema pasa de archivos JSON en disco (sin integridad referencial real) a una base de datos relacional con llaves foráneas e índices, lo que previene datos huérfanos (por ejemplo, una venta apuntando a un producto inexistente).
- PostgreSQL es el motor que se usará también en el despliegue en la nube, por lo que el entorno local (pgAdmin) y el de producción quedan alineados.

**⚠️ Lo que queda pendiente:**

- La Deuda técnica 2 del ADR-06 (transacción simulada en `VentaService.Registrar`) **sigue sin resolverse**: aunque ahora hay un motor de base de datos capaz de soportar transacciones reales (`Database.BeginTransaction()`), el código de `Application` todavía no las usa. Se mantiene como trabajo futuro, ahora técnicamente más simple de resolver gracias a este ADR.
- No se migraron datos históricos desde los archivos JSON; la base de datos se pobló con un script de datos de prueba nuevo, no con una migración de datos real.

---

## 🤖 Uso de Inteligencia Artificial

Este documento fue redactado de forma personal. Se utilizó inteligencia artificial como herramienta de apoyo en los siguientes aspectos específicos:

| Área de uso | Descripción |
|-------------|-------------|
| **Implementación de `SqlDbContext`** | Se usó IA para escribir la clase `SqlDbContext` que envuelve `AppDbContext`, y para ajustar el registro de servicios en `Program.cs` de Presentation y Api. |
| **Resolución de conflictos de versión NuGet** | Se usó IA para diagnosticar el error `NU1605` (degradación de paquete) al alinear las versiones de `Microsoft.EntityFrameworkCore` y `Npgsql.EntityFrameworkCore.PostgreSQL`. |
| **Script de datos de prueba** | Se usó IA para generar el script SQL de datos de prueba (categorías, productos, clientes, cajas, ventas) usado en pgAdmin. |
| **Redacción y estructura del documento** | Se empleó IA para organizar este ADR en el mismo formato usado en los ADRs anteriores. |

> **Nota:** La decisión de usar PostgreSQL sobre SQL Server y la verificación de que el pago de la deuda técnica es correcto (probando la app contra la base de datos real) son de autoría propia.