# C4 Nivel 3 — Componentes — dentro de LocalManager.Infrastructure

```mermaid
graph TD
    subgraph Infrastructure["LocalManager.Infrastructure"]
        subgraph Data["Data «Strategy (GOF Comportamiento)»"]
            IDbContext["IDbContext<br/>«interfaz Strategy»"]
            JsonDbContext["JsonDbContext<br/>(estrategia actual)"]
            AppDbContext["AppDbContext<br/>(EF Core / SQL Server, preparado)"]
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

    JsonDbContext -.->|"implementa"| IDbContext
    AppDbContext -.->|"implementa"| IDbContext

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

**Para quién es:** desarrolladores que trabajan en persistencia o van a habilitar SQL Server.

**¿Cómo se guardan los datos y cómo se puede cambiar el mecanismo sin romper nada?** Aquí conviven los dos patrones GOF del ADR-05: **Repository** (los `*Repository` implementan las interfaces de `Domain`) y **Strategy** (`IDbContext` permite intercambiar `JsonDbContext` por `AppDbContext` con una sola línea en `appsettings.json`, sin tocar repositorios ni servicios).