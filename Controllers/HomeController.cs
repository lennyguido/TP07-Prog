using Microsoft.AspNetCore.Mvc;
using Nueva_carpeta.Models;

namespace Nueva_carpeta.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;

    public HomeController(IWebHostEnvironment env)
    {
        _env = env;
    }

    public IActionResult Index()
    {
        BD baseDeDatos = new BD();
        int idUsuario = 0;
        string? idUsuarioTexto = HttpContext.Session.GetString("IdUsuarioLogueado");

        if (!string.IsNullOrEmpty(idUsuarioTexto))
        {
            idUsuario = int.Parse(idUsuarioTexto);
        }

        ViewBag.UsuarioLogueado = HttpContext.Session.GetString("UsuarioLogueado");
        ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuarioLogueado");

        List<Publicacion> publicaciones = baseDeDatos.ObtenerPublicaciones(0, idUsuario);
        return View(publicaciones);
    }

    public IActionResult IniciarSesion()
    {
        return View();
    }

    [HttpPost]
    public IActionResult IniciarSesion(string nombreUsuario, string contrasena)
    {
        BD baseDeDatos = new BD();
        Usuario? usuario = baseDeDatos.ObtenerUsuarioPorCredenciales(nombreUsuario, contrasena);

        if (usuario == null)
        {
            ViewBag.Error = "El usuario no existe o la contraseña es incorrecta.";
            return View();
        }

        HttpContext.Session.SetString("IdUsuarioLogueado", usuario.Id.ToString());
        HttpContext.Session.SetString("UsuarioLogueado", usuario.NombreUsuario);
        HttpContext.Session.SetString("NombreUsuarioLogueado", usuario.NombreUsuario);
        HttpContext.Session.SetString("NombreLogueado", usuario.Nombre);
        HttpContext.Session.SetString("ApellidoLogueado", usuario.Apellido);
        HttpContext.Session.SetString("TipoUsuarioLogueado", usuario.TipoUsuario);

        return RedirectToAction("Index");
    }

    public IActionResult Registrarse()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrarse(Usuario usuario)
    {
        BD baseDeDatos = new BD();

        if (baseDeDatos.ExisteUsuario(usuario.NombreUsuario))
        {
            ViewBag.Error = "Ese nombre de usuario ya existe, prueba con otro.";
            return View("Registrarse");
        }

        baseDeDatos.AgregarUsuario(usuario);
        return RedirectToAction("IniciarSesion");
    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult CrearPublicacion(string titulo, string descripcion, IFormFile imagen)
    {
        string? idUsuarioTexto = HttpContext.Session.GetString("IdUsuarioLogueado");

        if (string.IsNullOrEmpty(idUsuarioTexto))
        {
            return RedirectToAction("IniciarSesion");
        }

        if (imagen != null && imagen.Length > 0 && titulo != "" && descripcion != "")
        {
            string nombreArchivo = imagen.FileName;
            string rutaCarpeta = Path.Combine(_env.WebRootPath, "img", "publicaciones");

            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
            }

            string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                imagen.CopyTo(stream);
            }

            Publicacion publicacion = new Publicacion();
            publicacion.IdUsuario = int.Parse(idUsuarioTexto);
            publicacion.Titulo = titulo;
            publicacion.Descripcion = descripcion;
            publicacion.Imagen = nombreArchivo;
            publicacion.FechaPublicacion = DateTime.Now;

            BD baseDeDatos = new BD();
            baseDeDatos.AgregarPublicacion(publicacion);
        }

        return RedirectToAction("Index");
    }

    public Publicacion MeGusta(int idPublicacion)
    {
        Publicacion respuesta = new Publicacion();
        string? idUsuarioTexto = HttpContext.Session.GetString("IdUsuarioLogueado");

        if (string.IsNullOrEmpty(idUsuarioTexto))
        {
            return respuesta;
        }

        BD baseDeDatos = new BD();

        if (baseDeDatos.ExistePublicacion(idPublicacion) == false)
        {
            return respuesta;
        }

        int idUsuario = int.Parse(idUsuarioTexto);
        baseDeDatos.CambiarMeGusta(idPublicacion, idUsuario);

        respuesta.Id = idPublicacion;
        respuesta.CantidadMeGusta = baseDeDatos.CantidadMeGusta(idPublicacion);
        respuesta.YaLeGusto = baseDeDatos.UsuarioDioMeGusta(idPublicacion, idUsuario);

        return respuesta;
    }

    [HttpPost]
    public Comentario Comentar(int idPublicacion, string texto)
    {
        Comentario comentario = new Comentario();
        string? idUsuarioTexto = HttpContext.Session.GetString("IdUsuarioLogueado");

        if (string.IsNullOrEmpty(idUsuarioTexto) || texto == null || texto.Replace(" ", "") == "")
        {
            return comentario;
        }

        BD baseDeDatos = new BD();

        if (baseDeDatos.ExistePublicacion(idPublicacion) == false)
        {
            return comentario;
        }

        int idUsuario = int.Parse(idUsuarioTexto);
        Usuario? usuario = baseDeDatos.ObtenerUsuarioPorId(idUsuario);

        comentario.IdPublicacion = idPublicacion;
        comentario.IdUsuarioComenta = idUsuario;
        comentario.Texto = texto;
        comentario.FechaComentario = DateTime.Now;

        if (usuario != null)
        {
            comentario.NombreUsuario = usuario.NombreUsuario;
        }

        baseDeDatos.AgregarComentario(comentario);
        return comentario;
    }

    public List<Publicacion> ObtenerMas(int desde)
    {
        BD baseDeDatos = new BD();
        int idUsuario = 0;
        string? idUsuarioTexto = HttpContext.Session.GetString("IdUsuarioLogueado");

        if (!string.IsNullOrEmpty(idUsuarioTexto))
        {
            idUsuario = int.Parse(idUsuarioTexto);
        }

        return baseDeDatos.ObtenerPublicaciones(desde, idUsuario);
    }
}
