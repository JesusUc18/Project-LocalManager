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


## Capturas de pantalla

### DASHBOARD (panel general):
<img width="1899" height="473" alt="image" src="https://github.com/user-attachments/assets/67d1def1-8430-4b5b-b252-7ec0e68a4d7c" />


### VENTAS

Historial:
<img width="1904" height="454" alt="image" src="https://github.com/user-attachments/assets/852811f1-8c75-4593-89de-49e6b2cb4dea" />

Nueva Venta:
<img width="1904" height="594" alt="image" src="https://github.com/user-attachments/assets/6c30f0c1-0501-48d2-92f3-5f06f359b4d9" />

Registro de venta:
<img width="1904" height="466" alt="image" src="https://github.com/user-attachments/assets/0752e528-0957-4b7c-bb68-7e2151836f7e" />


### PRODUCTOS

Listado de Productos:
<img width="1902" height="468" alt="image" src="https://github.com/user-attachments/assets/9a32d46b-6b9b-4014-aae4-53a1a899a2e7" />

Nuevo Producto:
<img width="1904" height="678" alt="image" src="https://github.com/user-attachments/assets/98dac934-1692-4a2d-88c0-c92ce6e7e800" />

Registro de Producto:
<img width="1902" height="490" alt="image" src="https://github.com/user-attachments/assets/2de20286-7b56-4d1a-879b-fb179c0b4b28" />


### CATEGORÍAS

Listado:
<img width="1903" height="459" alt="image" src="https://github.com/user-attachments/assets/f683da23-39d0-4e72-81bd-f3243ff868cc" />

Agregar Categoría:
<img width="1903" height="455" alt="image" src="https://github.com/user-attachments/assets/d282ee4f-8d4b-4430-aa00-083af90729f9" />

Editar Categoría:
<img width="1899" height="460" alt="image" src="https://github.com/user-attachments/assets/1234168e-97c1-4fa7-922a-71c53d00e048" />


### CLIENTES

Listado:
<img width="1904" height="447" alt="image" src="https://github.com/user-attachments/assets/ec4c5d5a-c420-4c0b-bc68-68543807cc95" />

Agregar Cliente:
<img width="1899" height="604" alt="image" src="https://github.com/user-attachments/assets/c41d883d-44f1-4291-b14e-aee0b0ff7f0d" />

Editar Cliente:
<img width="1902" height="612" alt="image" src="https://github.com/user-attachments/assets/33410c7f-171e-4921-a7f4-ae0d86ac20a5" />


### CAJAS:

Control de Cajas:
<img width="1904" height="449" alt="image" src="https://github.com/user-attachments/assets/81d9e6d9-055e-4fd3-87a7-f635e57d788b" />

Abrir Caja:
<img width="1906" height="455" alt="image" src="https://github.com/user-attachments/assets/82129c96-8f22-4b13-bb12-3d25c6cc4029" />

Detalle de Caja:
<img width="1902" height="451" alt="image" src="https://github.com/user-attachments/assets/03129fce-a68f-47fa-8244-70017bcc59e6" />


### REPORTES
<img width="1901" height="556" alt="image" src="https://github.com/user-attachments/assets/b53d097b-30e6-472d-92f2-031187bc1865" />


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
