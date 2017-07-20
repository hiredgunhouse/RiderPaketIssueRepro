using System;
using System.IO;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyProject.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(
            ILogger<HomeController> logger,
            IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            ////return this.File("~/index.html", "text/html");
            string webRootPath = _hostingEnvironment.WebRootPath;
            string contentRootPath = _hostingEnvironment.ContentRootPath;

            //return Content(webRootPath + "\n" + contentRootPath);
            //return File(Path.Combine(webRootPath, "index.html"), "text/html");
            var indexHtmlPath = Path.Combine(webRootPath, "index.html");
            return Content(System.IO.File.ReadAllText(indexHtmlPath), "text/html");
        }

        public IActionResult Error()
        {
            // Get the details of the exception that occurred
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var message = string.Empty;

            if (exceptionFeature != null)
            {
                // Get which route the exception occurred at
                string routeWhereExceptionOccurred = exceptionFeature.Path;

                // Get the exception that occurred
                Exception exceptionThatOccurred = exceptionFeature.Error;

                // TODO: Do something with the exception
                // Log it with Serilog?
                // Send an e-mail, text, fax, or carrier pidgeon?  Maybe all of the above?
                // Whatever you do, be careful to catch any exceptions, otherwise you'll end up with a blank page and throwing a 500

                message = $"Unhandled exception: {exceptionThatOccurred}";
                _logger.LogError(new EventId(666, "EvilError"), exceptionThatOccurred, "Unhandled exception");
            }
            else
            {
                _logger.LogError(new EventId(666, "EvilError"), "Unhandled exception, no exception feature :/");
            }

#if DEBUG
            return this.Content(message);
#else
            return View();
#endif
        }
    }
}
