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



**ESTE README ES TEMPORAL**