# C4 Nivel 3 — Componentes — dentro de LocalManager.Api

```mermaid
graph TD
    subgraph Api["LocalManager.Api (ASP.NET Core Web API + Swagger)"]
        subgraph Controllers["Controllers REST"]
            CategoriasApi["CategoriasApiController"]
            ProductosApi["ProductosApiController"]
            ClientesApi["ClientesApiController"]
            VentasApi["VentasApiController"]
            CajaApi["CajaApiController"]
            ReportesApi["ReportesApiController"]
        end
        Models["Models (DTOs)<br/>ApiResponse, CrearVentaRequest"]
    end

    Application[("LocalManager.Application<br/>Services")]

    CategoriasApi -->|"usa"| Application
    ProductosApi -->|"usa"| Application
    ClientesApi -->|"usa"| Application
    VentasApi -->|"usa"| Application
    VentasApi -->|"recibe/devuelve"| Models
    CajaApi -->|"usa"| Application
    ReportesApi -->|"usa"| Application

    Controllers -->|"responde con"| Models

    style Api fill:#f5f5f5,stroke:#999
```

**Para quién es:** desarrolladores que consumen o extienden la API REST (equipos de integración, apps móviles futuras).

**¿Qué expone la API y cómo da servicio a sistemas externos?** Cada `*ApiController` mapea una entidad de negocio, delega en `LocalManager.Application` y devuelve respuestas estandarizadas mediante los DTOs de `Models` (`ApiResponse`, `CrearVentaRequest`), documentadas automáticamente por Swagger/OpenAPI.