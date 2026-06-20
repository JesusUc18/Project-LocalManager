# Local Manager — Versión Monolito

> **Estado:** `Propuesto` | **Autor:** Jesús Uc | **Fecha:** 15/05/2026

Aplicación web monolítica para la gestión de negocios locales pequeños y medianos. Controla ventas, inventario, clientes y caja sin depender de libretas o Excel.

---

## Arquitectura

```
LocalManager/
├── LocalManager.sln
└── LocalManager/
    ├── Controllers/     ← Controladores MVC
    ├── Models/          ← Entidades de negocio
    ├── Services/        ← Acceso a datos JSON
    ├── Views/           ← Vistas Razor
    └── Data/            ← Archivos JSON generados
```

Patrón **Model-View-Controller (MVC)** con persistencia temporal en archivos JSON.

---

## Capturas de pantalla

### DASHBOARD (panel general):
<img width="1899" height="473" alt="image" src="https://github.com/user-attachments/assets/67d1def1-8430-4b5b-b252-7ec0e68a4d7c" />


### VENTAS

Historial:
<img width="1904" height="454" alt="image" src="https://github.com/user-attachments/assets/852811f1-8c75-4593-89de-49e6b2cb4dea" />

Nueva Venta:
<img width="1904" height="594" alt="image" src="https://github.com/user-attachments/assets/6c30f0c1-0501-48d2-92f3-5f06f359b4d9" />

Registro de venta:
<img width="1904" height="466" alt="image" src="https://github.com/user-attachments/assets/0752e528-0957-4b7c-bb68-7e2151836f7e" />


### PRODUCTOS

Listado de Productos:
<img width="1902" height="468" alt="image" src="https://github.com/user-attachments/assets/9a32d46b-6b9b-4014-aae4-53a1a899a2e7" />

Nuevo Producto:
<img width="1904" height="678" alt="image" src="https://github.com/user-attachments/assets/98dac934-1692-4a2d-88c0-c92ce6e7e800" />

Registro de Producto:
<img width="1902" height="490" alt="image" src="https://github.com/user-attachments/assets/2de20286-7b56-4d1a-879b-fb179c0b4b28" />


### CATEGORÍAS

Listado:
<img width="1903" height="459" alt="image" src="https://github.com/user-attachments/assets/f683da23-39d0-4e72-81bd-f3243ff868cc" />

Agregar Categoría:
<img width="1903" height="455" alt="image" src="https://github.com/user-attachments/assets/d282ee4f-8d4b-4430-aa00-083af90729f9" />

Editar Categoría:
<img width="1899" height="460" alt="image" src="https://github.com/user-attachments/assets/1234168e-97c1-4fa7-922a-71c53d00e048" />


### CLIENTES

Listado:
<img width="1904" height="447" alt="image" src="https://github.com/user-attachments/assets/ec4c5d5a-c420-4c0b-bc68-68543807cc95" />

Agregar Cliente:
<img width="1899" height="604" alt="image" src="https://github.com/user-attachments/assets/c41d883d-44f1-4291-b14e-aee0b0ff7f0d" />

Editar Cliente:
<img width="1902" height="612" alt="image" src="https://github.com/user-attachments/assets/33410c7f-171e-4921-a7f4-ae0d86ac20a5" />


### CAJAS:

Control de Cajas:
<img width="1904" height="449" alt="image" src="https://github.com/user-attachments/assets/81d9e6d9-055e-4fd3-87a7-f635e57d788b" />

Abrir Caja:
<img width="1906" height="455" alt="image" src="https://github.com/user-attachments/assets/82129c96-8f22-4b13-bb12-3d25c6cc4029" />

Detalle de Caja:
<img width="1902" height="451" alt="image" src="https://github.com/user-attachments/assets/03129fce-a68f-47fa-8244-70017bcc59e6" />


### REPORTES
<img width="1901" height="556" alt="image" src="https://github.com/user-attachments/assets/b53d097b-30e6-472d-92f2-031187bc1865" />


---


## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Framework | ASP.NET Core 8 |
| Patrón | MVC |
| Base de datos | JSON (temporal) |
| Frontend | Razor + Bootstrap 5 |

---

## Módulos

- **Dashboard** — Resumen del negocio
- **Productos** — CRUD + stock + categoría
- **Categorías** — Clasificación de productos
- **Clientes** — Registro de clientes
- **Ventas** — Registro con múltiples productos
- **Caja** — Apertura/cierre de turnos
- **Reportes** — Ventas, stock bajo, resumen de cajas

---

## Ejecución

```bash
cd LocalManager
dotnet restore
dotnet run
```

Abre `https://localhost:5001` en tu navegador.

---

## Flujo de uso

1. Crear Categorías → 2. Agregar Productos → 3. Abrir Caja → 4. Registrar Ventas → 5. Cerrar Caja → 6. Consultar Reportes

---

## Base de datos

Los datos se almacenan en archivos `.json` dentro de `Data/`:

| Archivo | Entidad |
|---------|---------|
| `categorias.json` | Categorías |
| `productos.json` | Productos |
| `clientes.json` | Clientes |
| `ventas.json` | Ventas |
| `detalleventas.json` | Detalles de venta |
| `cajas.json` | Cajas |

---

## Autor

**Jesús Uc** — Proyecto de gestión de negocios locales.

---

## Estado

`Propuesto` — Prototipo funcional listo para pruebas.

---

## Uso de Inteligencia Artificial

Este documento fue redactado de forma personal. Sin embargo, se utilizó inteligencia artificial como herramienta de apoyo en los siguientes aspectos específicos:

| Área de uso | Descripción |
|-------------|-------------|
| **Selección de base de datos** | Se consultó IA para contrastar las características de SQL Server frente a otras opciones relacionales, con el fin de validar que la elección fuera coherente con los requerimientos del sistema. La decisión final fue tomada por el autor. |
| **Corrección de sintaxis Markdown** | Se empleó IA para revisar y corregir la sintaxis del documento en formato Markdown, garantizando que el formato de tablas, encabezados y bloques de código se renderice correctamente. |
| **Optimización de diagramas** | Se usó IA como apoyo para estructurar y mejorar la presentación visual de las vistas arquitectónicas, asegurando que reflejaran con claridad la arquitectura descrita. |

> **Nota:** El análisis de contexto, la toma de decisiones arquitectónicas, la redacción del razonamiento y la definición de consecuencias son de autoría propia, la IA no generó ningún contenido de fondo de este ADR de forma autónoma.

**ESTE README ES TEMPORAL**
