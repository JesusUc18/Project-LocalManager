# Local Manager — Sistema de Gestión para Negocios Locales

> **Estado:** `APROBADO` | **Autor:** Jesús Uc | **Última actualización:** 22/07/2026

Sistema de gestión de negocios locales con **Clean Architecture + Patrones GOF + API REST**. Permite controlar ventas, inventario, clientes y caja desde cualquier navegador, con una API documentada con Swagger/OpenAPI para consumo por múltiples clientes. Cuenta además con una suite de pruebas automatizadas con **xUnit** y un pipeline de **Integración Continua** en GitHub Actions.

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
│   │   ├── JsonDbContext.cs          → Estrategia JSON (actual)
│   │   └── AppDbContext.cs           → Estrategia SQL Server (preparado)
│   └── Repositories/                 → Implementaciones Repository (patrón GOF)
│
├── LocalManager.Presentation/        ← ASP.NET Core MVC
│   ├── Controllers/
│   ├── Views/
│   └── Data/                         → Archivos JSON compartidos (fuente única de datos)
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
"UseJsonPersistence": true   ← JSON (desarrollo)
"UseJsonPersistence": false  ← SQL Server (producción)
```

```
IDbContext ←── JsonDbContext   (actual)
IDbContext ←── SqlDbContext    (futuro)
```

Los repositorios reciben `IDbContext`, nunca la implementación concreta.

> ⚠️ **Deuda técnica conocida:** en `Program.cs` (Presentation y Api), las dos ramas del `if (usarJson)` registran actualmente `JsonDbContext` — la bandera `UseJsonPersistence` todavía no selecciona una estrategia distinta porque `SqlDbContext` no está implementado. Además, `LocalManager.Api/appsettings.json` apunta `JsonDatabase:DataPath` a una ruta absoluta de la máquina del autor en vez de una ruta relativa o variable de entorno. Ver **ADR-06** para el detalle y la propuesta de solución.

Este mismo desacoplamiento (controladores de `Presentation` dependiendo solo de interfaces de `Application`) es lo que permite probar los controladores en `LocalManager.xUnit` con fakes en memoria, sin necesitar `Infrastructure` ni una base de datos real. Ver **ADR-07**.

---

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8 |
| Patrón arquitectónico | MVC + Clean Architecture |
| Patrones de diseño | Repository (GOF Estructural) + Strategy (GOF Comportamiento) |
| Base de datos | JSON (temporal) / SQL Server (preparado) |
| ORM | Entity Framework Core 8 (preparado) |
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

Ver el razonamiento completo (por qué se eligieron estos controladores y qué queda pendiente de cubrir) en [`ADRs/ADR-07-Jesús-Uc.md`](./ADRs/ADR-07-Jesús-Uc.md).

---

## Transacciones Atómicas

Las ventas siguen un flujo pensado para comportarse como el principio **ACID**:

1. **Validación** — Se verifica stock suficiente para todos los productos
2. **Ejecución** — Venta, detalles y descuento de stock en una sola operación
3. **Persistencia** — Cada paso se guarda a medida que ocurre
4. **Rollback** — Si algo falla dentro del método, se revierte manualmente el stock ya descontado

> ⚠️ **Deuda técnica conocida:** esta atomicidad es actualmente *simulada en memoria*, no una transacción real de base de datos — con `JsonDbContext`, cada operación escribe a disco de inmediato, por lo que una interrupción a mitad del proceso puede dejar los archivos JSON inconsistentes. El rollback manual no cubre ese caso. Ver **ADR-06** para el detalle y la propuesta de solución (Unit of Work / transacción real con `SqlDbContext`).

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
| 1 | Ruta absoluta de máquina local en `appsettings.json` de `LocalManager.Api` + bandera `UseJsonPersistence` sin efecto real | Configuración / Infraestructura | Pendiente de pago |
| 2 | "Transacción atómica" de `VentaService.Registrar` simulada en memoria, sin garantía real ante interrupciones (cada `SaveChanges()` escribe a disco de inmediato) | Lógica de negocio | Pendiente de pago |

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
| ADR-07 | Suite de pruebas xUnit y pipeline de Integración Continua | `APROBADO` |

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
| **Estructuración de diagramas** | Se usó IA como apoyo para organizar la representación visual de la arquitectura. |
| **Actualización con deuda técnica (ADR-06)** | Se usó IA para redactar las notas de deuda técnica añadidas a este README y enlazarlas con el ADR-06, a partir de la inspección de código realizada previamente. |
| **Actualización con pruebas y CI/CD (ADR-07)** | Se usó IA para diseñar la suite de pruebas de `LocalManager.xUnit`, redactar el pipeline `ci.yml` y documentar en este README la sección de Pruebas Automatizadas y CI/CD, enlazándola con el ADR-07. |

> **Nota:** El análisis de contexto, la toma de decisiones arquitectónicas y la redacción del razonamiento son de autoría propia. La IA no generó contenido de fondo de este README de forma autónoma.