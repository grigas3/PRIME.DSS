using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PRIME.Core.Web.Controllers
{
    /// <summary>
    /// Sample Home Controller
    /// </summary>
    public class AppController : Controller
    {

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Error Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}