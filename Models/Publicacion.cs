namespace Nueva_carpeta.Models;

public class Publicacion
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Imagen { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public int CantidadMeGusta { get; set; }
    public bool YaLeGusto { get; set; }
    public List<Comentario> Comentarios { get; set; } = new();
}