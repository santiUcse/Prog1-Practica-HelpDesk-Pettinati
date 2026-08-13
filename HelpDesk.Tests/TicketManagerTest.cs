namespace HelpDesk.Tests;
using HelpDesk.Entidades;

public class TicketManagerTest
{
    [Test]
    public void CrearTicket_Valido_QuedaEnEstadoAbierto()
    {
        TicketManager manager = new TicketManager();

        Ticket ticket = manager.CrearTicket( "No funciona la impresora", "la impresora no enciende", Ticket.Prioridad.Media);

        Assert.That(ticket.Estado, Is.EqualTo(Ticket.EstadoTicket.Abierto));
    }

    [Test]
    public void CrearTicket_TituloVacio_LanzaArgumentException()
    {
        TicketManager manager = new TicketManager();

        Assert.Throws<ArgumentException>(() =>
            manager.CrearTicket( "", "Descripción", Ticket.Prioridad.Baja));
    }

    [Test]
    public void CrearTicket_TituloMuyLargo_LanzaArgumentException()
    {
        TicketManager manager = new TicketManager();
        string titulo = new string('A', 101);

        Assert.Throws<ArgumentException>(() => manager.CrearTicket( titulo, "Descripción", Ticket.Prioridad.Alta));
    }

    [Test]
public void CrearTicket_DescripcionVacia_LanzaArgumentException()
{
    TicketManager manager = new TicketManager();

    Assert.Throws<ArgumentException>(() => manager.CrearTicket( "Título", "", Ticket.Prioridad.Media));
}

[Test]
public void CambiarEstado_SecuenciaCorrecta_TerminaEnCerrado()
{
    TicketManager manager = new TicketManager();

    Ticket ticket = manager.CrearTicket("Error","Descripción",Ticket.Prioridad.Media);

    manager.TomarTicket(ticket.Id);
    manager.ResolverTicket(ticket.Id);
    manager.CerrarTicket(ticket.Id);

    Assert.That(ticket.Estado, Is.EqualTo(Ticket.EstadoTicket.Cerrado));
}

[Test]
public void CerrarTicket_DesdeAbierto_LanzaInvalidOperationException()
{
    TicketManager manager = new TicketManager();

    Ticket ticket = manager.CrearTicket("Error","Descripción",Ticket.Prioridad.Media);

    Assert.Throws<InvalidOperationException>(() =>manager.CerrarTicket(ticket.Id));
}

[Test]
public void ModificarTicket_Cerrado_LanzaInvalidOperationException()
{
    TicketManager manager = new TicketManager();

    Ticket ticket = manager.CrearTicket("Error","Descripción",Ticket.Prioridad.Media);

    manager.TomarTicket(ticket.Id);
    manager.ResolverTicket(ticket.Id);
    manager.CerrarTicket(ticket.Id);

    Assert.Throws<InvalidOperationException>(() =>manager.ResolverTicket(ticket.Id));
}

[Test]
public void BuscarPorId_Inexistente_LanzaException()
{
    TicketManager manager = new TicketManager();

    Assert.Throws<Exception>(() =>manager.BuscarPorId(99));
}

[Test]
public void ObtenerPorEstado_Abierto_DevuelveSoloTicketsAbiertos()
{
    TicketManager manager = new TicketManager();

    Ticket t1 = manager.CrearTicket("Error 1","Descripción",Ticket.Prioridad.Baja);

    Ticket t2 = manager.CrearTicket("Error 2","Descripción",Ticket.Prioridad.Alta);

    manager.TomarTicket(t2.Id);

    List<Ticket> abiertos = manager.ObtenerPorEstado(Ticket.EstadoTicket.Abierto);

    Assert.That(abiertos.Count, Is.EqualTo(1));
    Assert.That(abiertos[0].Id, Is.EqualTo(t1.Id));
}

[Test]
public void BuscarPorTitulo_TextoExistente_DevuelveCoincidencias()
{
    TicketManager manager = new TicketManager();

    manager.CrearTicket("Error de impresora","Descripción",Ticket.Prioridad.Media);

    manager.CrearTicket("Pantalla azul","Descripción",Ticket.Prioridad.Alta);

    List<Ticket> resultado = manager.BuscarPorTitulo("impresora");

    Assert.That(resultado.Count, Is.EqualTo(1));
    Assert.That(resultado[0].Titulo, Is.EqualTo("Error de impresora"));
}


}