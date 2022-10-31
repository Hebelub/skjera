using Microsoft.AspNetCore.Mvc;

namespace prosjekt.Controllers;

public class AdministrateController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}