namespace HelpDesk;
using HelpDesk.Entidades;

public class TicketService
{
    private readonly TicketRepositorio _repositorio;

    public TicketService(TicketRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public List<Ticket> ObtenerTodos()
    {
        return _repositorio.ObtenerTodos();
    }

    public Ticket? ObtenerPorId(int id)
    {
        return _repositorio.ObtenerTodos()
            .FirstOrDefault(t => t.Id == id);
    }

    public void Crear(Ticket ticket)
    {
        var tickets = _repositorio.ObtenerTodos();

        ticket.Id = tickets.Count == 0 
            ? 1 
            : tickets.Max(t => t.Id) + 1;

        tickets.Add(ticket);

        _repositorio.Guardar(tickets);
    }

    public void TomarTicket(int id)
    {
        var ticket = ObtenerPorId(id);

        if (ticket == null)
            throw new Exception("Ticket inexistente");

        if (ticket.Estado != Ticket.EstadoTicket.Abierto)
            throw new InvalidOperationException("Estado inválido");

        ticket.Estado = Ticket.EstadoTicket.EnProgreso;

        _repositorio.Guardar(ObtenerTodos());
    }

    public void ResolverTicket(int id)
    {
        var ticket = ObtenerPorId(id);

        if (ticket == null)
            throw new Exception("Ticket inexistente");

        if (ticket.Estado != Ticket.EstadoTicket.EnProgreso)
            throw new InvalidOperationException("Estado inválido");

        ticket.Estado = Ticket.EstadoTicket.Resuelto;

        _repositorio.Guardar(ObtenerTodos());
    }

    public void CerrarTicket(int id)
    {
        var ticket = ObtenerPorId(id);

        if (ticket == null)
            throw new Exception("Ticket inexistente");

        if (ticket.Estado != Ticket.EstadoTicket.Resuelto)
            throw new InvalidOperationException("Estado inválido");

        ticket.Estado = Ticket.EstadoTicket.Cerrado;

        _repositorio.Guardar(ObtenerTodos());
    }

    public List<Ticket> ObtenerPorEstado(Ticket.EstadoTicket estado)
    {
        return ObtenerTodos()
            .Where(t => t.Estado == estado)
            .ToList();
    }

    public List<Ticket> BuscarPorTitulo(string texto)
    {
        return ObtenerTodos()
            .Where(t => t.Titulo.Contains(texto, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}