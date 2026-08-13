using Dapper;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nueva_carpeta.Models;

namespace Nueva_carpeta.Controllers;

public class HomeController : Controller
{ public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Index(string nombreUsuario, string contrasena, string nombre, string apellido, string tipoUsuario)
    {
    BD baseDeDatos = new BD();

        Usuario usuario = new Usuario(nombreUsuario, contrasena, nombre, apellido, tipoUsuario);

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
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
