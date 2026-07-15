# ADR-04: Incorporación de API REST

| Campo  | Valor |
|--------|-------|
| Autor  | Jesús Uc |
| Fecha  | 19/06/2026 |
| Estado | `Actualizado por el ADR-05` |

---

## Contexto

En el ADR-03 se adoptó una **Arquitectura en Capas (Clean Architecture)** para el proyecto **Local Manager**, organizando la solución en cuatro proyectos: `Domain`, `Application`, `Infrastructure` y `Presentation` (ASP.NET Core MVC). Esta arquitectura permitió aislar la lógica de negocio de la capa de presentación y preparó el terreno para evoluciones futuras.

Sin embargo, la actividad actual del proyecto exige incorporar una **API REST** que exponga los endpoints del sistema de forma profesional, documentada y consumible por otros clientes (incluyendo una futura aplicación móvil). El sistema actual solo cuenta con la interfaz MVC (Razor), que genera HTML completo en el servidor y no permite que un cliente externo (como una app móvil o un frontend JavaScript) consuma los datos de forma programática.

Restricciones que se mantienen del ADR-03:

- **Arquitectura en Capas:** La API debe respetar la regla de dependencia hacia el centro (`Domain` ← `Application` ← `Infrastructure` / `Presentation` / `Api`).
- **Tecnología:** C# / .NET, ASP.NET Core, EF Core, SQL Server (sin cambios).
- **Reutilización:** La API debe reutilizar `Domain` y `Application` sin modificarlos, demostrando que la arquitectura en capas cumple su propósito.

---

## Decisión

Se incorpora un nuevo proyecto **ASP.NET Core Web API** (`LocalManager.Api`) a la solución existente, exponiendo endpoints REST para todos los módulos del sistema (Productos, Categorías, Clientes, Ventas, Caja y Reportes). La API se documenta con **Swagger / OpenAPI**, estándar de la industria para documentar APIs.

La solución `LocalManager.sln` pasa a tener **cinco proyectos**:

- **LocalManager.Domain** — Entidades e interfaces de repositorio (sin cambios).
- **LocalManager.Application** — Servicios de negocio (sin cambios).
- **LocalManager.Infrastructure** — Repositorios y `DbContext` (sin cambios).
- **LocalManager.Presentation** — ASP.NET Core MVC existente (sin cambios).
- **LocalManager.Api** — Nuevo proyecto ASP.NET Core Web API + Swagger.

### ¿Por qué?

- **Resuelve el problema de interoperabilidad:** La interfaz MVC genera HTML completo en el servidor, lo que solo sirve para navegadores. Una API REST devuelve JSON puro, que puede ser consumido por cualquier cliente: navegadores con JavaScript, aplicaciones móviles, integraciones con otros sistemas, o incluso la propia interfaz MVC si en el futuro se decide migrar a un frontend SPA.
- **Reutilización total de la lógica de negocio:** Gracias a la Arquitectura en Capas (ADR-03), `LocalManager.Api` depende de `Application` y `Domain` exactamente igual que `LocalManager.Presentation`. Los servicios (`VentaService`, `ProductoService`, etc.) y las entidades se reutilizan sin modificar una sola línea, demostrando que el aislamiento del dominio funciona.
- **Swagger como estándar de documentación:** Swagger UI genera automáticamente una interfaz interactiva donde se pueden probar todos los endpoints sin necesidad de herramientas externas. Esto es el estándar de la industria para documentar APIs y es lo que se revisará para validar los endpoints.
- **Preparación para la app móvil:** El ADR-02 y ADR-03 identificaron que una futura app móvil requeriría refactorizar el backend a una Web API. Con esta decisión, ese camino ya está abierto: la app móvil solo necesitará consumir los endpoints existentes.
- **Coexistencia MVC + API:** No es necesario eliminar la interfaz MVC. Ambos proyectos (`Presentation` y `Api`) coexisten en la misma solución, compartiendo las mismas capas de negocio y datos. Esto permite una transición gradual si en el futuro se decide migrar completamente a una arquitectura API-first.

### Endpoints implementados

| Módulo | Endpoint | Método | Descripción |
|--------|----------|--------|-------------|
| **Productos** | `/api/productos` | GET | Lista todos los productos |
| | `/api/productos/{id}` | GET | Obtiene un producto por ID |
| | `/api/productos` | POST | Crea un nuevo producto |
| | `/api/productos/{id}` | PUT | Actualiza un producto |
| | `/api/productos/{id}` | DELETE | Elimina un producto |
| **Categorías** | `/api/categorias` | GET | Lista todas las categorías |
| | `/api/categorias/{id}` | GET | Obtiene una categoría por ID |
| | `/api/categorias` | POST | Crea una nueva categoría |
| | `/api/categorias/{id}` | PUT | Actualiza una categoría |
| | `/api/categorias/{id}` | DELETE | Elimina una categoría |
| **Clientes** | `/api/clientes` | GET | Lista todos los clientes |
| | `/api/clientes/{id}` | GET | Obtiene un cliente por ID |
| | `/api/clientes` | POST | Crea un nuevo cliente |
| | `/api/clientes/{id}` | PUT | Actualiza un cliente |
| | `/api/clientes/{id}` | DELETE | Elimina un cliente |
| **Ventas** | `/api/ventas` | GET | Lista todas las ventas |
| | `/api/ventas/{id}` | GET | Obtiene una venta por ID |
| | `/api/ventas` | POST | Registra una nueva venta (transaccional) |
| | `/api/ventas/caja/{cajaId}` | GET | Ventas de una caja específica |
| | `/api/ventas/fecha/{fecha}` | GET | Ventas de una fecha específica |
| **Caja** | `/api/caja` | GET | Lista todas las cajas |
| | `/api/caja/{id}` | GET | Obtiene una caja por ID |
| | `/api/caja/abiertas` | GET | Cajas actualmente abiertas |
| | `/api/caja/abrir` | POST | Abre un nuevo turno de caja |
| | `/api/caja/{id}/cerrar` | POST | Cierra un turno de caja |
| **Reportes** | `/api/reportes/dashboard` | GET | KPIs del negocio |
| | `/api/reportes/ventas-hoy` | GET | Ventas del día |
| | `/api/reportes/ventas-mes` | GET | Ventas del mes |
| | `/api/reportes/stock-bajo` | GET | Productos con stock bajo |
| | `/api/reportes/resumen-cajas` | GET | Resumen de todas las cajas |

---

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **gRPC** | Aunque es más rápido en rendimiento, gRPC requiere generar archivos `.proto` y clientes específicos. Para un sistema de gestión de negocios locales donde los clientes serán navegadores y apps móviles, REST es más universal y fácil de consumir. gRPC brilla en comunicación servicio-a-servicio, no en API pública. |
| **GraphQL** | Permite que el cliente solicite exactamente los campos que necesita, pero introduce complejidad adicional: esquemas, resolvers, y una curva de aprendizaje que no justifica su adopción en un proyecto de 3 meses con un solo desarrollador. Además, la actividad específica pide REST. |
| **Mantener solo MVC sin API** | Ya descartada en el contexto: la actividad exige explícitamente incorporar una API REST. Además, sin API no hay forma de que una app móvil o un frontend SPA consuma los datos del sistema. |
| **Crear una API separada que duplique la lógica de negocio** | Violaba directamente el ADR-03. Duplicar servicios y repositorios en un proyecto aparte generaría deuda técnica inmediata: cualquier cambio en las reglas de negocio (como el cálculo de stock o el cierre de caja) tendría que hacerse en dos lugares. La arquitectura en capas evita esto. |

---

## Consecuencias

**✅ Lo que gano:**

- **Interoperabilidad:** Cualquier cliente (navegador con JavaScript, app móvil, Postman, integración externa) puede consumir los datos del sistema vía JSON.
- **Documentación profesional:** Swagger UI genera automáticamente una página interactiva donde se prueban todos los endpoints sin herramientas externas. Esto es el estándar de la industria y lo que se revisará para validar la entrega.
- **Reutilización comprobada:** `Domain` y `Application` se usan exactamente igual desde `Presentation` (MVC) y desde `Api` (REST). Esto valida que la Arquitectura en Capas (ADR-03) funciona como se diseñó.
- **Camino abierto a la app móvil:** El ADR-02 y ADR-03 mencionaban que una app móvil requeriría refactorizar el backend. Ahora esa refactorización ya está hecha: la app móvil solo necesita consumir los endpoints existentes.
- **Coexistencia sin fricción:** MVC y API comparten las mismas capas de negocio y datos. No hay conflictos ni duplicación de código.

**⚠️ Lo que sacrifico o asumo:**

- **Mayor complejidad de la solución:** Pasar de 4 a 5 proyectos implica más archivos y más configuración de inyección de dependencias. Sin embargo, cada proyecto tiene una responsabilidad clara.
- **Doble mantenimiento de presentación:** Si se modifica una regla de negocio, solo se toca `Application` (una vez). Pero si se agrega un nuevo campo a una entidad, puede requerir actualizar tanto la vista Razor (MVC) como el DTO de la API. Esto se mitigará en el futuro si se decide deprecar MVC y usar solo la API con un frontend SPA.
- **Seguridad:** La API expone endpoints públicos. En producción se debería agregar autenticación (JWT, API Keys) y autorización, lo que no está incluido en esta entrega por limitaciones de tiempo.

---

## Diagrama de Arquitectura con API

```
┌─────────────────┐     ┌─────────────────┐
│   Navegador     │     │   App Móvil     │
│   (MVC Razor)   │     │   (fetch/axios) │
└────────┬────────┘     └────────┬────────┘
         │                       │
         ▼                       ▼
┌───────────────┐         ┌─────────────────┐
│  Presentation │         │      Api        │
│   (MVC)       │         │   (REST +       │
│               │         │    Swagger)     │
└────────┬──────┘         └────────┬────────┘
         │                         │
         └──────────┬──────────────┘
                    │
                    ▼
         ┌─────────────────┐
         │   Application   │  ← Servicios de negocio
         │   (IService)    │     (VentaService, etc.)
         └────────┬────────┘
                  │
                  ▼
         ┌─────────────────┐
         │     Domain      │  ← Entidades + IRepository
         │                 │     (Producto, Venta, etc.)
         └─────────────────┘
                  ▲
         ┌────────┴────────┐
         │  Infrastructure │  ← Repositorios + DbContext
         │                 │     (JSON temporal / EF Core)
         └─────────────────┘
```

---

## 🤖 Uso de Inteligencia Artificial

Este documento fue redactado de forma personal. Se utilizó inteligencia artificial como herramienta de apoyo en los siguientes aspectos específicos:

| Área de uso | Descripción |
|-------------|-------------|
| **Comparación de tecnologías API** | Se consultó IA para contrastar REST vs gRPC vs GraphQL, validando que REST fuera la opción más adecuada para el contexto del proyecto (universalidad, facilidad de consumo, estándar de la industria). La decisión final fue tomada por el autor. |
| **Corrección de sintaxis Markdown** | Se empleó IA para revisar la sintaxis del documento, asegurando el correcto renderizado de tablas, listas y bloques de código. |
| **Estructuración del diagrama** | Se usó IA como apoyo para organizar la representación visual de la arquitectura con los 5 proyectos y el flujo de dependencias. |

> **Nota:** El análisis de contexto, la justificación de la decisión arquitectónica, la definición de endpoints, la evaluación de alternativas y la definición de consecuencias son de autoría propia. La IA no generó contenido de fondo de este ADR de forma autónoma.
