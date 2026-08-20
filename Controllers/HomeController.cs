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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}