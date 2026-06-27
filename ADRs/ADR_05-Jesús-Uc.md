# ADR-05: Integración de Patrones de Diseño GOF

| Campo  | Valor |
|--------|-------|
| Autor  | Jesús Uc |
| Fecha  | 26/06/2026 |
| Estado | `APROBADO` |

---

## Contexto

En los ADRs anteriores se estableció la arquitectura base de **Local Manager**:

- **ADR-01/02:** MVC con ASP.NET Core + EF Core + SQL Server.
- **ADR-03:** Arquitectura en Capas (Clean Architecture) con cuatro proyectos: `Domain`, `Application`, `Infrastructure`, `Presentation`.
- **ADR-04:** Incorporación de `LocalManager.Api` como quinto proyecto REST + Swagger.

Al avanzar en la implementación del sistema, surgió la necesidad de formalizar y documentar los **patrones de diseño GOF (Gang of Four)** utilizados, ya que el código comenzó a resolverlos de forma natural sin haberlos nombrado explícitamente. Esta decisión tiene dos objetivos:

1. **Reconocer** los patrones que ya operan en el sistema, documentando por qué fueron la solución correcta.
2. **Integrar formalmente** al menos dos patrones de **categorías distintas** que resuelvan problemas concretos del sistema, siguiendo el requisito de la actividad.

Restricciones que se mantienen:

- La solución sigue siendo un **monolito de despliegue** en cinco proyectos bajo `LocalManager.sln`.
- La regla de dependencia hacia el centro de Clean Architecture no se puede violar.
- El sistema actualmente utiliza **JSON como persistencia temporal** (`JsonDbContext`) antes de migrar a SQL Server con EF Core.

---

## Decisión

Se integran y documentan formalmente **dos patrones GOF de categorías distintas**:

| # | Patrón | Categoría GOF | Problema que resuelve en el sistema |
|---|--------|---------------|--------------------------------------|
| 1 | **Repository** | **Estructural** | Aislar la lógica de acceso a datos de la lógica de negocio |
| 2 | **Strategy** | **Comportamiento** | Intercambiar la estrategia de persistencia (JSON → SQL Server) sin modificar los servicios |

---

## Patrón 1: Repository (Estructural)

### Problema concreto

Los servicios de negocio (`VentaService`, `ProductoService`, `CajaService`, etc.) necesitan acceder a los datos del sistema —productos, ventas, clientes— sin acoplarse al mecanismo de almacenamiento concreto. Si `VentaService` llamara directamente a `JsonDbContext` o a `DbContext` de EF Core, un cambio de JSON a SQL Server obligaría a modificar la lógica de negocio, violando el principio de inversión de dependencias definido en el ADR-03.

### Cómo se implementa

El patrón Repository ya opera en el sistema como parte de la arquitectura en capas, pero se formaliza aquí:

- **`Domain`** define las interfaces de repositorio (contratos puros):
  ```
  IProductoRepository  → ObtenerTodos(), ObtenerPorId(), Agregar(), Actualizar(), Eliminar()
  IVentaRepository     → ObtenerTodas(), ObtenerPorId(), Agregar(), AgregarDetalle(), ...
  ICajaRepository      → ObtenerTodas(), ObtenerAbiertas(), Abrir(), Cerrar(), ...
  ```

- **`Infrastructure`** implementa esas interfaces usando `JsonDbContext` (actualmente) o `AppDbContext` de EF Core (futuro):
  ```
  ProductoRepository : IProductoRepository  → usa JsonDbContext
  VentaRepository    : IVentaRepository     → usa JsonDbContext
  CajaRepository     : ICajaRepository      → usa JsonDbContext
  ```

- **`Application`** solo conoce las interfaces, nunca las implementaciones concretas:
  ```csharp
  // VentaService.cs — solo recibe IVentaRepository, IProductoRepository, IClienteRepository
  public VentaService(IVentaRepository ventaRepository,
                      IProductoRepository productoRepository,
                      IClienteRepository clienteRepository)
  ```

### Por qué este patrón y no otro

El problema a resolver es **aislar el acceso a datos**. Las alternativas evaluadas fueron:

| Alternativa | Por qué no |
|-------------|------------|
| **Active Record** (la entidad misma hace las consultas) | Viola Clean Architecture: las entidades de `Domain` estarían acopladas a `JsonDbContext` o EF Core, que son de `Infrastructure`. Esto rompería la regla de dependencia hacia el centro. |
| **DAO (Data Access Object)** | Funcionalmente similar a Repository, pero sin el concepto de colección en memoria. Para un sistema que opera con listas de entidades (inventario, ventas), Repository es más expresivo y es el patrón estándar de .NET y EF Core. |
| **Acceso directo desde los servicios** | Ya descartado en ADR-03: nada impediría que un servicio llamara directamente a `JsonDbContext`, mezclando infraestructura con negocio. El compilador no lo protegería. |

### Consecuencias

✅ `VentaService` puede registrar una venta y descontar stock sin saber si los datos están en JSON, SQL Server o cualquier otro motor. La lógica transaccional del ADR-01 ("venta + descuento de stock = operación atómica") vive en `Application`, no en `Infrastructure`.

✅ Al migrar de JSON a SQL Server, solo se reemplaza la implementación en `Infrastructure`. Los servicios, los controladores MVC y los controladores de la API no cambian.

⚠️ Cada entidad requiere su propia interfaz y su propia implementación, lo que aumenta el número de archivos. Este costo ya se aceptó en ADR-03 como parte de la arquitectura en capas.

---

## Patrón 2: Strategy (Comportamiento)

### Problema concreto

El sistema actualmente usa **`JsonDbContext`** como mecanismo de persistencia temporal (archivos JSON en disco), porque la actividad no requería SQL Server aún. Sin embargo, el plan desde el ADR-01 fue migrar a **SQL Server con EF Core** (`AppDbContext`). El problema es: ¿cómo se intercambia el motor de persistencia sin modificar los repositorios, los servicios ni los controladores?

Sin un patrón formal, cambiar la fuente de datos implicaría buscar y reemplazar referencias a `JsonDbContext` en todos los repositorios, con alto riesgo de introducir errores en la lógica de negocio.

### Cómo se implementa

El patrón Strategy se aplica sobre la capa de persistencia, definiendo un **contexto de datos intercambiable**:

1. Se define una interfaz `IDbContext` en `Infrastructure` que expone las operaciones genéricas de persistencia:
   ```csharp
   public interface IDbContext
   {
       List<T> Set<T>() where T : class;
       void Add<T>(T entity) where T : class;
       void Update<T>(T entity) where T : class;
       void Remove<T>(int id) where T : class;
       T? Find<T>(int id) where T : class;
       int SaveChanges();
   }
   ```

2. Las dos **estrategias concretas** implementan esa interfaz:
   - `JsonDbContext : IDbContext` — estrategia actual (archivos JSON, sin dependencias externas).
   - `SqlDbContext : IDbContext` — estrategia futura (EF Core + SQL Server).

3. Los repositorios reciben `IDbContext` por inyección de dependencias, sin saber cuál estrategia está activa:
   ```csharp
   // ProductoRepository.cs — usa IDbContext, no sabe si es JSON o SQL
   public class ProductoRepository : IProductoRepository
   {
       private readonly IDbContext _context;
       public ProductoRepository(IDbContext context) { _context = context; }

       public List<Producto> ObtenerTodos() => _context.Set<Producto>();
       public Producto? ObtenerPorId(int id) => _context.Find<Producto>(id);
       public void Agregar(Producto p) { _context.Add(p); _context.SaveChanges(); }
       public void Actualizar(Producto p) { _context.Update(p); _context.SaveChanges(); }
       public void Eliminar(int id) { _context.Remove<Producto>(id); _context.SaveChanges(); }
   }
   ```

4. El **selector de estrategia** vive en `Program.cs` (configuración de inyección de dependencias):
   ```csharp
   // Program.cs — aquí se elige la estrategia activa
   var usarJson = builder.Configuration.GetValue<bool>("UseJsonPersistence");

   if (usarJson)
       builder.Services.AddSingleton<IDbContext, JsonDbContext>();
   else
       builder.Services.AddScoped<IDbContext, SqlDbContext>();
   ```

   La estrategia se controla con una sola línea en `appsettings.json`:
   ```json
   {
     "UseJsonPersistence": true
   }
   ```
   Cambiar a `false` activa SQL Server sin tocar ningún repositorio, servicio ni controlador.

### Por qué este patrón y no otro

El problema a resolver es **intercambiar el mecanismo de persistencia sin modificar el código existente**. Las alternativas evaluadas fueron:

| Alternativa | Por qué no |
|-------------|------------|
| **Template Method** | Define el esqueleto de un algoritmo en una clase base, dejando pasos a las subclases. Resolvería el intercambio, pero requeriría que `JsonDbContext` y `SqlDbContext` hereden de una clase base abstracta. La herencia es más rígida que la composición vía interfaz, y en .NET la inyección de dependencias funciona mejor con interfaces que con herencia. |
| **Abstract Factory** | Crearía familias de objetos relacionados (un factory para JSON y otro para SQL). Es más complejo de lo necesario: solo hay un tipo de contexto que intercambiar, no familias de objetos. Strategy con una interfaz simple es suficiente. |
| **Hardcodear la implementación** | Ya está descartado: implica buscar y reemplazar `JsonDbContext` en todos los repositorios al migrar a SQL Server, con alto riesgo de errores y sin control del compilador. |
| **Factory Method** | Podría crear la instancia correcta del contexto, pero no resuelve el intercambio en tiempo de configuración sin levantar la aplicación. La inyección de dependencias de .NET ya actúa como el selector de estrategia de forma más limpia. |

### Consecuencias

✅ La migración de JSON a SQL Server se reduce a cambiar `"UseJsonPersistence": false` en `appsettings.json` y registrar las migraciones de EF Core. Ningún repositorio, servicio, controlador MVC ni controlador de la API requiere modificaciones.

✅ Durante el desarrollo y las pruebas locales se puede usar `JsonDbContext` sin necesidad de instalar SQL Server. En producción se activa `SqlDbContext`. Ambos entornos comparten exactamente el mismo código de repositorios y servicios.

✅ Si en el futuro se quisiera agregar una tercera estrategia (por ejemplo, persistencia en memoria para pruebas unitarias), solo se implementa `IDbContext` y se registra en el contenedor de DI. El resto del sistema no cambia.

⚠️ La interfaz `IDbContext` expone operaciones genéricas (`Set<T>`, `Find<T>`) que funcionan para entidades simples, pero operaciones avanzadas de SQL (joins complejos, consultas con includes de EF Core) requerirían extender la interfaz o moverlas a los repositorios concretos que sí conozcan el contexto SQL. Esto se resolverá al implementar `SqlDbContext` en la siguiente etapa del proyecto.

---

## Relación entre los patrones y la arquitectura existente

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        LocalManager.sln                                 │
│                                                                         │
│  ┌─────────────┐    ┌──────────────────┐    ┌────────────────┐          │
│  │   Domain    │    │   Application    │    │ Presentation / │          │
│  │             │◄───│                  │◄───│     Api        │          │
│  │ IProducto-  │    │ VentaService     │    │ Controllers    │          │
│  │ Repository  │    │ ProductoService  │    │ (MVC + REST)   │          │
│  │ IVentaRepo  │    │  (usa IRepo's)   │    │                │          │
│  └──────┬──────┘    └──────────────────┘    └────────────────┘          │
│         │                                                               │
│  [PATRÓN REPOSITORY: Domain define contratos, Infrastructure implementa]│
│         │                                                               │
│  ┌──────▼───────────────────────────────────────────┐                   │
│  │                  Infrastructure                  │                   │
│  │                                                  │                   │
│  │  ProductoRepository(IDbContext) ──────────────►  │                   │
│  │  VentaRepository(IDbContext)    ──────────────►  │                   │
│  │  CajaRepository(IDbContext)     ──────────────►  │                   │
│  │                                                  │                   │
│  │  [PATRÓN STRATEGY: IDbContext intercambiable]    │                   │
│  │                                                  │                   │
│  │  IDbContext ◄─── JsonDbContext  (desarrollo)     │                   │
│  │  IDbContext ◄─── SqlDbContext   (producción)     │                   │
│  └──────────────────────────────────────────────────┘                   │
└─────────────────────────────────────────────────────────────────────────┘
```

El patrón **Repository** opera entre `Domain` (contratos) e `Infrastructure` (implementaciones), protegiendo a `Application` del acceso directo a datos.

El patrón **Strategy** opera dentro de `Infrastructure`, permitiendo que los repositorios sean independientes del motor de persistencia concreto.

Ambos patrones refuerzan la **regla de dependencia hacia el centro** establecida en el ADR-03: ningún cambio en `Infrastructure` (cambio de JSON a SQL Server) afecta a `Domain`, `Application`, `Presentation` ni `Api`.

---

## 🤖 Uso de Inteligencia Artificial

Este documento fue redactado de forma personal. Se utilizó inteligencia artificial como herramienta de apoyo en los siguientes aspectos específicos:

| Área de uso | Descripción |
|-------------|-------------|
| **Corrección de sintaxis Markdown** | Se empleó IA para revisar la sintaxis del documento, asegurando el correcto renderizado de tablas, bloques de código y encabezados. |
| **Revisión del diagrama ASCII** | Se usó IA para verificar que el diagrama reflejara correctamente las dependencias entre patrones y capas descritas en el texto. |

> **Nota:** La identificación de los problemas concretos en el sistema, la selección de los patrones GOF aplicables, el análisis de alternativas y la definición de consecuencias son de autoría propia. La IA no seleccionó los patrones ni generó el razonamiento de negocio de este ADR.
