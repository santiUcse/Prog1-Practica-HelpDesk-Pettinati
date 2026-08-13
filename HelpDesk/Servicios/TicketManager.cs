namespace HelpDesk;
using HelpDesk.Entidades;

public class TicketManager
{
    private List<Ticket> tickets = new List<Ticket>();



    public Ticket CrearTicket(
        string titulo,
        string descripcion,
        Ticket.Prioridad prioridad)
    {
        int nuevoId = tickets.Count + 1;

        Ticket ticket = new Ticket(
            nuevoId,
            titulo,
            descripcion,
            prioridad
        );

        tickets.Add(ticket);

        return ticket;
    }



    public List<Ticket> ObtenerTodos()
    {
        return tickets;
    }


    // Buscar por ID
    public Ticket BuscarPorId(int id)
    {
        Ticket ticket = tickets.FirstOrDefault(t => t.Id == id);

        if (ticket == null) throw new Exception("El ticket no existe");

        return ticket;
    }


   
    public void TomarTicket(int id)
    {
        CambiarEstado(id, Ticket.EstadoTicket.EnProgreso);
    }



    public void ResolverTicket(int id)
    {
        CambiarEstado(id, Ticket.EstadoTicket.Resuelto);
    }


    public void CerrarTicket(int id)
    {
        CambiarEstado(id, Ticket.EstadoTicket.Cerrado);
    }


    private void CambiarEstado(
        int id,
        Ticket.EstadoTicket nuevoEstado)
    {
        Ticket ticket = BuscarPorId(id);


        if (ticket.Estado == Ticket.EstadoTicket.Cerrado) throw new InvalidOperationException( "No se puede modificar un ticket cerrado");


        if ((int)nuevoEstado != (int)ticket.Estado + 1)throw new InvalidOperationException( "Transición de estado inválida");


        ticket.Estado = nuevoEstado;
    }


    public List<Ticket> ObtenerPorEstado(
        Ticket.EstadoTicket estado)
    {
        return tickets
            .Where(t => t.Estado == estado)
            .ToList();
    }


    public List<Ticket> BuscarPorTitulo(string texto)
    {
        return tickets
            .Where(t => t.Titulo.Contains(
                texto,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}