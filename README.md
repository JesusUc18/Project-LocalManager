# Local Manager — Versión Clean Architecture

> **Estado:** `APROBADO` | **Autor:** Jesús Uc | **Fecha:** 12/06/2026

Aplicación web para la gestión de negocios locales con **Arquitectura en Capas (Clean Architecture)**. La lógica de negocio está aislada del framework de presentación y del motor de base de datos.

---

## Arquitectura (4 Proyectos)

```
LocalManager/
├── LocalManager.sln
├── LocalManager.Domain/              ← CENTRO (no depende de nadie)
│   ├── Entities/                     → Categoria, Producto, Cliente, Venta, Caja
│   └── Interfaces/Repositories/    → ICategoriaRepository, etc.
│
├── LocalManager.Application/         ← Reglas de negocio (solo Domain)
│   ├── Services/                     → ICategoriaService, IProductoService, etc.
│   └── Services/                     → CategoriaService, ProductoService, VentaService
│
├── LocalManager.Infrastructure/      ← Persistencia (solo Domain)
│   ├── Data/
│   │   ├── AppDbContext.cs         → EF Core preparado para SQL Server
│   │   └── JsonDbContext.cs         → JSON temporal (actual)
│   └── Repositories/                 → CategoriaRepository, ProductoRepository, etc.
│
└── LocalManager.Presentation/          ← ASP.NET Core MVC
    ├── Controllers/                  → Home, Productos, Ventas, Caja, Reportes
    └── Views/                        → Razor + Bootstrap 5
```

### Regla de Dependencia

```
Presentation → Application → Domain
      ↓            ↓
Infrastructure ←──┘
```

`Domain` no conoce `Infrastructure` ni `Presentation`. El compilador lo garantiza.

---

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8 |
| Patrón | MVC + Clean Architecture |
| Base de datos | JSON (temporal) / SQL Server (preparado) |
| ORM | Entity Framework Core 8 (preparado) |
| Frontend | Razor + Bootstrap 5 |

---

## Módulos

- **Dashboard** — KPIs del negocio
- **Productos** — CRUD + stock + categoría + código de barras
- **Categorías** — Clasificación de productos
- **Clientes** — Registro de clientes
- **Ventas** — Registro transaccional con múltiples productos
- **Caja** — Apertura/cierre de turnos con control de montos
- **Reportes** — Ventas del día/mes, stock bajo, resumen de cajas

---

## Transacciones Atómicas

Las ventas implementan el principio **ACID**:

1. **Validación** — Se verifica stock suficiente para todos los productos
2. **Ejecución** — Todo en memoria (venta, detalles, descuento de stock)
3. **Persistencia** — `SaveChanges()` guarda todo de forma atómica
4. **Rollback** — Si algo falla, nada se persiste

---

## Ejecución

```bash
cd LocalManager
dotnet restore
dotnet build

# Ejecutar MVC
dotnet run --project LocalManager.Presentation
```

Abre `https://localhost:5001`

---

## Migración a SQL Server

1. Descomentar en `Program.cs`:
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

`APROBADO` — Arquitectura en capas lista para desarrollo iterativo y escalabilidad futura.


**ESTE README ES TEMPORAL**
