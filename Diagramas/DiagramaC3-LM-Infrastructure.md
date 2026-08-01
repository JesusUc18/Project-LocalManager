# C4 Nivel 3 — Componentes — dentro de LocalManager.Infrastructure

```mermaid
graph TD
    subgraph Infrastructure["LocalManager.Infrastructure"]
        subgraph Data["Data «Strategy (GOF Comportamiento)»"]
            IDbContext["IDbContext<br/>«interfaz Strategy»"]
            JsonDbContext["JsonDbContext<br/>(estrategia: archivos JSON)"]
            SqlDbContext["SqlDbContext<br/>(estrategia: PostgreSQL)"]
            AppDbContext["AppDbContext<br/>(DbContext de EF Core / Npgsql)"]
        end
        subgraph Repos["Repositories «Repository (GOF Estructural)»"]
            CategoriaRepository["CategoriaRepository"]
            ProductoRepository["ProductoRepository"]
            ClienteRepository["ClienteRepository"]
            VentaRepository["VentaRepository"]
            CajaRepository["CajaRepository"]
        end
    end

    DomainInterfaces[("LocalManager.Domain<br/>Interfaces/Repositories")]
    PostgreSQL[("PostgreSQL<br/>Base de datos")]

    JsonDbContext -.->|"implementa"| IDbContext
    SqlDbContext -.->|"implementa"| IDbContext
    SqlDbContext -->|"usa"| AppDbContext
    AppDbContext -->|"UseNpgsql"| PostgreSQL

    CategoriaRepository -->|"implementa"| DomainInterfaces
    ProductoRepository -->|"implementa"| DomainInterfaces
    ClienteRepository -->|"implementa"| DomainInterfaces
    VentaRepository -->|"implementa"| DomainInterfaces
    CajaRepository -->|"implementa"| DomainInterfaces

    CategoriaRepository -->|"usa"| IDbContext
    ProductoRepository -->|"usa"| IDbContext
    ClienteRepository -->|"usa"| IDbContext
    VentaRepository -->|"usa"| IDbContext
    CajaRepository -->|"usa"| IDbContext

    style Infrastructure fill:#f5f5f5,stroke:#999
```

**Para quién es:** desarrolladores que trabajan en persistencia o que dan mantenimiento a la conexión con PostgreSQL.

**¿Cómo se guardan los datos y cómo se puede cambiar el mecanismo sin romper nada?** Aquí conviven los dos patrones GOF del ADR-05: **Repository** (los `*Repository` implementan las interfaces de `Domain`) y **Strategy** (`IDbContext` permite intercambiar `JsonDbContext` por `SqlDbContext` con una sola línea en `appsettings.json` — `UseJsonPersistence` —, sin tocar repositorios ni servicios).

> **Actualización (ADR-08):** `SqlDbContext` ya no es un componente "preparado" sino la estrategia **activa**. No implementa el acceso a PostgreSQL directamente: envuelve a `AppDbContext` (el `DbContext` real de Entity Framework Core, configurado con el proveedor `Npgsql`) y traduce sus operaciones al contrato `List<T> Set/Add/Update/Remove/Find/SaveChanges` que ya esperaban los repositorios desde el ADR-05. Ningún repositorio cambió una sola línea para lograr esto — es la prueba en código de que el patrón Strategy cumple su propósito.