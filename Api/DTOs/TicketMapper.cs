using HelpDesk;

namespace Api.DTOs;

public static class TicketMapper
{
    public static TicketResponse ATicketResponse(this Ticket ticket)
    {
        return new TicketResponse
        {
            Id = ticket.Id,
            Titulo = ticket.Titulo,
            Descripcion = ticket.Descripcion,
            Prioridad = ticket.Prioridad,
            Estado = ticket.Estado,
            FechaCreacion = ticket.FechaCreacion
        };
    }
}
