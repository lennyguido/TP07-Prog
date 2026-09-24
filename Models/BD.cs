using Dapper;
using Microsoft.Data.SqlClient;

namespace Nueva_carpeta.Models;

public class BD
{
    private readonly string _connectionString;

    public BD(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? @"Server=localhost;DataBase=DBRedSocial;Integrated Security=True;TrustServerCertificate=True;";
    }

    public void AgregarUsuario(Usuario usuario)
    {
        string query = @"
            INSERT INTO Usuarios (NombreUsuario, Contraseña, Nombre, Apellido)
            VALUES (@NombreUsuario, @Contrasena, @Nombre, @Apellido)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                NombreUsuario = usuario.NombreUsuario,
                Contrasena = usuario.Contrasena,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido
            });
        }
    }

    public bool ExisteUsuario(string nombreUsuario)
    {
        string query = "SELECT COUNT(1) FROM Usuarios WHERE NombreUsuario = @NombreUsuario";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            int cantidad = connection.ExecuteScalar<int>(query, new
            {
                NombreUsuario = nombreUsuario
            });

            return cantidad > 0;
        }
    }

    public Usuario? ObtenerUsuarioPorCredenciales(string nombreUsuario, string contrasena)
    {
        string query = @"
            SELECT Id,
                   NombreUsuario,
                   Contraseña AS Contrasena,
                   Nombre,
                 Apellido
            FROM Usuarios
            WHERE NombreUsuario = @NombreUsuario AND Contraseña = @Contrasena";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Usuario>(query, new
            {
                NombreUsuario = nombreUsuario,
                Contrasena = contrasena
            });
        }
    }

    public List<Publicacion> ObtenerPublicaciones(int desde, int cantidad, int? idUsuarioLogueado)
    {
        string query = @"
            SELECT p.Id,
                   p.IdUsuario,
                   u.NombreUsuario,
                   p.Titulo,
                   p.Descripcion,
                   p.Imagen,
                   p.FechaPublicacion,
                   (SELECT COUNT(1) FROM PublicacionesMeGusta mg WHERE mg.[IdPublicación] = p.Id) AS CantidadMeGusta,
                   CASE WHEN @IdUsuarioLogueado IS NOT NULL AND EXISTS (
                        SELECT 1
                        FROM PublicacionesMeGusta mg
                        WHERE mg.[IdPublicación] = p.Id AND mg.IdUsuario = @IdUsuarioLogueado
                   ) THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS YaLeGusto
            FROM Publicaciones p
            INNER JOIN Usuarios u ON u.Id = p.IdUsuario
            ORDER BY p.FechaPublicacion DESC, p.Id DESC
            OFFSET @Desde ROWS FETCH NEXT @Cantidad ROWS ONLY;";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            List<Publicacion> publicaciones = connection.Query<Publicacion>(query, new
            {
                Desde = desde,
                Cantidad = cantidad,
                IdUsuarioLogueado = idUsuarioLogueado
            }).ToList();

            foreach (Publicacion publicacion in publicaciones)
            {
                publicacion.Comentarios = ObtenerComentariosPorPublicacion(publicacion.Id);
            }

            return publicaciones;
        }
    }

    public bool ExistePublicacion(int idPublicacion)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            int cantidad = connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Publicaciones WHERE Id = @IdPublicacion", new { IdPublicacion = idPublicacion });
            return cantidad > 0;
        }
    }

    public void CrearPublicacion(int idUsuario, string titulo, string descripcion, string imagen)
    {
        string query = @"
            INSERT INTO Publicaciones (IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion)
            VALUES (@IdUsuario, @Titulo, @Descripcion, @Imagen, @FechaPublicacion)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                IdUsuario = idUsuario,
                Titulo = titulo,
                Descripcion = descripcion,
                Imagen = imagen,
                FechaPublicacion = DateTime.Now
            });
        }
    }

    public List<Comentario> ObtenerComentariosPorPublicacion(int idPublicacion)
    {
        string query = @"
            SELECT c.Id,
                   c.IdPublicacion,
                   c.IdUsuarioComenta,
                   u.NombreUsuario,
                   c.Texto,
                   c.FechaComentario
            FROM Comentarios c
            INNER JOIN Usuarios u ON u.Id = c.IdUsuarioComenta
            WHERE c.IdPublicacion = @IdPublicacion
            ORDER BY c.FechaComentario ASC, c.Id ASC";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.Query<Comentario>(query, new { IdPublicacion = idPublicacion }).ToList();
        }
    }

    public Comentario? CrearComentario(int idPublicacion, int idUsuario, string texto)
    {
        string query = @"
            INSERT INTO Comentarios (IdPublicacion, IdUsuarioComenta, Texto, FechaComentario)
            VALUES (@IdPublicacion, @IdUsuarioComenta, @Texto, @FechaComentario);

            DECLARE @NuevoId INT = CAST(SCOPE_IDENTITY() AS INT);

            SELECT c.Id,
                   c.IdPublicacion,
                   c.IdUsuarioComenta,
                   u.NombreUsuario,
                   c.Texto,
                   c.FechaComentario
            FROM Comentarios c
            INNER JOIN Usuarios u ON u.Id = c.IdUsuarioComenta
            WHERE c.Id = @NuevoId;";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Comentario>(query, new
            {
                IdPublicacion = idPublicacion,
                IdUsuarioComenta = idUsuario,
                Texto = texto,
                FechaComentario = DateTime.Now
            });
        }
    }

    public (bool Existe, bool QuedoConMeGusta, int CantidadMeGusta) AlternarMeGusta(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using SqlTransaction transaction = connection.BeginTransaction();

            bool existePublicacion = connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM Publicaciones WHERE Id = @IdPublicacion",
                new { IdPublicacion = idPublicacion },
                transaction) > 0;

            if (!existePublicacion)
            {
                transaction.Rollback();
                return (false, false, 0);
            }

            bool yaLeGusto = connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion AND IdUsuario = @IdUsuario",
                new { IdPublicacion = idPublicacion, IdUsuario = idUsuario },
                transaction) > 0;

            if (yaLeGusto)
            {
                connection.Execute(
                    "DELETE FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion AND IdUsuario = @IdUsuario",
                    new { IdPublicacion = idPublicacion, IdUsuario = idUsuario },
                    transaction);
            }
            else
            {
                connection.Execute(
                    "INSERT INTO PublicacionesMeGusta ([IdPublicación], IdUsuario) VALUES (@IdPublicacion, @IdUsuario)",
                    new { IdPublicacion = idPublicacion, IdUsuario = idUsuario },
                    transaction);
            }

            int cantidadMeGusta = connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion",
                new { IdPublicacion = idPublicacion },
                transaction);

            transaction.Commit();
            return (true, !yaLeGusto, cantidadMeGusta);
        }
    }
}
