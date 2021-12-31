using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XianKaiHua.Web.Areas.React.Contorllers
{
    [Area("React")]
    public class ReactHomeController : Controller
    {
        public IActionResult ReactIndex()
        {
            return View();
        }
    }
}
