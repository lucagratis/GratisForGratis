using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class ErroreController : Controller
    {
        // GET: Errore
        public ActionResult Index(HandleErrorInfo errore)
        {
            return View();
        }

        public ActionResult AccessoNegato()
        {
            return View();
        }
    }
}