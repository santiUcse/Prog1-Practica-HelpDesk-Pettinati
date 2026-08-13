using System.ComponentModel.DataAnnotations;
using HelpDesk;

namespace Api.DTOs;

public class TicketRequest
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Máximo 100 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public Prioridad Prioridad { get; set; }
}
