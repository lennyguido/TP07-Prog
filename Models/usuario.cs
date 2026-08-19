namespace Nueva_carpeta.Models;

public class Usuario
{
    public string NombreUsuario { get; set; }
    public string Contrasena { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string TipoUsuario { get; set; }

    public Usuario()
    {
        NombreUsuario = string.Empty;
        Contrasena = string.Empty;
        Nombre = string.Empty;
        Apellido = string.Empty;
        TipoUsuario = string.Empty;
    }

    public Usuario(string nombreUsuario, string contrasena, string nombre, string apellido, string tipoUsuario)
    {
        NombreUsuario = nombreUsuario;
        Contrasena = contrasena;
        Nombre = nombre;
        Apellido = apellido;
        TipoUsuario = tipoUsuario;
    }
}
