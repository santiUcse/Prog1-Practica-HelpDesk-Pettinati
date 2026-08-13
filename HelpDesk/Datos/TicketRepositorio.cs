using System.Text.Json;
using HelpDesk.Entidades;

namespace HelpDesk;

public class TicketRepositorio
{
    private readonly string _ruta;

    public TicketRepositorio(string ruta)
    {
        _ruta = ruta;
    }

    public List<Ticket> ObtenerTodos()
    {
        if (!File.Exists(_ruta))
            return new List<Ticket>();

        string json = File.ReadAllText(_ruta);

        return JsonSerializer.Deserialize<List<Ticket>>(json) 
               ?? new List<Ticket>();
    }

    public void Guardar(List<Ticket> tickets)
    {
        string json = JsonSerializer.Serialize(tickets,
            new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(_ruta, json);
    }
}