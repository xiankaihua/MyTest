using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XianKaiHua.IServices;

namespace XianKaiHua.Web.Controllers
{
    public class HomeController : Controller
    {
        private IPersonServices _ipersonServices;
        public HomeController(IPersonServices personServices_p)
        {
            _ipersonServices = personServices_p;
        }

        #region Index
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetList(int page, int limit, string titles, string state, string qid, string startTime, string endTime)
        {
            int total = 0;
            //var list = _wordtesttableService.GetList(limit, page, titles, state, out total);
            var list = _ipersonServices.TestGetList(limit, page, titles, out total);
            return Json(new { code =200, result = "成功", total = total, data = list });
        }
        #endregion
    }
}
