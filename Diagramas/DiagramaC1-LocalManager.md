# C4 Nivel 1 — Contexto — LocalManager

```mermaid
graph TD
    Dueno["👤 Dueño del negocio<br/>Persona"]
    ClienteExterno["👤 Sistema externo<br/>Cliente de la API"]
    Sistema["LocalManager<br/>Sistema de gestión de negocios locales<br/>(ventas, inventario, clientes, caja)"]

    Dueno -->|"gestiona ventas, productos,<br/>clientes y caja desde el navegador"| Sistema
    ClienteExterno -->|"consume datos vía<br/>API REST (JSON)"| Sistema

    style Sistema fill:#1168bd,stroke:#0b4884,color:#fff
    style Dueno fill:#08427b,stroke:#052e56,color:#fff
    style ClienteExterno fill:#08427b,stroke:#052e56,color:#fff
```

**Para quién es:** cualquier persona, técnica o no técnica (stakeholders, negocio, nuevos integrantes del equipo).

**¿Quién usa el sistema y qué hace el sistema, en términos simples?** Nadie en esta vista necesita saber que existe ASP.NET Core, Clean Architecture, ni los patrones GOF (Repository/Strategy) usados internamente.