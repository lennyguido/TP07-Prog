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
}
