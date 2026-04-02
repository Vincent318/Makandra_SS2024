
using Replay.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Replay.Controllers;

public class ErrorController : Controller {


    public ErrorController() {

    }
    public IActionResult Index(int statusCode) {
         var viewName = statusCode switch
        {   
            404 => "NotFound",
            403 => "Forbidden",
            400 => "BadRequest",
            500 => "InternalError",
            503 => "ServiceUnavailable",
            _ => "Index"
        };
        return View(viewName, new ErrorViewModel { StatusCode = statusCode });     
    }
}