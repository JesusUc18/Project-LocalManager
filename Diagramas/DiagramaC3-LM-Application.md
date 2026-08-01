# C4 Nivel 3 — Componentes — dentro de LocalManager.Application

```mermaid
graph TD
    subgraph Application["LocalManager.Application"]
        subgraph Services["Services"]
            CategoriaService["CategoriaService<br/>: ICategoriaService"]
            ProductoService["ProductoService<br/>: IProductoService"]
            ClienteService["ClienteService<br/>: IClienteService"]
            VentaService["VentaService<br/>: IVentaService"]
            CajaService["CajaService<br/>: ICajaService"]
        end
    end

    DomainRepos[("LocalManager.Domain<br/>Interfaces/Repositories")]

    CategoriaService -->|"usa ICategoriaRepository"| DomainRepos
    ProductoService -->|"usa IProductoRepository"| DomainRepos
    ClienteService -->|"usa IClienteRepository"| DomainRepos
    VentaService -->|"usa IVentaRepository,<br/>IProductoRepository,<br/>IClienteRepository"| DomainRepos
    CajaService -->|"usa ICajaRepository"| DomainRepos

    style Application fill:#f5f5f5,stroke:#999
```

**Para quién es:** desarrolladores que implementan o modifican reglas de negocio.

**¿Dónde vive la lógica de negocio y de qué depende?** Cada servicio recibe **solo interfaces** de `Domain` (nunca implementaciones concretas de `Infrastructure`), lo que garantiza que `Application` pueda cambiarse de persistencia (JSON ↔ SQL Server) sin tocar una sola línea de este proyecto.