namespace Nueva_carpeta.Models;

public class FeedViewModel
{
    public List<Publicacion> Publicaciones { get; set; } = new();
    public CrearPublicacionViewModel NuevaPublicacion { get; set; } = new();
    public bool TieneMasPublicaciones { get; set; }
    public int SiguienteDesde { get; set; }
}