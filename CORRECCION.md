# Corrección — Práctica HelpDesk (Unidad 6)

## Nota previa importante
Este repositorio es **prácticamente idéntico, archivo por archivo**, al de la entrega `Prog1-Practica-HelpDesk-Martinez` (mismo código en `Ticket.cs`, `TicketManager.cs`, `TicketService.cs`, `TicketRepositorio.cs`, capa `Api/`, etc. — la única diferencia de contenido es un reordenamiento cosmético de un `using` en `TicketManagerTest.cs`). Además, **todo el historial de commits de este repo está firmado por el autor "Benjamin Martinez"** (mismo nombre que en el repo de Martinez), no por Pettinati:

```
39fc969 2026-08-13 Benjamin Martinez Solucionamos problemas
0298632 2026-08-13 Benjamin Martinez Solucionamos problemas
1edacc0 2026-08-13 Benjamin Martinez borramo cosa2
4a7640c 2026-08-13 Benjamin Martinez Agregué API
85bbd86 2026-08-13 santiUcse Initial commit
```

Se deja constancia de esto para que quien corrija lo tenga en cuenta; el detalle técnico que sigue abajo es el mismo análisis de código que se hizo sobre el repo de Martinez, dado que el código fuente es el mismo.

## Build y tests
`dotnet build HelpDesk.sln` compila sin errores (mismo warning de nullabilidad en `TicketManager.cs:40`). `dotnet test` corre **11/11 tests en verde**.

El `.sln` solo incluye `HelpDesk` y `HelpDesk.Tests`; `Api/HelpDeskApi.csproj` no está en la solución y no compilaría tal cual está (`Api/Program.cs` llama `new TicketService("tickets.json")`, pero el constructor de `TicketService` recibe un `TicketRepositorio`, no un `string`). No es parte de esta práctica, así que no afecta la corrección.

## Arquitectura por capas
Capas separadas correctamente (`Entidades/`, `Servicios/`, `Datos/`), pero **conviven dos implementaciones paralelas**: `TicketManager` (in-memory, sin persistencia) y `TicketService` (con persistencia real). Todos los tests prueban `TicketManager`; `TicketService` — la clase que ya está inyectada en `Api/Program.cs` y que sería la usada por una futura Web API — no tiene ningún test.

## Entidad Ticket y reglas de negocio
`Ticket.cs` valida título vacío, título >100 caracteres y descripción vacía con `ArgumentException`, arranca en `Abierto` y asigna `FechaCreacion` automáticamente. Enums `Prioridad` y `EstadoTicket` anidados dentro de `Ticket`, correctos.

La máquina de estados está bien resuelta en `TicketManager.CambiarEstado` (compara `(int)nuevoEstado != (int)ticket.Estado + 1` para bloquear saltos/retrocesos, y chequea que no esté `Cerrado`). En `TicketService` la misma regla se repite manualmente en cada método (`TomarTicket`, `ResolverTicket`, `CerrarTicket`) en vez de centralizarse.

## Persistencia
**Hay un bug de persistencia real y reproducible en `TicketService`** (verificado ejecutando el flujo `Crear` → `TomarTicket` contra un archivo real, ya que el código es idéntico al de Martinez):

- `TicketRepositorio` usa `System.Text.Json` (no Newtonsoft.Json como sugiere la consigna).
- El constructor de `Ticket(int id, string titulo, string descripcion, Prioridad prioridad)` no cubre `Estado` ni `FechaCreacion`, y el parámetro `prioridad` no coincide en nombre con la propiedad `PrioridadAsignada`. Al deserializar con `System.Text.Json`, esto tira:
  ```
  System.InvalidOperationException: Each parameter in the deserialization constructor on type
  'HelpDesk.Entidades.Ticket' must bind to an object property or field on deserialization.
  ```
- Resultado: **cualquier operación que lea el archivo después de que ya tiene al menos un ticket guardado crashea** — `TomarTicket`, `ResolverTicket`, `CerrarTicket`, `ObtenerTodos`, `ObtenerPorId`, `ObtenerPorEstado`, `BuscarPorTitulo`, todos pasan por `_repositorio.ObtenerTodos()`.
- Aparte del crash: `TomarTicket`/`ResolverTicket`/`CerrarTicket` llaman a `ObtenerPorId(id)` (que lee el archivo y devuelve un `Ticket`), mutan su `Estado` en memoria, y luego llaman `_repositorio.Guardar(ObtenerTodos())` — pero `ObtenerTodos()` vuelve a leer el archivo desde cero, devolviendo objetos distintos e independientes de los mutados. El cambio de estado se perdería aunque la deserialización no fallara.
- Como este código nunca se ejecuta en los tests entregados (que usan `TicketManager`), el bug no fue detectado.

El manejo de archivo inexistente sí está bien resuelto (`if (!File.Exists(_ruta)) return new List<Ticket>();`).

## Tests unitarios
8 tests en `TicketManagerTest.cs` cubren el checklist pedido: creación válida, título vacío, título largo, descripción vacía, secuencia completa de transición, dos transiciones inválidas, búsqueda por id inexistente, filtrado por estado y búsqueda por texto. Nomenclatura `Metodo_Escenario_ResultadoEsperado` respetada, patrón AAA presente. `UnitTest1.cs` quedó la plantilla default de NUnit sin uso real.

El problema central es el mismo que en arquitectura: testean `TicketManager`, no `TicketService`, así que la persistencia queda sin cobertura.

## Preparación para la Web API (5 reglas de diseño)
1. **Service sin Console/ReadLine**: cumplido.
2. **`ObtenerPorId` devuelve null**: cumplido en `TicketService.ObtenerPorId`. `TicketManager.BuscarPorId` en cambio lanza `throw new Exception(...)` si no encuentra el ticket — no sigue la regla, aunque no es la clase pensada para la API.
3. **Excepciones tipadas**: `ArgumentException`/`InvalidOperationException` usadas correctamente en validaciones y transiciones. Para "ticket inexistente" se usa `Exception` genérica en `TicketService` (no tipada).
4. **Nombres alineados a verbos HTTP**: `TicketService` cumple (`ObtenerTodos`, `ObtenerPorId`, `Crear`, `TomarTicket`, `ResolverTicket`, `CerrarTicket`, `ObtenerPorEstado`, `BuscarPorTitulo`). `TicketManager` usa `BuscarPorId` en vez de `ObtenerPorId`.
5. **Ruta no hardcodeada como absoluta**: en `Api/Program.cs` se pasa `"tickets.json"` (relativa), correcto.

## Observaciones generales
Ver la nota al inicio del documento sobre la identidad del código y del historial de commits con el repo de Martinez. Más allá de eso, el análisis técnico es el mismo: buenas validaciones en la entidad y buena cobertura de tests sobre `TicketManager`, pero la clase que realmente importa para la próxima etapa (`TicketService`, con persistencia) no está testeada y tiene un bug de deserialización que rompe la persistencia apenas el archivo tiene datos.

## Web API

Se verificó de forma independiente que `Api/` en este repo es **idéntico, archivo por archivo**, al de Martinez (mismo `diff -rq` sin diferencias de contenido, solo de artefactos de build). Por lo tanto vale el mismo análisis, evaluado igual de en serio que en cualquier otra entrega:

El proyecto `Api/HelpDeskApi.csproj` (`Microsoft.NET.Sdk.Web`) implementa el patrón esperado en varios aspectos — controller flaco, DTOs separados, mapeo por extension method — pero **no compila**, por tres motivos independientes:

1. **Referencia de proyecto rota**: `Api/HelpDeskApi.csproj:10` referencia `..\Clases\Clases.csproj`, un proyecto que no existe en el repo (la librería de lógica se llama `HelpDesk` y vive en `..\HelpDesk\HelpDesk.csproj`, no `..\Clases\Clases.csproj`). `dotnet build Api/HelpDeskApi.csproj` falla con 12 errores `CS0246` porque ningún tipo del namespace `HelpDesk` (`Ticket`, `TicketService`, `Prioridad`, `EstadoTicket`) se resuelve. Como el `.sln` tampoco incluye a `Api` como proyecto, este problema queda completamente oculto si solo se corre `dotnet build HelpDesk.sln`.
2. **Firma de `Crear` no coincide**: `TicketController.Crear` (`Api/Controllers/TicketController.cs:36-40`) llama a `_service.Crear(request.Titulo, request.Descripcion, request.Prioridad)` y usa el valor de retorno (`ticket.Id`) para armar el `CreatedAtAction`. Pero `TicketService.Crear` (`HelpDesk/Servicios/TicketService.cs:24`) tiene una única sobrecarga, `Crear(Ticket ticket)`, que devuelve `void`. Aunque se arreglara la referencia de proyecto del punto anterior, este método seguiría sin compilar.
3. **DI rota en `Program.cs`**: `Api/Program.cs:21` registra `builder.Services.AddSingleton<TicketService>(_ => new TicketService("tickets.json"))`, pero el único constructor de `TicketService` recibe un `TicketRepositorio`, no un `string`.

Más allá de estos tres bloqueantes de compilación, el diseño se ajusta bien a la rúbrica:

- **Arquitectura de 3 proyectos**: la intención es correcta — `Api` referencia a la capa de lógica, no al revés, sin referencia circular. El problema es solo el path roto del punto 1.
- **Controller flaco**: `TicketController` cumple con `[ApiController]` y `[Route("api/[controller]")]`, sin lógica de negocio propia — delega todo a `_service` y solo arma la respuesta HTTP.
- **Endpoints**: cubre todas las operaciones esperadas — `GET /api/ticket` (todos), `GET /api/ticket/{id}`, `POST /api/ticket` (crear), `POST /api/ticket/{id}/tomar|resolver|cerrar` (transiciones), `GET /api/ticket/por-estado` (filtro) y `GET /api/ticket/buscar` (texto en título).
- **DTOs**: `TicketRequest` y `TicketResponse` (`Api/DTOs/`) están correctamente ubicados en el proyecto Web API, no en la entidad de dominio ni en el Service. El Service no conoce los DTOs en ningún momento — solo trabaja con `Ticket`.
- **Mapeo**: `TicketMapper.ATicketResponse()` (`Api/DTOs/TicketMapper.cs`) es un extension method usado desde el controller para la dirección entidad→DTO. No existe el mapeo inverso como método aparte; en su lugar el controller intenta pasar los campos sueltos directamente al service (ver punto 2 de bloqueantes arriba), que es precisamente donde falla.
- **Validación**: `TicketRequest` usa `[Required]` y `[StringLength(100, MinimumLength = 1)]` sobre `Titulo`, y `[Required]` sobre `Descripcion` y `Prioridad`, aprovechando el 400 automático de `[ApiController]`.
- **Códigos de estado HTTP**: `ObtenerPorId` devuelve `NotFound()` si el service devuelve `null` (correcto), `Crear` devuelve `201` vía `CreatedAtAction` (correcto, en la intención). En `Tomar`/`Resolver`/`Cerrar` solo se atrapa `InvalidOperationException` para devolver `400` — el caso de "ticket inexistente" en `TicketService` lanza una `Exception` genérica, no tipada, así que **no es capturada por el `catch` del controller** y se propagaría como un 500 no controlado en vez de un 404.
- **Inyección de dependencias**: el patrón elegido (registrar `TicketService` e inyectarlo por constructor) es correcto conceptualmente; lo que falla es la implementación puntual señalada en el punto 3 de arriba.
- **Extras**: Swagger configurado (`AddSwaggerGen`/`UseSwagger`/`UseSwaggerUI`) y CORS también (`AddCors` con política "PermitirTodo"). No se usa `ILogger`.

En síntesis: mismo diagnóstico que en el repo de Martinez — la forma general sigue bien el patrón de la unidad, pero tal como está entregado no compila por tres problemas independientes y acumulativos, y además tiene un agujero real en el manejo de errores para el caso "ticket no encontrado" en las transiciones de estado.
