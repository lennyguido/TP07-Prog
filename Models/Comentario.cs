namespace Nueva_carpeta.Models;

public class Comentario
{
    public int Id { get; set; }
    public int IdPublicacion { get; set; }
    public int IdUsuarioComenta { get; set; }
    public string NombreUsuario { get; set; }
    public string Texto { get; set; }
    public DateTime FechaComentario { get; set; }

    public Comentario()
    {
        Id = 0;
        IdPublicacion = 0;
        IdUsuarioComenta = 0;
        NombreUsuario = string.Empty;
        Texto = string.Empty;
        FechaComentario = new DateTime();
    }
}
