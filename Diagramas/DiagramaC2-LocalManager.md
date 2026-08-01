# C4 Nivel 2 — Contenedores — LocalManager

```mermaid
graph TD
    Dueno["👤 Dueño del negocio"]
    ClienteExt["👤 Sistema externo"]

    subgraph LocalManager["LocalManager (Sistema)"]
        Presentation["LocalManager.Presentation<br/>[ASP.NET Core MVC]<br/>Vistas Razor + Controllers"]
        Api["LocalManager.Api<br/>[ASP.NET Core Web API]<br/>Controllers REST + Swagger"]
        Application["LocalManager.Application<br/>[Class Library]<br/>Servicios de negocio"]
        Infrastructure["LocalManager.Infrastructure<br/>[Class Library]<br/>Repositorios + Strategy de persistencia"]
        Domain["LocalManager.Domain<br/>[Class Library]<br/>Entidades + Interfaces"]
        Persistencia[("PostgreSQL<br/>[EF Core 8 / Npgsql]<br/>Estrategia activa (JSON disponible como respaldo)")]
    end

    Dueno -->|"HTTPS (navegador)"| Presentation
    ClienteExt -->|"HTTPS / JSON (REST)"| Api

    Presentation -->|"usa"| Application
    Api -->|"usa"| Application
    Presentation -.->|"inyecta implementación (DI)"| Infrastructure
    Api -.->|"inyecta implementación (DI)"| Infrastructure

    Application -->|"depende de interfaces de"| Domain
    Infrastructure -->|"implementa interfaces de"| Domain
    Infrastructure -->|"lee / escribe"| Persistencia

    style Presentation fill:#1168bd,stroke:#0b4884,color:#fff
    style Api fill:#1168bd,stroke:#0b4884,color:#fff
    style Application fill:#1168bd,stroke:#0b4884,color:#fff
    style Infrastructure fill:#1168bd,stroke:#0b4884,color:#fff
    style Domain fill:#1168bd,stroke:#0b4884,color:#fff
    style Persistencia fill:#438dd5,stroke:#2e6295,color:#fff
```

**Para quién es:** el equipo técnico (desarrolladores, arquitectos) que necesita entender cómo se despliega y comunica el sistema.

**¿Cuáles son las piezas técnicas grandes del sistema (los 5 proyectos de la solución + persistencia) y cómo se comunican entre sí?** Aquí ya aparece la regla de dependencia de Clean Architecture: `Domain` no conoce a nadie, y tanto `Presentation` como `Api` solo hablan con `Application` (y resuelven `Infrastructure` por inyección de dependencias, nunca de forma directa).

> **Actualización (ADR-08):** la Deuda técnica 1 del ADR-06 quedó pagada — el patrón Strategy sobre `IDbContext` ahora sí selecciona un motor distinto según `UseJsonPersistence`. La estrategia activa en desarrollo y en el despliegue es **PostgreSQL** (vía EF Core / Npgsql), y `JsonDbContext` queda disponible como estrategia alternativa sin motor de base de datos.