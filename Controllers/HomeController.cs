using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nueva_carpeta.Models;

namespace Nueva_carpeta.Controllers;

public class HomeController : Controller
{
    private readonly BD _baseDeDatos;
    private readonly IWebHostEnvironment _environment;

    public HomeController(BD baseDeDatos, IWebHostEnvironment environment)
    {
        _baseDeDatos = baseDeDatos;
        _environment = environment;
    }

    public IActionResult Index()
    {
        int? idUsuarioLogueado = ObtenerIdUsuarioLogueado();

        FeedViewModel modelo = new FeedViewModel
        {
            Publicaciones = _baseDeDatos.ObtenerPublicaciones(0, 10, idUsuarioLogueado),
            TieneMasPublicaciones = _baseDeDatos.ObtenerPublicaciones(10, 1, idUsuarioLogueado).Count > 0,
            SiguienteDesde = 10
        };

        CargarDatosSesionEnViewBag();
        return View(modelo);
    }

    public IActionResult IniciarSesion()
    {
        if (UsuarioLogueado())
        {
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    [HttpPost]
    public IActionResult IniciarSesion(string nombreUsuario, string contrasena)
    {
        Usuario? usuario = _baseDeDatos.ObtenerUsuarioPorCredenciales(nombreUsuario, contrasena);

        if (usuario == null)
        {
            ViewBag.Error = "El usuario no existe o la contraseña es incorrecta.";
            return View();
        }

        HttpContext.Session.SetInt32("IdUsuarioLogueado", usuario.Id);
        HttpContext.Session.SetString("UsuarioLogueado", usuario.NombreUsuario);
        HttpContext.Session.SetString("NombreUsuarioLogueado", usuario.NombreUsuario);
        HttpContext.Session.SetString("NombreLogueado", usuario.Nombre);
        HttpContext.Session.SetString("ApellidoLogueado", usuario.Apellido);
        HttpContext.Session.SetString("TipoUsuarioLogueado", string.Empty);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Registrarse()
    {
        if (UsuarioLogueado())
        {
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    [HttpPost]
    public IActionResult Registrarse(Usuario usuario)
    {
        if (_baseDeDatos.ExisteUsuario(usuario.NombreUsuario))
        {
            ViewBag.Error = "Ese nombre de usuario ya existe, prueba con otro.";
            return View(usuario);
        }

        _baseDeDatos.AgregarUsuario(usuario);
        return RedirectToAction(nameof(IniciarSesion));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CrearPublicacion(CrearPublicacionViewModel modelo)
    {
        if (!UsuarioLogueado())
        {
            return RedirectToAction(nameof(IniciarSesion));
        }

        if (!ModelState.IsValid || modelo.ImagenArchivo == null || modelo.ImagenArchivo.Length == 0)
        {
            TempData["ErrorPublicacion"] = "Completa todos los datos y agrega una imagen.";
            return RedirectToAction(nameof(Index));
        }

        string extension = Path.GetExtension(modelo.ImagenArchivo.FileName);
        string nombreArchivo = $"{Guid.NewGuid():N}{extension}";
        string carpeta = Path.Combine(_environment.WebRootPath, "uploads", "publicaciones");
        Directory.CreateDirectory(carpeta);

        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);
        using (FileStream stream = new FileStream(rutaCompleta, FileMode.Create))
        {
            modelo.ImagenArchivo.CopyTo(stream);
        }

        int idUsuario = ObtenerIdUsuarioLogueado() ?? 0;
        _baseDeDatos.CrearPublicacion(idUsuario, modelo.Titulo, modelo.Descripcion, nombreArchivo);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult ToggleMeGusta([FromBody] SolicitudPublicacionDto solicitud)
    {
        int? idUsuario = ObtenerIdUsuarioLogueado();

        if (idUsuario == null)
        {
            return Unauthorized(new { success = false, mensaje = "Debes iniciar sesión." });
        }

        var resultado = _baseDeDatos.AlternarMeGusta(solicitud.IdPublicacion, idUsuario.Value);

        if (!resultado.Existe)
        {
            return NotFound(new { success = false, mensaje = "La publicación no existe." });
        }

        return Json(new
        {
            success = true,
            meGusta = resultado.QuedoConMeGusta,
            cantidadMeGusta = resultado.CantidadMeGusta
        });
    }

    [HttpPost]
    public IActionResult Comentar([FromBody] SolicitudComentarioDto solicitud)
    {
        int? idUsuario = ObtenerIdUsuarioLogueado();

        if (idUsuario == null)
        {
            return Unauthorized(new { success = false, mensaje = "Debes iniciar sesión." });
        }

        if (string.IsNullOrWhiteSpace(solicitud.Texto))
        {
            return BadRequest(new { success = false, mensaje = "El comentario no puede estar vacío." });
        }

        if (!_baseDeDatos.ExistePublicacion(solicitud.IdPublicacion))
        {
            return NotFound(new { success = false, mensaje = "La publicación no existe." });
        }

        Comentario? comentario = _baseDeDatos.CrearComentario(solicitud.IdPublicacion, idUsuario.Value, solicitud.Texto.Trim());

        if (comentario == null)
        {
            return StatusCode(500, new { success = false, mensaje = "No se pudo crear el comentario." });
        }

        return Json(new
        {
            success = true,
            comentario = new
            {
                id = comentario.Id,
                nombreUsuario = comentario.NombreUsuario,
                texto = comentario.Texto,
                fechaComentario = comentario.FechaComentario.ToString("dd/MM/yyyy HH:mm")
            }
        });
    }

    [HttpGet]
    public IActionResult ObtenerMas(int desde = 10)
    {
        int? idUsuarioLogueado = ObtenerIdUsuarioLogueado();
        List<Publicacion> publicaciones = _baseDeDatos.ObtenerPublicaciones(desde, 10, idUsuarioLogueado);

        Response.Headers["X-Tiene-Mas"] = publicaciones.Count == 10 ? "true" : "false";
        CargarDatosSesionEnViewBag();

        if (publicaciones.Count == 0)
        {
            return Content(string.Empty);
        }

        return PartialView("_Publicacion", publicaciones);
    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Bienvenida()
    {
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    private bool UsuarioLogueado()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioLogueado"));
    }

    private int? ObtenerIdUsuarioLogueado()
    {
        return HttpContext.Session.GetInt32("IdUsuarioLogueado");
    }

    private void CargarDatosSesionEnViewBag()
    {
        ViewBag.UsuarioLogueado = HttpContext.Session.GetString("UsuarioLogueado");
        ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuarioLogueado");
        ViewBag.Nombre = HttpContext.Session.GetString("NombreLogueado");
        ViewBag.Apellido = HttpContext.Session.GetString("ApellidoLogueado");
        ViewBag.TipoUsuario = HttpContext.Session.GetString("TipoUsuarioLogueado");
    }
}

public class SolicitudPublicacionDto
{
    public int IdPublicacion { get; set; }
}

public class SolicitudComentarioDto
{
    public int IdPublicacion { get; set; }
    public string Texto { get; set; } = string.Empty;
}
