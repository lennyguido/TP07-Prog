namespace Nueva_carpeta.Models;

public class Comentario
{
    public int Id { get; set; }
    public int IdPublicacion { get; set; }
    public int IdUsuarioComenta { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public DateTime FechaComentario { get; set; }
}