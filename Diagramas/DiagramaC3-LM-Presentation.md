# C4 Nivel 3 — Componentes — dentro de LocalManager.Presentation

```mermaid
graph TD
    subgraph Presentation["LocalManager.Presentation (ASP.NET Core MVC)"]
        subgraph Controllers["Controllers"]
            HomeController["HomeController"]
            CategoriasController["CategoriasController"]
            ProductosController["ProductosController"]
            ClientesController["ClientesController"]
            VentasController["VentasController"]
            CajaController["CajaController"]
            ReportesController["ReportesController"]
        end
        ViewModels["ViewModels<br/>VentaViewModel"]
        Views["Views (Razor)<br/>Categorias, Productos, Clientes,<br/>Ventas, Caja, Reportes, Home"]
    end

    Application[("LocalManager.Application<br/>Services")]

    CategoriasController -->|"usa"| Application
    ProductosController -->|"usa"| Application
    ClientesController -->|"usa"| Application
    VentasController -->|"usa"| Application
    VentasController -->|"construye"| ViewModels
    CajaController -->|"usa"| Application
    ReportesController -->|"usa"| Application

    Controllers -->|"renderiza"| Views

    style Presentation fill:#f5f5f5,stroke:#999
```

**Para quién es:** desarrolladores frontend/MVC que trabajan en las vistas Razor y su interacción con el dueño del negocio.

**¿Cómo está organizado el sitio web que usa el dueño del negocio?** Cada `Controller` (uno por entidad de negocio) solo conoce `LocalManager.Application` — nunca accede directamente a `Infrastructure` ni a los archivos de persistencia — y delega en `Application` toda la lógica antes de renderizar las vistas Razor.