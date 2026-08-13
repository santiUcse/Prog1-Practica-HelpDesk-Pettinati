namespace HelpDesk.Entidades;
public class Ticket
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public Prioridad PrioridadAsignada { get; set; }


    public enum Prioridad
    {
        Baja,
        Media,

        Alta,
        Critica
    }
    
    public enum EstadoTicket
    {
        Abierto,
        EnProgreso,
        Resuelto,
        Cerrado
    }
    public EstadoTicket Estado { get; set; }
    public DateTime FechaCreacion { get; set; }

    public Ticket(int id, string titulo, string descripcion, Prioridad prioridad)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("El título es obligatorio");

        if (titulo.Length > 100)
            throw new ArgumentException("El título no puede superar 100 caracteres");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción es obligatoria");

        Id = id;
        Titulo = titulo;
        Descripcion = descripcion;
        PrioridadAsignada = prioridad;
        Estado = EstadoTicket.Abierto;
        FechaCreacion = DateTime.Now;
    }
}