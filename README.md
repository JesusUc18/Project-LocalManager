# Local Manager — Versión API REST

> **Estado:** `APROBADO` | **Autor:** Jesús Uc | **Fecha:** 19/06/2026

Sistema de gestión de negocios locales con **Arquitectura en Capas + API REST**. Expone endpoints documentados con Swagger/OpenAPI para consumo por cualquier cliente: navegadores, apps móviles, integraciones.

---

## Arquitectura (5 Proyectos)

```
LocalManager/
├── LocalManager.sln
├── LocalManager.Domain/              ← CENTRO (no depende de nadie)
│   ├── Entities/                     → Categoria, Producto, Cliente, Venta, Caja
│   └── Interfaces/Repositories/    → ICategoriaRepository, etc.
│
├── LocalManager.Application/         ← Reglas de negocio (solo Domain)
│   └── Services/                     → CategoriaService, ProductoService, VentaService, etc.
│
├── LocalManager.Infrastructure/      ← Persistencia (solo Domain)
│   ├── Data/
│   │   ├── AppDbContext.cs         → EF Core preparado para SQL Server
│   │   └── JsonDbContext.cs         → JSON temporal (actual)
│   └── Repositories/                 → Implementaciones de repositorios
│
├── LocalManager.Presentation/          ← ASP.NET Core MVC (sin cambios)
│   ├── Controllers/
│   └── Views/
│
└── LocalManager.Api/                 ← ASP.NET Core Web API + Swagger (NUEVO)
    ├── Controllers/                  → ProductosApi, VentasApi, CajaApi, ReportesApi
    ├── Models/                       → DTOs: ApiResponse, CrearVentaRequest
    └── wwwroot/
        ├── index.html                → Tester interactivo de la API
        └── style.css                 → Estilos del tester
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

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8 |
| Patrón | MVC + Clean Architecture + REST |
| Base de datos | JSON (temporal) / SQL Server (preparado) |
| ORM | Entity Framework Core 8 (preparado) |
| Documentación API | Swagger / OpenAPI 3.0 |
| Frontend MVC | Razor + Bootstrap 5 |
| Frontend API | HTML + CSS + JavaScript vanilla (tester) |

---

## API REST

### Swagger UI

```
https://localhost:5002/          ← Swagger UI en raíz (modo desarrollo)
https://localhost:5002/swagger   ← Alternativa
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

### Tester Interactivo

Abre `https://localhost:5002/index.html` para probar todos los endpoints desde una interfaz web sin necesidad de Postman.

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
# O https://localhost:5002/index.html (Tester interactivo)
```

---

## Transacciones Atómicas

Las ventas implementan el principio **ACID**:

1. **Validación** — Se verifica stock suficiente para todos los productos
2. **Ejecución** — Todo en memoria (venta, detalles, descuento de stock)
3. **Persistencia** — Todo se guarda de forma atómica
4. **Rollback** — Si algo falla, nada se persiste

---

## Migración a SQL Server

1. Descomentar en `Program.cs` de Presentation y Api:
   ```csharp
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

2. Ejecutar:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Domain y Application no cambian** — la regla de dependencia se respeta.

---

## Autor

**Jesús Uc** — Proyecto de gestión de negocios locales.

---

## Estado

`APROBADO` — MVC + API REST + Swagger funcionando. Listo para desarrollo iterativo y consumo por múltiples clientes.

**ESTE README ES TEMPORAL**