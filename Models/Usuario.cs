namespace Nueva_carpeta.Models;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; }
    public string Contrasena { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string TipoUsuario { get; set; }

    public Usuario()
    {
        Id = 0;
        NombreUsuario = string.Empty;
        Contrasena = string.Empty;
        Nombre = string.Empty;
        Apellido = string.Empty;
        TipoUsuario = string.Empty;
    }
}
