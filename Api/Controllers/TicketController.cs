using HelpDesk;
using Microsoft.AspNetCore.Mvc;
using Api.DTOs;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly TicketService _service;

    public TicketController(TicketService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        var responses = _service.ObtenerTodos().Select(t => t.ATicketResponse());
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var ticket = _service.ObtenerPorId(id);
        if (ticket == null)
            return NotFound();

        return Ok(ticket.ATicketResponse());
    }

    [HttpPost]
    public IActionResult Crear([FromBody] TicketRequest request)
    {
        var ticket = _service.Crear(request.Titulo, request.Descripcion, request.Prioridad);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = ticket.Id }, ticket.ATicketResponse());
    }

    [HttpPost("{id}/tomar")]
    public IActionResult Tomar(int id)
    {
        try
        {
            _service.TomarTicket(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("{id}/resolver")]
    public IActionResult Resolver(int id)
    {
        try
        {
            _service.ResolverTicket(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("{id}/cerrar")]
    public IActionResult Cerrar(int id)
    {
        try
        {
            _service.CerrarTicket(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("buscar")]
    public IActionResult BuscarPorTitulo([FromQuery] string texto)
    {
        var resultado = _service.BuscarPorTitulo(texto);
        return Ok(resultado.Select(t => t.ATicketResponse()));
    }

    [HttpGet("por-estado")]
    public IActionResult ObtenerPorEstado([FromQuery] EstadoTicket estado)
    {
        var lista = _service.ObtenerPorEstado(estado);
        return Ok(lista.Select(t => t.ATicketResponse()));
    }
}
