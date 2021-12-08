using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XianKaiHua.IServices;

namespace XianKaiHua.Test.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonServices _ipersonServices;
        public PersonController(IPersonServices personServices_w)
        {
            this._ipersonServices = personServices_w;
        }

        [HttpGet("GetBlogNews")]
        public async Task<HostModel> GetBlogNews()
        {
            var id = this.User.FindFirst("Id").Value;
            var data = await _ipersonServices.Query(c => c.tid == id);
            return new HostModel { code = 200, data = data, result = ""};
        }

        [HttpGet("Add")]
        public async Task<HostModel> Add()
        {
            var id = this.User.FindFirst("Id").Value;
            var data = await _ipersonServices.Add(new Models.Entity.Person {  tid = Guid .NewGuid().ToString()});
            if(data > 0)
                return new HostModel { code = 200, data = data, result = "操作成功" };
            return new HostModel { code = 404, data = data, result = "操作异常" };
        }

        public class HostModel
        {
            public int code { get; set; }
            public string result { get; set; }
            public dynamic data { get; set; }

        }

    }
}
