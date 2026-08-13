//crea una clase usuario con nombre usuario, contraseña, nombre, apellido y tipoUsuario porfavor

public class Usuario
{
    public string NombreUsuario { get; set; }
    public string Contrasena { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string TipoUsuario { get; set; }

    public Usuario(string nombreUsuario, string contrasena, string nombre, string apellido, string tipoUsuario)
    {
        NombreUsuario = nombreUsuario;
        Contrasena = contrasena;
        Nombre = nombre;
        Apellido = apellido;
        TipoUsuario = tipoUsuario;
    }
}
//me ayudas usando dapper a conectarlo a la base de datos.
