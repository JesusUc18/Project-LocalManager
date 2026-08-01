# C4 Nivel 3 — Componentes — dentro de LocalManager.Domain

```mermaid
graph TD
    subgraph Domain["LocalManager.Domain"]
        subgraph Entities["Entities"]
            Categoria["Categoria"]
            Producto["Producto"]
            Cliente["Cliente"]
            Venta["Venta"]
            DetalleVenta["DetalleVenta"]
            Caja["Caja"]
        end
        subgraph Interfaces["Interfaces/Repositories «Repository (GOF Estructural)»"]
            ICategoriaRepository["ICategoriaRepository"]
            IProductoRepository["IProductoRepository"]
            IClienteRepository["IClienteRepository"]
            IVentaRepository["IVentaRepository"]
            ICajaRepository["ICajaRepository"]
        end
    end

    ICategoriaRepository -->|"opera sobre"| Categoria
    IProductoRepository -->|"opera sobre"| Producto
    IClienteRepository -->|"opera sobre"| Cliente
    IVentaRepository -->|"opera sobre"| Venta
    IVentaRepository -->|"opera sobre"| DetalleVenta
    ICajaRepository -->|"opera sobre"| Caja

    style Domain fill:#f5f5f5,stroke:#999
```

**Para quién es:** desarrolladores que van a modificar entidades o contratos de acceso a datos.

**¿Qué hay dentro del centro de la arquitectura?** `Domain` es el proyecto que no depende de ningún otro (ni `Infrastructure`, ni `Presentation`, ni `Api`) y define, en las interfaces `I*Repository`, el contrato del patrón **Repository** (GOF, Estructural) que `Infrastructure` implementará más adelante.