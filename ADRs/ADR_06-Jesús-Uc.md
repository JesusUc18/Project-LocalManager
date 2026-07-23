# ADR-06: Deuda Técnica Identificada en Local Manager

| Campo  | Valor |
|--------|-------|
| Autor  | Jesús Uc |
| Fecha  | 15/07/2026 |
| Estado | `Actualizado por el ADR-07` |

---

## Contexto

Los ADRs anteriores (01 a 05) documentaron la arquitectura de **Local Manager**: MVC + Clean Architecture en cuatro/cinco proyectos, la incorporación de una API REST, y los patrones GOF **Repository** y **Strategy** aplicados a la capa de persistencia.

Durante esa evolución, al ser un desarrollador único trabajando bajo un plazo de tres meses, se tomaron decisiones que priorizaron avanzar rápido sobre dejar el código en su forma final. Este ADR tiene como objetivo **reconocer explícitamente** esa deuda técnica —en lugar de dejarla oculta en el código— y proponer una solución concreta para cada caso, siguiendo la misma disciplina de documentación usada en el resto del proyecto.

Se identificaron dos deudas técnicas: una de **configuración/infraestructura** y otra de **lógica de negocio**, ambas dentro de la capa `Infrastructure` y su consumo desde `Application`/`Api`.

---

## Deuda técnica 1: Ruta absoluta de máquina local escrita a mano en `appsettings.json` (Configuración)

### Qué es

En `LocalManager.Api/appsettings.json`, la clave `JsonDatabase:DataPath` no apunta a una carpeta relativa dentro del proyecto (como sí ocurre en `LocalManager.Presentation/appsettings.json`, que usa `"Data"`), sino a una ruta absoluta de Windows específica de la máquina del autor:

```json
"JsonDatabase": {
  "DataPath": "C:\\Users\\Dell\\source\\repos\\Project-LocalManager\\LocalManager.Presentation\\Data\\"
}
```

Además, en `Program.cs` de ambos proyectos (`Presentation` y `Api`) existe un `if/else` sobre la bandera `UseJsonPersistence` en el que **ambas ramas hacen exactamente lo mismo** (registrar `JsonDbContext`), con un comentario que dice "descomentar cuando se implemente `SqlDbContext`". Es decir, la bandera de configuración que el ADR-05 presenta como el "selector de estrategia" (Strategy) en realidad no selecciona nada todavía: da igual el valor que tenga, el sistema siempre usa JSON.

### Por qué existe

Es una mezcla de decisión consciente y descuido:

- **Decisión consciente:** mientras se desarrollaba `LocalManager.Api` en una máquina distinta a `LocalManager.Presentation`, se apuntó manualmente la ruta de datos del API a la carpeta `Data` del proyecto `Presentation` para que ambos proyectos compartieran los mismos archivos JSON durante las pruebas locales, en vez de invertir tiempo en configurar una ruta relativa correcta entre proyectos hermanos de la solución.
- **Descuido no detectado a tiempo:** esa ruta absoluta, atada a la carpeta personal `C:\Users\Dell\...`, se quedó en el archivo que sí se sube al repositorio (`appsettings.json`, sin `.gitignore`), en lugar de moverse a `appsettings.Development.json` o a un mecanismo de configuración por entorno. El `if/else` sin implementar en `Program.cs` también quedó así por avanzar con otras entregas (ADR-04, ADR-05) sin volver a cerrar ese pendiente.

### Costo de no pagarla

- El proyecto **no corre en ninguna otra máquina** tal cual: al clonar el repositorio en otra computadora (o en el servidor de despliegue), `LocalManager.Api` intentará leer/escribir en `C:\Users\Dell\...`, una ruta que no existe fuera de la máquina original, y la aplicación fallará al iniciar o guardará datos en una carpeta vacía sin avisar con claridad la causa.
- Si un compañero de equipo o el propio evaluador clona el repositorio, pierde tiempo diagnosticando un error que no tiene relación con la lógica del sistema, solo con una ruta hardcodeada.
- La bandera `UseJsonPersistence`, documentada en el ADR-05 como el mecanismo para migrar a SQL Server sin tocar código, en la práctica **no cumple su propósito**: si mañana se cambia a `false` esperando activar SQL Server, el sistema seguirá usando JSON silenciosamente, generando una falsa sensación de que la migración ya está lista cuando no lo está.
- Cuantos más módulos se agreguen leyendo esta misma configuración, más lugares habrá que corregir a mano el día que se detecte el problema, en vez de corregirlo en un solo punto.

### Propuesta de solución

- **Externalizar la ruta usando variables de entorno / configuración por ambiente:** mover `JsonDatabase:DataPath` a `appsettings.Development.json` (ignorado o específico por desarrollador) y dejar en `appsettings.json` un valor relativo por defecto (`"Data"`), tal como ya hace `Presentation`. Alternativamente, leer la ruta desde una variable de entorno (`LOCALMANAGER_DATA_PATH`) con `builder.Configuration.AddEnvironmentVariables()`, cayendo en `"Data"` si no está definida.
- **Técnica de refactorización:** *Replace Hard-Coded Value with Configuration Parameter* (extraer valor embebido a configuración externa), combinada con **Substitute Algorithm** en el bloque `if (usarJson) {...} else {...}` de `Program.cs`, dejando una única rama de registro de `IDbContext` mientras `SqlDbContext` no exista, o lanzando una excepción clara (`NotImplementedException`) en la rama `else` en lugar de simular que hace algo distinto. Esto deja el código honesto respecto a lo que realmente hace, hasta que se implemente la estrategia SQL real.

---

## Deuda técnica 2: "Transacción atómica" de `VentaService.Registrar` que no es realmente atómica (Lógica de negocio)

### Qué es

El ADR-01 y el ADR-05 describen el registro de una venta como una **operación atómica**: "la venta y el descuento de stock ocurren juntos o no ocurren". Sin embargo, revisando `VentaService.Registrar` junto con `ProductoRepository` y `VentaRepository`, cada operación individual (`_ventaRepository.Agregar`, `_ventaRepository.AgregarDetalle`, `_productoRepository.Actualizar`) llama a `_context.SaveChanges()` **inmediatamente**, lo que reescribe los archivos JSON en disco en ese mismo instante, no al final de la operación completa.

Esto significa que si el proceso se interrumpe (excepción no controlada, caída del servicio, corte de energía) **entre** el registro de la venta y el descuento del stock de todos sus productos, los archivos ya quedaron escritos en disco de forma parcial: la venta existe, pero el stock de algunos productos sí se descontó y el de otros no. El "rollback manual" que existe en el código (`foreach` que revierte `productosDescontados`) solo cubre el caso en que la excepción ocurra *dentro* del propio método y haya tiempo de ejecutar ese bloque; no protege contra una interrupción del proceso a la mitad de una de esas llamadas a `SaveChanges()`.

### Por qué existe

Fue una decisión consciente para cumplir con el requisito del ADR-01 ("operaciones financieras consistentes") dentro del tiempo disponible: se implementó una simulación de transacción en memoria (guardar los IDs ya descontados y revertirlos manualmente si algo falla) porque `JsonDbContext` no soporta transacciones reales como sí lo haría `SqlDbContext` con Entity Framework Core (`BeginTransaction`/`Commit`/`Rollback`). Se priorizó tener *algo* que se comportara como atómico para las pruebas funcionales, dejando pendiente la garantía real de atomicidad para cuando se migre a SQL Server (tal como ya se anticipa en el ADR-05).

### Costo de no pagarla

- Es la parte del sistema que maneja **dinero e inventario**, exactamente el escenario que el ADR-01 identificó como el más sensible a inconsistencias. Un descuadre entre ventas registradas y stock real es el tipo de error que un negocio real notaría de inmediato (vender un producto que el sistema todavía cree que existe en stock, o bloquear ventas de un producto que en realidad sí hay).
- El costo crece con el uso: cada venta adicional es una nueva oportunidad de que una interrupción deje los archivos JSON inconsistentes, y al no haber una forma automática de detectar ese desfase, el error se acumula de forma silenciosa hasta que alguien nota que el reporte de stock no cuadra con la caja.
- Mientras el sistema siga usándose como base para el ADR-04 (API REST) y una futura app móvil, más clientes concurrentes podrán disparar ventas al mismo tiempo, aumentando la probabilidad de que dos operaciones se crucen exactamente en el punto vulnerable.

### Propuesta de solución

- **Corto plazo (sin cambiar de motor de datos):** aplicar la técnica de refactorización *Extract Method* + *Introduce Transaction Script* para mover toda la escritura a disco al final de la operación: que `Agregar`, `AgregarDetalle` y `Actualizar` dejen de llamar a `SaveChanges()` por su cuenta, y que `VentaService.Registrar` sea quien decida cuándo confirmar todos los cambios juntos con una sola llamada a `SaveChanges()` al final (patrón *Unit of Work*, complementario al Repository ya usado). Así, si algo falla antes de ese punto, nada se escribió todavía en disco.
- **Mediano plazo (cuando se implemente `SqlDbContext`, ya previsto en el ADR-05):** sustituir el rollback manual por una transacción real de base de datos (`DbContext.Database.BeginTransaction()` de EF Core), aplicando la técnica *Replace Manual Compensation with Transaction Boundary*: envolver las operaciones de `Registrar` en una transacción real que haga `Commit` solo si todos los pasos tienen éxito, y `Rollback` automático ante cualquier excepción, eliminando la necesidad de revertir manualmente el stock producto por producto.

---

## Consecuencias

**✅ Lo que se gana al documentar y planear el pago de esta deuda:**

- Ambas correcciones son incrementales y no rompen la Arquitectura en Capas ni los patrones GOF definidos en el ADR-03 y ADR-05: la deuda 1 se resuelve en `Program.cs`/`appsettings.json`, y la deuda 2 se resuelve dentro de `Infrastructure`/`Application`, sin tocar `Domain` ni los controladores.
- Deja el sistema listo para que cualquier persona (compañero, evaluador, o el propio autor en otra máquina) pueda clonar el repositorio y ejecutarlo sin depender de una ruta personal.
- Reduce el riesgo real de inconsistencias de datos financieros antes de que el sistema tenga más usuarios o se migre a producción.

**⚠️ Lo que se asume mientras la deuda no se paga:**

- El proyecto sigue siendo funcional para efectos de desarrollo y demostración en la máquina original, por lo que el pago de esta deuda no es bloqueante para las entregas anteriores, pero sí debe resolverse antes de cualquier despliegue fuera del entorno local del autor.

---

## 🤖 Uso de Inteligencia Artificial

Este documento fue redactado de forma personal. Se utilizó inteligencia artificial como herramienta de apoyo en los siguientes aspectos específicos:

| Área de uso | Descripción |
|-------------|-------------|
| **Revisión de código para identificar deuda técnica** | Se usó IA para inspeccionar `appsettings.json`, `Program.cs`, `JsonDbContext.cs`, `ProductoRepository.cs` y `VentaService.cs` y ayudar a localizar las dos deudas documentadas, contrastándolas con lo declarado en los ADRs anteriores (ej. la "atomicidad" del ADR-01). |
| **Redacción y estructura del documento** | Se empleó IA para organizar el contenido en el formato Qué es / Por qué existe / Costo / Propuesta de solución, y para revisar la sintaxis Markdown. |
| **Nombramiento de técnicas de refactorización** | Se consultó IA para nombrar correctamente las técnicas de refactorización aplicables (Replace Hard-Coded Value with Configuration Parameter, Extract Method, Introduce Unit of Work, Substitute Algorithm). |

> **Nota:** La identificación de que estas deudas existían en el código real del proyecto, la decisión de cuáles documentar, y la evaluación de su costo e impacto en el negocio son de autoría propia. La IA no generó deuda técnica ficticia ni decidió cuáles problemas eran relevantes; su rol fue de apoyo en la inspección de código y en la redacción.