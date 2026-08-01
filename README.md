# Local Manager — Sistema de Gestión para Negocios Locales

> **Estado:** `APROBADO` | **Autor:** Jesús Uc | **Última actualización:** 01/08/2026

Sistema de gestión de negocios locales con **Clean Architecture + Patrones GOF + API REST**. Permite controlar ventas, inventario, clientes y caja desde cualquier navegador, con una API documentada con Swagger/OpenAPI para consumo por múltiples clientes. Cuenta además con una suite de pruebas automatizadas con **xUnit**, un pipeline de **Integración Continua** en GitHub Actions, y persiste sus datos en **PostgreSQL**.

---

## Arquitectura (5 Proyectos + Pruebas)

```
LocalManager/
├── LocalManager.sln
├── LocalManager.Domain/              ← CENTRO (no depende de nadie)
│   ├── Entities/                     → Categoria, Producto, Cliente, Venta, Caja
│   └── Interfaces/Repositories/      → ICategoriaRepository, IProductoRepository, etc.
│
├── LocalManager.Application/         ← Reglas de negocio (solo Domain)
│   └── Services/                     → CategoriaService, ProductoService, VentaService, etc.
│
├── LocalManager.Infrastructure/      ← Persistencia (solo Domain)
│   ├── Data/
│   │   ├── IDbContext.cs             → Interfaz Strategy (patrón GOF)
│   │   ├── JsonDbContext.cs          → Estrategia JSON (respaldo)
│   │   ├── AppDbContext.cs           → DbContext de EF Core (Npgsql / PostgreSQL)
│   │   └── SqlDbContext.cs           → Estrategia SQL (activa) — envuelve AppDbContext
│   └── Repositories/                 → Implementaciones Repository (patrón GOF)
│
├── LocalManager.Presentation/        ← ASP.NET Core MVC
│   ├── Controllers/
│   ├── Views/
│   └── Data/                         → Archivos JSON (solo si UseJsonPersistence = true)
│
├── LocalManager.Api/                 ← ASP.NET Core Web API + Swagger
│   ├── Controllers/                  → ProductosApi, VentasApi, CajaApi, ReportesApi
│   └── Models/                       → DTOs: ApiResponse, CrearVentaRequest
│
└── LocalManager.xUnit/               ← Pruebas automatizadas (xUnit)
    ├── CategoriasControllerTests.cs  → CategoriasController + fake de ICategoriaService
    ├── ClientesControllerTests.cs    → ClientesController + fake de IClienteService
    └── CajaControllerTests.cs        → CajaController + fake de ICajaService
```

### Regla de Dependencia

```
Api → Application → Domain
Presentation → Application → Domain
      ↓            ↓
Infrastructure ←──┘

LocalManager.xUnit → Presentation → Application → Domain
```

`Domain` no conoce `Infrastructure`, `Presentation`, `Api` ni `LocalManager.xUnit`. El compilador lo garantiza.

---

## Patrones de Diseño GOF

### Repository (Estructural)
Cada entidad tiene su interfaz de repositorio definida en `Domain` e implementada en `Infrastructure`. Los servicios de negocio nunca acceden directamente a los datos — solo conocen la interfaz.

```
Domain:         IProductoRepository, IVentaRepository, ICajaRepository...
Infrastructure: ProductoRepository, VentaRepository, CajaRepository...
Application:    VentaService(IVentaRepository, IProductoRepository) ← solo interfaces
```

### Strategy (Comportamiento)
El mecanismo de persistencia es intercambiable sin modificar repositorios, servicios ni controladores. Se controla con una sola línea en `appsettings.json`:

```json
"UseJsonPersistence": true    ← JSON (respaldo, sin base de datos)
"UseJsonPersistence": false   ← PostgreSQL vía EF Core / Npgsql (activo)
```

```
IDbContext ←── JsonDbContext   (estrategia JSON)
IDbContext ←── SqlDbContext    (estrategia PostgreSQL, usa AppDbContext internamente)
```

Los repositorios reciben `IDbContext`, nunca la implementación concreta.

> ✅ **Deuda técnica pagada (ADR-08):** la Deuda técnica 1 del ADR-06 quedó resuelta — `SqlDbContext` ya está implementado, la bandera `UseJsonPersistence` selecciona una estrategia real, y las rutas absolutas de máquina local en `appsettings.json` se reemplazaron por `ConnectionStrings:DefaultConnection`.

Este mismo desacoplamiento (controladores de `Presentation` dependiendo solo de interfaces de `Application`) es lo que permite probar los controladores en `LocalManager.xUnit` con fakes en memoria, sin necesitar `Infrastructure` ni una base de datos real. Ver **ADR-07**.

---

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8 |
| Patrón arquitectónico | MVC + Clean Architecture |
| Patrones de diseño | Repository (GOF Estructural) + Strategy (GOF Comportamiento) |
| Base de datos | **PostgreSQL** (activo) / JSON (respaldo) |
| ORM | Entity Framework Core 8 + Npgsql |
| Documentación API | Swagger / OpenAPI 3.0 |
| Frontend MVC | Razor + Bootstrap 5 |
| Pruebas | xUnit 2.9.3 + coverlet |
| CI/CD | GitHub Actions |

---

## API REST

### Swagger UI

```
https://localhost:5002/        ← Swagger UI (modo desarrollo)
```

### Endpoints

| Módulo | Endpoint | Método | Descripción |
|--------|----------|--------|-------------|
| **Productos** | `/api/productos` | GET | Lista todos los productos |
| | `/api/productos/{id}` | GET | Obtiene un producto por ID |
| | `/api/productos` | POST | Crea un nuevo producto |
| | `/api/productos/{id}` | PUT | Actualiza un producto |
| | `/api/productos/{id}` | DELETE | Elimina un producto |
| **Categorías** | `/api/categorias` | GET | Lista todas las categorías |
| | `/api/categorias/{id}` | GET | Obtiene una categoría por ID |
| | `/api/categorias` | POST | Crea una nueva categoría |
| | `/api/categorias/{id}` | PUT | Actualiza una categoría |
| | `/api/categorias/{id}` | DELETE | Elimina una categoría |
| **Clientes** | `/api/clientes` | GET | Lista todos los clientes |
| | `/api/clientes/{id}` | GET | Obtiene un cliente por ID |
| | `/api/clientes` | POST | Crea un nuevo cliente |
| | `/api/clientes/{id}` | PUT | Actualiza un cliente |
| | `/api/clientes/{id}` | DELETE | Elimina un cliente |
| **Ventas** | `/api/ventas` | GET | Lista todas las ventas |
| | `/api/ventas/{id}` | GET | Obtiene una venta por ID |
| | `/api/ventas` | POST | Registra una venta (transaccional) |
| | `/api/ventas/caja/{cajaId}` | GET | Ventas de una caja |
| | `/api/ventas/fecha/{fecha}` | GET | Ventas de una fecha |
| **Caja** | `/api/caja` | GET | Lista todas las cajas |
| | `/api/caja/{id}` | GET | Obtiene una caja por ID |
| | `/api/caja/abiertas` | GET | Cajas abiertas |
| | `/api/caja/abrir` | POST | Abre un turno |
| | `/api/caja/{id}/cerrar` | POST | Cierra un turno |
| **Reportes** | `/api/reportes/dashboard` | GET | KPIs del negocio |
| | `/api/reportes/ventas-hoy` | GET | Ventas del día |
| | `/api/reportes/ventas-mes` | GET | Ventas del mes |
| | `/api/reportes/stock-bajo` | GET | Productos con stock < 5 |
| | `/api/reportes/resumen-cajas` | GET | Resumen de cajas |

---

## Ejecución

```bash
cd LocalManager
dotnet restore
dotnet build

# Ejecutar MVC
dotnet run --project LocalManager.Presentation
# Abre https://localhost:5001

# Ejecutar API
dotnet run --project LocalManager.Api
# Abre https://localhost:5002 (Swagger UI)
```

---

## Persistencia con PostgreSQL

Desde el **ADR-08**, la estrategia activa de persistencia (patrón Strategy, ADR-05) es **PostgreSQL** vía Entity Framework Core / Npgsql.

1. Crear la base de datos en pgAdmin (o el gestor de tu preferencia).
2. Configurar `ConnectionStrings:DefaultConnection` en `appsettings.json` de `Presentation` y `Api`.
3. Generar y aplicar las migraciones:
   ```bash
   cd LocalManager.Infrastructure
   dotnet ef migrations add InicialPostgres --startup-project ../LocalManager.Presentation --output-dir Data/Migrations
   dotnet ef database update --startup-project ../LocalManager.Presentation
   ```
4. **Domain y Application no cambian** — la regla de dependencia se respeta; solo se sustituyó el proveedor de EF Core en `Infrastructure`.

Para volver temporalmente a la estrategia JSON (sin base de datos), basta con poner `"UseJsonPersistence": true` en `appsettings.json` — sin recompilar ni modificar código.

---

## Pruebas Automatizadas y CI/CD

El proyecto `LocalManager.xUnit` contiene pruebas unitarias (Arrange-Act-Assert) para tres controladores de la capa `Presentation`, usando fakes en memoria que implementan directamente las interfaces de `Application`:

| Controlador probado | Interfaz mockeada (fake) | Qué valida |
|----------------------|---------------------------|------------|
| `CategoriasController` | `ICategoriaService` | Listado, creación y manejo de ID inexistente |
| `ClientesController` | `IClienteService` | Listado, edición y eliminación |
| `CajaController` | `ICajaService` | Listado, apertura de turno y manejo de ID inexistente |

Ejecutar localmente:

```bash
dotnet test LocalManager.xUnit/LocalManager.xUnit.csproj
```

El pipeline `.github/workflows/ci.yml` ejecuta automáticamente `dotnet restore`, `dotnet build` y `dotnet test` sobre la solución en cada `push` y `pull request`, evitando que una regresión en estos controladores llegue a `main` sin ser detectada.

Ver el razonamiento completo (por qué se eligieron estos controladores y qué queda pendiente de cubrir) en [`ADRs/ADR_07-Jesús-Uc.md`](./ADRs/ADR_07-Jesús-Uc.md).

---

## Transacciones Atómicas

Las ventas siguen un flujo pensado para comportarse como el principio **ACID**:

1. **Validación** — Se verifica stock suficiente para todos los productos
2. **Ejecución** — Venta, detalles y descuento de stock en una sola operación
3. **Persistencia** — Cada paso se guarda a medida que ocurre
4. **Rollback** — Si algo falla dentro del método, se revierte manualmente el stock ya descontado

> ⚠️ **Deuda técnica conocida:** esta atomicidad sigue siendo *simulada en memoria* (Deuda técnica 2 del ADR-06). Con la migración a PostgreSQL (ADR-08) ya existe el motor capaz de resolverlo con una transacción real (`Database.BeginTransaction()`), pero `VentaService.Registrar` todavía no la usa. Este riesgo se analiza formalmente en la [Evaluación ATAM](./ATAM/Evaluacion-ATAM-Jesús-Uc.md).

---

## Diagramas C4 (Niveles 1 a 3)

| Nivel | Diagrama | Descripción |
|-------|----------|--------------|
| 1 — Contexto | [`DiagramaC1-LocalManager.md`](./Diagramas/DiagramaC1-LocalManager.md) | El sistema como una caja, sus actores |
| 2 — Contenedores | [`DiagramaC2-LocalManager.md`](./Diagramas/DiagramaC2-LocalManager.md) | Los 5 proyectos de la solución + PostgreSQL |
| 3 — Componentes | [`DiagramaC3-LM-Domain.md`](./Diagramas/DiagramaC3-LM-Domain.md) | Entidades e interfaces `Repository` |
| 3 — Componentes | [`DiagramaC3-LM-Application.md`](./Diagramas/DiagramaC3-LM-Application.md) | Servicios de negocio |
| 3 — Componentes | [`DiagramaC3-LM-Infrastructure.md`](./Diagramas/DiagramaC3-LM-Infrastructure.md) | `SqlDbContext`, `JsonDbContext` y repositorios |
| 3 — Componentes | [`DiagramaC3-LM-Presentation.md`](./Diagramas/DiagramaC3-LM-Presentation.md) | Controllers y Views MVC |
| 3 — Componentes | [`DiagramaC3-LM-Api.md`](./Diagramas/DiagramaC3-LM-Api.md) | Controllers REST y DTOs |

---

## Evaluación ATAM

Se realizó un análisis de **riesgo**, **trade-off** y **punto de sensibilidad** sobre decisiones arquitectónicas reales del proyecto (la transacción simulada de ventas, la elección de PostgreSQL sobre SQL Server, y la interfaz `IDbContext` del patrón Strategy). Ver el detalle completo en [`ATAM/Evaluacion-ATAM-Jesús-Uc.md`](./ATAM/Evaluacion-ATAM-Jesús-Uc.md).

---

## Migración a SQL Server

1. Cambiar en `appsettings.json` de Presentation y Api:
   ```json
   "UseJsonPersistence": false
   ```

2. Descomentar en `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IDbContext, SqlDbContext>();
   ```

3. Ejecutar migraciones:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Domain y Application no cambian** — la regla de dependencia se respeta.

---

## Deuda Técnica Conocida

Siguiendo la misma disciplina de documentación del resto del proyecto, la deuda técnica identificada se registra formalmente en **ADR-06** en lugar de dejarse implícita en el código:

| # | Deuda | Categoría | Estado |
|---|-------|-----------|--------|
| 1 | Ruta absoluta de máquina local en `appsettings.json` de `LocalManager.Api` + bandera `UseJsonPersistence` sin efecto real | Configuración / Infraestructura | ✅ Pagada — ver **ADR-08** |
| 2 | "Transacción atómica" de `VentaService.Registrar` simulada en memoria, sin garantía real ante interrupciones (cada `SaveChanges()` escribe a disco de inmediato) | Lógica de negocio | Pendiente de pago — analizada en la [Evaluación ATAM](./ATAM/Evaluacion-ATAM-Jesús-Uc.md) |

Ver el detalle completo (qué es, por qué existe, costo de no pagarla y propuesta de solución) en [`ADRs/ADR_06-Jesús-Uc.md`](./ADRs/ADR_06-Jesús-Uc.md).

---

## ADRs

| ADR | Decisión | Estado |
|-----|----------|--------|
| ADR-01 | Estructura base: ASP.NET Core MVC + EF Core + SQL Server | `Actualizado por el ADR-02` |
| ADR-02 | Vistas arquitectónicas: Lógica, Desarrollo, Procesos y Despliegue | `Actualizado por el ADR-03` |
| ADR-03 | Estilo arquitectónico: Clean Architecture en capas | `Actualizado por el ADR-04` |
| ADR-04 | Incorporación de API REST con Swagger | `Actualizado por el ADR-05` |
| ADR-05 | Patrones GOF: Repository (Estructural) + Strategy (Comportamiento) | `Actualizado por el ADR-06` |
| ADR-06 | Deuda técnica identificada: configuración hardcodeada y falsa atomicidad en ventas | `Actualizado por el ADR-07` |
| ADR-07 | Suite de pruebas xUnit y pipeline de Integración Continua | `Actualizado por el ADR-08` |
| ADR-08 | Migración de persistencia JSON a PostgreSQL (pago de la Deuda técnica 1) | `APROBADO` |

---

## Autor

**Jesús Uc** — Proyecto de gestión de negocios locales.

---

## Uso de Inteligencia Artificial

Este documento fue redactado de forma personal. Se utilizó inteligencia artificial como herramienta de apoyo en los siguientes aspectos específicos:

| Área de uso | Descripción |
|-------------|-------------|
| **Comparación de tecnologías y patrones** | Se consultó IA para contrastar alternativas arquitectónicas y de patrones GOF, validando que las decisiones fueran coherentes con las restricciones del proyecto. La decisión final fue tomada por el autor. |
| **Corrección de sintaxis Markdown** | Se empleó IA para revisar la sintaxis del documento, asegurando el correcto renderizado de tablas, listas y bloques de código. |
| **Estructuración de diagramas** | Se usó IA como apoyo para organizar la representación visual de la arquitectura, incluyendo la actualización de los diagramas C4 tras el ADR-08. |
| **Actualización con deuda técnica (ADR-06)** | Se usó IA para redactar las notas de deuda técnica añadidas a este README y enlazarlas con el ADR-06, a partir de la inspección de código realizada previamente. |
| **Actualización con pruebas y CI/CD (ADR-07)** | Se usó IA para diseñar la suite de pruebas de `LocalManager.xUnit`, redactar el pipeline `ci.yml` y documentar en este README la sección de Pruebas Automatizadas y CI/CD, enlazándola con el ADR-07. |
| **Migración a PostgreSQL (ADR-08)** | Se usó IA para implementar `SqlDbContext`, configurar EF Core/Npgsql, generar el script de datos de prueba y documentar esta sección y la de Diagramas C4 / Evaluación ATAM. |

> **Nota:** El análisis de contexto, la toma de decisiones arquitectónicas y la redacción del razonamiento son de autoría propia. La IA no generó contenido de fondo de este README de forma autónoma.