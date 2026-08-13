using HelpDesk;

namespace Api.DTOs;

public class TicketResponse
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Prioridad Prioridad { get; set; }
    public EstadoTicket Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
}
