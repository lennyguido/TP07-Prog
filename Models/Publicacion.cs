namespace Nueva_carpeta.Models;

public class Publicacion
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Imagen { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public int CantidadMeGusta { get; set; }
    public bool YaLeGusto { get; set; }
    public List<Comentario> Comentarios { get; set; }

    public Publicacion()
    {
        Id = 0;
        IdUsuario = 0;
        NombreUsuario = string.Empty;
        Titulo = string.Empty;
        Descripcion = string.Empty;
        Imagen = string.Empty;
        FechaPublicacion = new DateTime();
        CantidadMeGusta = 0;
        YaLeGusto = false;
        Comentarios = new List<Comentario>();
    }
}
