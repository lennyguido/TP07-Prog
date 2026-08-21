using Dapper;
using Microsoft.Data.SqlClient;
using Nueva_carpeta.Models;

namespace Nueva_carpeta.Models;

public class BD
{

private string _connectionString = @"Server=localhost; DataBase=TP05_Login;Integrated Security=True;TrustServerCertificate=True;";
    public void AgregarUsuario(Usuario usuario)
    {
        string query = "INSERT INTO Usuario (nombreUsuario, contraseña, nombre, apellido, tipoUsuario) VALUES (@NombreUsuario, @Contrasena, @Nombre, @Apellido, @TipoUsuario)";
        
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new 
            { 
                NombreUsuario = usuario.NombreUsuario, 
                Contrasena = usuario.Contrasena, 
                Nombre = usuario.Nombre, 
                Apellido = usuario.Apellido, 
                TipoUsuario = usuario.TipoUsuario 
            });
        }
    }
    public bool ExisteUsuario(string nombreUsuario)
{
    string query = "SELECT * FROM Usuario WHERE nombreUsuario = @NombreUsuario";

    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        List<Usuario> usuarios = connection.Query<Usuario>(query, new
        {
            NombreUsuario = nombreUsuario
        }).ToList();

        if (usuarios.Count > 0)
        {
            return true;
        }

        return false;
    }
}

    public Usuario? ObtenerUsuarioPorCredenciales(string nombreUsuario, string contrasena)
    {
        string query = @"
            SELECT nombreUsuario AS NombreUsuario,
                   contraseña AS Contrasena,
                   nombre AS Nombre,
                   apellido AS Apellido,
                   tipoUsuario AS TipoUsuario
            FROM Usuario
            WHERE nombreUsuario = @NombreUsuario AND contraseña = @Contrasena";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Usuario>(query, new
            {
                NombreUsuario = nombreUsuario,
                Contrasena = contrasena
            });
        }
    }
}
