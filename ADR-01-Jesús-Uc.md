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

---

## Decisión

Decidí implementar el patrón arquitectónico **Model-View-Controller (MVC)** utilizando **ASP.NET Core** como base para el desarrollo y despliegue de la aplicación, con **SQL Server** como base de datos relacional y **Entity Framework Core** para el manejo de los datos.

### ¿Por qué?

Se eligió esta combinación por las siguientes razones:
 
- **MVC como patrón base:** Me permite dividir el sistema en tres responsabilidades claras: los Modelos encapsulan los datos y las reglas del negocio (productos, ventas, inventario), los Controladores reciben las acciones del usuario y coordinan la respuesta, y las Vistas se encargan únicamente de mostrar la información. Esta separación evita que un cambio en la interfaz rompa accidentalmente la lógica de una venta o un movimiento de inventario.
- **ASP.NET Core:** Provee una infraestructura completa para construir aplicaciones web con C# sin necesidad de configurar herramientas adicionales. La plantilla MVC de Visual Studio ya incluye el enrutamiento, la gestión de peticiones HTTP y el sistema de vistas con Razor, lo que reduce el tiempo de configuración inicial y permite enfocarse directamente en la lógica del negocio.
- **Entity Framework Core:** Simplifica enormemente el acceso a la base de datos al permitir trabajar con las entidades del sistema (Producto, Venta, Cliente, etc.) directamente como objetos de C#, sin escribir SQL manual para cada operación. Esto agiliza el desarrollo de los módulos CRUD que forman la base del sistema.
- **SQL Server:** Al tratarse de un sistema con datos financieros y relaciones complejas entre tablas, una base de datos relacional con soporte a transacciones es la opción más adecuada. Garantiza que operaciones como registrar una venta y descontar el stock ocurran de forma completa o no ocurran, evitando inconsistencias que podrían afectar al negocio.
