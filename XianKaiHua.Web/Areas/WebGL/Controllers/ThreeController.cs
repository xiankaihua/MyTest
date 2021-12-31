using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XianKaiHua.Web.Controllers
{
    [Area("WebGL")]
    //[Authorize]

    public class ThreeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Editor()
        {
            return View();
        }

        /// <summary>
        /// 场景创建
        /// </summary>
        /// <returns></returns>
        public IActionResult SceneIndex()
        {
            return View();
        }


        public IActionResult Sprites3D()
        {
            return View();
        }

    }
}
