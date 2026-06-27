# Local Manager — Sistema de Gestión para Negocios Locales

> **Estado:** `APROBADO` | **Autor:** Jesús Uc | **Fecha:** 26/06/2026

Sistema de gestión de negocios locales con **Clean Architecture + Patrones GOF + API REST**. Permite controlar ventas, inventario, clientes y caja desde cualquier navegador, con una API documentada con Swagger/OpenAPI para consumo por múltiples clientes.

---

## Arquitectura (5 Proyectos)

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
└── LocalManager.Api/                 ← ASP.NET Core Web API + Swagger
    ├── Controllers/                  → ProductosApi, VentasApi, CajaApi, ReportesApi
    └── Models/                       → DTOs: ApiResponse, CrearVentaRequest
```

### Regla de Dependencia

```
Api → Application → Domain
Presentation → Application → Domain
      ↓            ↓
Infrastructure ←──┘
```

`Domain` no conoce `Infrastructure`, `Presentation` ni `Api`. El compilador lo garantiza.

---

## Patrones de Diseño GOF (ADR-05)

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

## Transacciones Atómicas

Las ventas implementan el principio **ACID**:

1. **Validación** — Se verifica stock suficiente para todos los productos
2. **Ejecución** — Venta, detalles y descuento de stock en una sola operación
3. **Persistencia** — Todo se guarda de forma atómica
4. **Rollback** — Si algo falla, nada se persiste

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

## ADRs

| ADR | Decisión | Estado |
|-----|----------|--------|
| ADR-01 | Estructura base: ASP.NET Core MVC + EF Core + SQL Server | `APROBADO` |
| ADR-02 | Vistas arquitectónicas: Lógica, Desarrollo, Procesos y Despliegue | `APROBADO` |
| ADR-03 | Estilo arquitectónico: Clean Architecture en capas | `APROBADO` |
| ADR-04 | Incorporación de API REST con Swagger | `APROBADO` |
| ADR-05 | Patrones GOF: Repository (Estructural) + Strategy (Comportamiento) | `APROBADO` |

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

> **Nota:** El análisis de contexto, la toma de decisiones arquitectónicas y la redacción del razonamiento son de autoría propia. La IA no generó contenido de fondo de este README de forma autónoma.