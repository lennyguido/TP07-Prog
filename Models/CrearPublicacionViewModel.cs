using System.ComponentModel.DataAnnotations;

namespace Nueva_carpeta.Models;

public class CrearPublicacionViewModel
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [StringLength(4000, MinimumLength = 5)]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public IFormFile? ImagenArchivo { get; set; }
}