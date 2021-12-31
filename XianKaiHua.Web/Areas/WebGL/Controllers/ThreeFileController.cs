using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianKaiHua.Web.Areas.WebGL.Controllers
{
    [Area("WebGL")]
    /// <summary>
    /// Three资源文件请求
    /// </summary>
    public class ThreeFileController : Controller
    {
        [HttpGet]
        public Stream FileLoading(string strpath)
        {
            strpath = "/wwwroot" + strpath;
            //if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + strpath))
            //{
            //    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + strpath);
            //}
            var filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory() + strpath);

            string strContent = string.Empty;

            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            strContent = Encoding.ASCII.GetString(buffer);
            fs.Close();
            Stream stream = new MemoryStream(buffer);
            return stream;
            //return strContent;
        }


        //[HttpGet]
        //public Stream FileLoading(string strpath,string filename)
        //{
        //    strpath = "/wwwroot" + strpath;
        //    if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + strpath))
        //    {
        //        System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + strpath);
        //    }
        //    var filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory() + strpath, filename);
            
        //    string strContent = string.Empty;

        //    FileStream fs = new FileStream(filePath, FileMode.Open);
        //    byte[] buffer = new byte[fs.Length];
        //    fs.Read(buffer, 0, buffer.Length);
        //    strContent = Encoding.UTF8.GetString(buffer);
        //    fs.Close();
        //    //byte[] 转换成 Stream
        //   Stream stream = new MemoryStream(buffer);
        //    return stream;
        //    //return strContent;
        //    //return buffer;
        //}
    }
}
