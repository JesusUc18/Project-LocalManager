# ADR-07: Suite de Pruebas Automatizadas y Pipeline de Integración Continua

| Campo  | Valor |
|--------|-------|
| Autor  | Jesús Uc |
| Fecha  | 22/07/2026 |
| Estado | `APROBADO` |

---

## Contexto

Hasta el ADR-06, **Local Manager** contaba con arquitectura en capas, patrones GOF (Repository, Strategy) y una API REST, pero ninguna prueba automatizada verificaba que ese comportamiento se mantuviera al hacer cambios. Cualquier regresión en un controlador solo se detectaba probando la aplicación manualmente.

Este ADR documenta la incorporación de una suite de pruebas con **xUnit** y de un **pipeline de Integración Continua** en GitHub Actions que compila la solución y ejecuta esa suite en cada `push`, siguiendo la misma disciplina ya aplicada al proyecto grupal `CitasApp`.

---

## Qué se probó y por qué se eligieron esas clases

Se creó el proyecto `LocalManager.xUnit` con pruebas para **tres controladores de la capa Presentation**, siguiendo el patrón Arrange-Act-Assert:

| Clase probada | Por qué se eligió |
|----------------|--------------------|
| `CategoriasController` | Es el controlador CRUD más simple del módulo de Inventario y depende de una sola interfaz (`ICategoriaService`), ideal para validar rápido que el patrón de pruebas con fakes funciona correctamente. |
| `ClientesController` | Representa un módulo independiente (Clientes) y permite probar tanto lectura (`Index`, `Edit`) como un flujo de escritura (`DeleteConfirmed`), ampliando la cobertura a operaciones que modifican estado. |
| `CajaController` | Es el controlador con más reglas de negocio relevantes (apertura/cierre de turno), y sus acciones (`Abrir`, `Cerrar`) son las que sostienen el módulo de Ventas; probarlo reduce el riesgo de descuadres de caja mencionados como deuda técnica en el ADR-06. |

Cada controlador se probó usando **fakes en memoria** que implementan directamente las interfaces de `Application` (`ICategoriaService`, `IClienteService`, `ICajaService`), en lugar de un framework de mocking, para mantener consistencia con el enfoque ya usado en `CitasApp` y evitar una dependencia adicional. Esto también valida indirectamente que la Arquitectura en Capas (ADR-03) se respeta: los controladores de `Presentation` solo dependen de interfaces de `Application`, por lo que pueden probarse sin levantar `Infrastructure` ni una base de datos real.

No se probó la capa `Infrastructure` (repositorios EF/JSON) en este ADR porque requeriría una base de datos o un sistema de archivos real; queda como trabajo futuro con pruebas de integración separadas.

---

## Pipeline de Integración Continua

Se agregó `.github/workflows/ci.yml`, que en cada `push` o `pull request`:

1. Descarga el código (`actions/checkout`).
2. Instala el SDK de .NET 8.
3. Restaura dependencias (`dotnet restore`) sobre `LocalManager.sln`.
4. Compila en modo `Release` (`dotnet build`).
5. Ejecuta la suite de `LocalManager.xUnit` (`dotnet test`).

Si cualquiera de los tres controladores probados deja de comportarse como se espera, el pipeline falla y el error se detecta antes de llegar a `main`, en vez de descubrirse manualmente.

---

## Consecuencias

**✅ Lo que se gana:**

- Cambios futuros en `CategoriasController`, `ClientesController` o `CajaController` quedan protegidos por pruebas automáticas.
- El pipeline documenta, de forma ejecutable, que la solución compila y pasa pruebas en un entorno limpio (no solo en la máquina del autor), lo que además ayuda a detectar a futuro problemas como la ruta absoluta descrita en la Deuda técnica 1 del ADR-06.
- El patrón de fakes usado aquí es reutilizable para probar los controladores restantes (`ProductosController`, `VentasController`, `ReportesController`) sin necesidad de introducir un framework de mocking.

**⚠️ Lo que se asume mientras no se amplíe la cobertura:**

- `ProductosController`, `VentasController`, `ReportesController` y toda la capa `Infrastructure`/`Api` siguen sin pruebas automatizadas; una regresión ahí no sería detectada por este pipeline todavía.

---

## 🤖 Uso de Inteligencia Artificial

Este documento fue redactado de forma personal. Se utilizó inteligencia artificial como herramienta de apoyo en los siguientes aspectos específicos:

| Área de uso | Descripción |
|-------------|-------------|
| **Diseño de la suite de pruebas** | Se usó IA para proponer la estructura de los fakes en memoria y los casos de prueba (Arrange-Act-Assert) para `CategoriasController`, `ClientesController` y `CajaController`, tomando como referencia el patrón ya aplicado en `CitasApp.xUnit`. |
| **Redacción y estructura del documento** | Se empleó IA para organizar este ADR en el mismo formato usado en los ADRs anteriores y revisar la sintaxis Markdown. |

> **Nota:** La elección de qué controladores probar y la validación de que los casos de prueba reflejan el comportamiento real de la aplicación son de autoría propia. La IA no ejecutó las pruebas ni verificó el pipeline, su rol fue de apoyo en el diseño y la redacción.
