# Evaluación ATAM — Local Manager

| Campo  | Valor |
|--------|-------|
| Autor  | Jesús Uc |
| Fecha  | 31/07/2026 |
| Basado en | ADR-01 a ADR-08 |

---

## 1. ¿Qué es ATAM y por qué se hace ahora?

**ATAM** (Architecture Tradeoff Analysis Method) es un método para evaluar una arquitectura ya definida, buscando específicamente tres cosas en las decisiones que ya se tomaron:

- **Riesgos:** decisiones que podrían causar un problema si no se atienden.
- **Trade-offs:** decisiones donde se ganó algo a cambio de sacrificar otra cosa.
- **Puntos de sensibilidad:** parámetros donde un cambio pequeño tiene un impacto grande en la arquitectura (para bien: significa que el sistema está bien encapsulado en ese punto).

Se hace al final de la Unidad II porque ya existen 8 ADRs con decisiones reales sobre las que analizar — no tendría sentido hacer este análisis al inicio del proyecto, cuando todavía no había arquitectura que evaluar.

---

## 2. Drivers de negocio (contexto rápido)

Tomados del ADR-01: el sistema debe manejar operaciones financieras consistentes (ventas, stock, caja) para un negocio local pequeño, desarrollado por una sola persona en un plazo de tres meses, priorizando tecnologías ya conocidas (C#/.NET) sobre una curva de aprendizaje larga.

---

## 3. Árbol de utilidad (resumen)

| Atributo de calidad | Escenario concreto | Prioridad (negocio / técnica) |
|---|---|---|
| **Modificabilidad** | Cambiar el motor de persistencia (JSON → SQL) sin tocar controladores ni servicios | Alta / Alta |
| **Consistencia de datos** | Una venta y el descuento de stock ocurren juntos o ninguno ocurre | Alta / Media |
| **Testabilidad** | Probar un controlador sin levantar base de datos real | Media / Alta |
| **Portabilidad / costo** | Poder desplegar el sistema en un proveedor de nube gratuito | Media / Alta |

Estos cuatro escenarios son la base de los tres análisis siguientes.

---

## 4. Riesgo — "Transacción atómica" simulada en `VentaService.Registrar`

**Escenario:** el proceso se interrumpe (excepción, caída del servicio) entre el registro de una venta y el descuento del stock de todos sus productos.

**Decisión relacionada:** ADR-01 (operaciones financieras consistentes) y ADR-06 (Deuda técnica 2).

**Por qué es un riesgo:** `VentaService.Registrar` simula atomicidad con un rollback manual en memoria (revertir el stock ya descontado si algo fallaba dentro del propio método), pero cada operación (`Agregar`, `AgregarDetalle`, `Actualizar`) llamaba a `SaveChanges()` de inmediato. Con `JsonDbContext`, eso significa que cada llamada **ya escribió en disco** antes de que el método completo termine. Si el proceso se interrumpe a mitad del camino (no dentro del `try/catch`, sino por una caída real del servicio), el archivo JSON queda en un estado a medias: la venta existe, pero el stock de algunos productos no se descontó.

**Impacto si se materializa:** descuadre entre lo que el sistema cree que hay en inventario y lo que realmente hay — justo el tipo de error que un negocio real detecta de inmediato y que erosiona la confianza en el sistema.

**Estado actual:** con la migración a PostgreSQL (ADR-08) ya existe el motor capaz de resolverlo con una transacción real (`Database.BeginTransaction()`), pero `VentaService` todavía no la usa — el riesgo *técnicamente se puede cerrar* ahora, pero **sigue abierto en el código**. Se mantiene como trabajo pendiente documentado en el ADR-06.

---

## 5. Trade-off — PostgreSQL en vez de SQL Server

**Escenario:** elegir el motor de base de datos para la estrategia SQL de persistencia (ADR-08).

**Decisión relacionada:** ADR-01 (planteaba SQL Server originalmente) → ADR-08 (cambia a PostgreSQL).

**Qué se ganó:**
- Costo $0 en capas gratuitas de proveedores en la nube (Render, Railway, Supabase, Neon), donde SQL Server gratuito es mucho más limitado.
- Portabilidad: PostgreSQL corre igual en Windows, Linux y macOS, sin atarse al ecosistema Microsoft.
- El cambio fue posible sin tocar `Domain`, `Application` ni ningún controlador — solo se sustituyó el proveedor de Entity Framework Core.

**Qué se sacrificó:**
- Se pierde la integración nativa con herramientas del ecosistema Microsoft que el ADR-01 había anticipado (SSMS, integración directa con Azure SQL, ciertas extensiones de Visual Studio pensadas específicamente para SQL Server).
- Tipos de datos y funciones específicas de T-SQL que no tienen equivalente exacto en PostgreSQL, en caso de que en el futuro se necesitaran procedimientos almacenados avanzados.

**Por qué fue la decisión correcta para este proyecto:** el driver de negocio dominante en esta etapa es *poder desplegar una demo funcional y gratuita en la nube* (ver Sección 2), y ese driver pesa más que la integración con herramientas Microsoft que el proyecto no ha usado hasta ahora.

---

## 6. Punto de sensibilidad — La interfaz `IDbContext` (patrón Strategy)

**Escenario:** cambiar el mecanismo de persistencia completo (por ejemplo, de PostgreSQL a MongoDB, o agregar caché con Redis).

**Decisión relacionada:** ADR-05 (Strategy sobre `IDbContext`), confirmada en la práctica por el ADR-08.

**Por qué es un punto de sensibilidad:** toda la capa `Infrastructure` y `Application` depende únicamente de la interfaz `IDbContext`, nunca de una implementación concreta. Esto significa que el sistema es **muy sensible** a esa única interfaz — cualquier cambio en su contrato (agregar un método, cambiar una firma) se propaga a **todas** las estrategias (`JsonDbContext`, `SqlDbContext`) y a los 5 repositorios que la consumen.

**Por qué es algo positivo aquí:** es sensibilidad *controlada* — el sistema tiene un único punto de cambio para una decisión arquitectónica grande (el motor de persistencia), en lugar de tener esa decisión esparcida por decenas de archivos. La prueba concreta: migrar de "JSON únicamente" a "PostgreSQL disponible" (ADR-08) tomó cambios en exactamente 2 archivos nuevos (`SqlDbContext.cs`, configuración en `Program.cs`) y **cero cambios** en los 5 repositorios ni en ningún controlador.

**Contraparte a vigilar:** si en el futuro `IDbContext` necesitara soportar una operación que no todas las estrategias pueden implementar igual de bien (por ejemplo, transacciones reales — ver Sección 4), ese mismo punto de sensibilidad podría convertirse en un cuello de botella de diseño, porque cualquier método nuevo en la interfaz debe tener sentido para *todas* las estrategias, no solo para la más avanzada.

---

## 7. Resumen

| Tipo | Decisión analizada | Estado |
|---|---|---|
| 🔴 Riesgo | Transacción simulada en `VentaService.Registrar` (ADR-06) | Abierto — motor listo (PostgreSQL), lógica pendiente |
| 🔁 Trade-off | PostgreSQL vs. SQL Server (ADR-08) | Decisión tomada y ya implementada |
| 🎯 Punto de sensibilidad | Interfaz `IDbContext` (ADR-05) | Validado en la práctica con el ADR-08 |

---

## 🤖 Uso de Inteligencia Artificial

Este documento fue redactado de forma personal. Se utilizó inteligencia artificial como herramienta de apoyo en los siguientes aspectos específicos:

| Área de uso | Descripción |
|-------------|-------------|
| **Estructura del documento ATAM** | Se usó IA para proponer la estructura estándar de una evaluación ATAM (drivers de negocio, árbol de utilidad, análisis de riesgo/trade-off/sensibilidad) adaptada al formato ya usado en los ADRs del proyecto. |
| **Redacción y enlace con ADRs existentes** | Se empleó IA para redactar el análisis de cada escenario, citando y conectando decisiones ya documentadas en los ADR-01, ADR-05, ADR-06 y ADR-08. |

> **Nota:** La identificación de qué decisiones del proyecto ameritaban análisis de riesgo, trade-off y sensibilidad es de autoría propia, basada en el conocimiento directo del código y las decisiones tomadas a lo largo del semestre.