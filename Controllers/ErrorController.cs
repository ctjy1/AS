namespace Assignment2.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;
            return View("Error");
        }
    }

}
