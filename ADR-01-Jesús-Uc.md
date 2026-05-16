# ADR-01: Estructura base del sistema

| Campo  | Valor |
|--------|-------|
| Autor  | Jesús Uc |
| Fecha  | 15/05/2026 |
| Estado | `Propuesto` |

---

## Contexto

Quiero construir una aplicación web llamada **Local Manager** para la gestión de negocios locales pequeños y medianos, orientada a dueños o administradores que necesitan controlar sus ventas, inventario, clientes y caja sin depender de libretas o archivos de Excel. El sistema debe permitir registrar ventas, controlar el stock de productos, administrar clientes y generar reportes básicos que ayuden a tomar mejores decisiones en el negocio.
 
Para el desarrollo de este proyecto consideré las siguientes restricciones:
 
- **Complejidad de los datos:** El sistema maneja entidades fuertemente relacionadas entre sí: una venta tiene múltiples productos, cada producto pertenece a una categoría y afecta el inventario, todo bajo una caja que corresponde a un turno. Esta estructura requiere un modelo de datos ordenado que garantice consistencia en cada operación.
- **Tecnologías conocidas:** Para no perder el tiempo en curvas de aprendizaje innecesarias durante el primer mes del proyecto, se priorizó trabajar con C# y el ecosistema de .NET, que son las tecnologías que ya manejo y que se vieron en clase, poco a poco iré evaluando proximas tecnologías para mejorar mi proyecto.

