using Dapper;
using Microsoft.Data.SqlClient;

namespace Nueva_carpeta.Models;

public class BD
{
    private string _connectionString = @"Server=localhost;DataBase=DBRedSocial;Integrated Security=True;TrustServerCertificate=True;";

    public void AgregarUsuario(Usuario usuario)
    {
        string query = "INSERT INTO Usuarios (NombreUsuario, Contraseña, Nombre, Apellido, TipoUsuario) VALUES (@NombreUsuario, @Contrasena, @Nombre, @Apellido, @TipoUsuario)";

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
        string query = "SELECT * FROM Usuarios WHERE NombreUsuario = @NombreUsuario";

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
        string query = @"SELECT Id,
                                NombreUsuario,
                                Contraseña AS Contrasena,
                                Nombre,
                                Apellido,
                                TipoUsuario
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

    public Usuario? ObtenerUsuarioPorId(int idUsuario)
    {
        string query = @"SELECT Id,
                                NombreUsuario,
                                Contraseña AS Contrasena,
                                Nombre,
                                Apellido,
                                TipoUsuario
                         FROM Usuarios
                         WHERE Id = @IdUsuario";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            return connection.QueryFirstOrDefault<Usuario>(query, new
            {
                IdUsuario = idUsuario
            });
        }
    }

    public void AgregarPublicacion(Publicacion publicacion)
    {
        string query = @"INSERT INTO Publicaciones (IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion)
                         VALUES (@IdUsuario, @Titulo, @Descripcion, @Imagen, @FechaPublicacion)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                IdUsuario = publicacion.IdUsuario,
                Titulo = publicacion.Titulo,
                Descripcion = publicacion.Descripcion,
                Imagen = publicacion.Imagen,
                FechaPublicacion = publicacion.FechaPublicacion
            });
        }
    }

    public bool ExistePublicacion(int idPublicacion)
    {
        string query = "SELECT * FROM Publicaciones WHERE Id = @IdPublicacion";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            List<Publicacion> publicaciones = connection.Query<Publicacion>(query, new
            {
                IdPublicacion = idPublicacion
            }).ToList();

            if (publicaciones.Count > 0)
            {
                return true;
            }

            return false;
        }
    }

    public List<Publicacion> ObtenerPublicaciones(int desde, int idUsuarioLogueado)
    {
        List<Publicacion> publicaciones = new List<Publicacion>();

        string query = @"SELECT *
                         FROM Publicaciones
                         ORDER BY FechaPublicacion DESC
                         OFFSET @Desde ROWS FETCH NEXT 10 ROWS ONLY";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            publicaciones = connection.Query<Publicacion>(query, new
            {
                Desde = desde
            }).ToList();
        }

        foreach (Publicacion publicacion in publicaciones)
        {
            Usuario? usuario = ObtenerUsuarioPorId(publicacion.IdUsuario);

            if (usuario != null)
            {
                publicacion.NombreUsuario = usuario.NombreUsuario;
            }

            publicacion.CantidadMeGusta = CantidadMeGusta(publicacion.Id);
            publicacion.Comentarios = ObtenerComentarios(publicacion.Id);

            if (idUsuarioLogueado != 0)
            {
                publicacion.YaLeGusto = UsuarioDioMeGusta(publicacion.Id, idUsuarioLogueado);
            }
        }

        return publicaciones;
    }

    public int CantidadMeGusta(int idPublicacion)
    {
        string query = "SELECT Id FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            List<int> meGusta = connection.Query<int>(query, new
            {
                IdPublicacion = idPublicacion
            }).ToList();

            return meGusta.Count;
        }
    }

    public bool UsuarioDioMeGusta(int idPublicacion, int idUsuario)
    {
        string query = "SELECT Id FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion AND IdUsuario = @IdUsuario";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            List<int> meGusta = connection.Query<int>(query, new
            {
                IdPublicacion = idPublicacion,
                IdUsuario = idUsuario
            }).ToList();

            if (meGusta.Count > 0)
            {
                return true;
            }

            return false;
        }
    }

    public void CambiarMeGusta(int idPublicacion, int idUsuario)
    {
        string buscar = "SELECT Id FROM PublicacionesMeGusta WHERE [IdPublicación] = @IdPublicacion AND IdUsuario = @IdUsuario";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            List<int> meGusta = connection.Query<int>(buscar, new
            {
                IdPublicacion = idPublicacion,
                IdUsuario = idUsuario
            }).ToList();

            if (meGusta.Count > 0)
            {
                string borrar = "DELETE FROM PublicacionesMeGusta WHERE Id = @Id";
                connection.Execute(borrar, new
                {
                    Id = meGusta[0]
                });
            }
            else
            {
                string agregar = "INSERT INTO PublicacionesMeGusta ([IdPublicación], IdUsuario) VALUES (@IdPublicacion, @IdUsuario)";
                connection.Execute(agregar, new
                {
                    IdPublicacion = idPublicacion,
                    IdUsuario = idUsuario
                });
            }
        }
    }

    public List<Comentario> ObtenerComentarios(int idPublicacion)
    {
        List<Comentario> comentarios = new List<Comentario>();
        string query = "SELECT * FROM Comentarios WHERE IdPublicacion = @IdPublicacion ORDER BY FechaComentario ASC";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            comentarios = connection.Query<Comentario>(query, new
            {
                IdPublicacion = idPublicacion
            }).ToList();
        }

        foreach (Comentario comentario in comentarios)
        {
            Usuario? usuario = ObtenerUsuarioPorId(comentario.IdUsuarioComenta);

            if (usuario != null)
            {
                comentario.NombreUsuario = usuario.NombreUsuario;
            }

        }

        return comentarios;
    }

    public void AgregarComentario(Comentario comentario)
    {
        string query = @"INSERT INTO Comentarios (IdPublicacion, IdUsuarioComenta, Texto, FechaComentario)
                         VALUES (@IdPublicacion, @IdUsuarioComenta, @Texto, @FechaComentario)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Execute(query, new
            {
                IdPublicacion = comentario.IdPublicacion,
                IdUsuarioComenta = comentario.IdUsuarioComenta,
                Texto = comentario.Texto,
                FechaComentario = comentario.FechaComentario
            });
        }
    }
}
