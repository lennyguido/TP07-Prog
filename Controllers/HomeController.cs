using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nueva_carpeta.Models;

namespace Nueva_carpeta.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
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

        HttpContext.Session.SetString("UsuarioLogueado", usuario.NombreUsuario);
        HttpContext.Session.SetString("NombreUsuarioLogueado", usuario.NombreUsuario);
        HttpContext.Session.SetString("NombreLogueado", usuario.Nombre);
        HttpContext.Session.SetString("ApellidoLogueado", usuario.Apellido);
        HttpContext.Session.SetString("TipoUsuarioLogueado", usuario.TipoUsuario);

        return RedirectToAction("Bienvenida");
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

    return View("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Bienvenida()
    {
        string? usuarioLogueado = HttpContext.Session.GetString("UsuarioLogueado");

        if (string.IsNullOrEmpty(usuarioLogueado))
        {
            return RedirectToAction("IniciarSesion");
        }

        ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuarioLogueado");
        ViewBag.Nombre = HttpContext.Session.GetString("NombreLogueado");
        ViewBag.Apellido = HttpContext.Session.GetString("ApellidoLogueado");
        ViewBag.TipoUsuario = HttpContext.Session.GetString("TipoUsuarioLogueado");
        return View();
    }

    public IActionResult CerrarSesion()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}