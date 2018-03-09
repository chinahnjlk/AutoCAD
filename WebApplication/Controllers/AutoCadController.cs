using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using netDxf.Entities;
using Newtonsoft.Json;
using RtSafe.DxfCore;

namespace WebApplication.Controllers
{
    public class AutoCadController : Controller
    {
        // GET: AutoCAD
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetData(string type,string layer)
        {

            //FileInfo fileInfo = new FileInfo(@"P:\Demo\WebApplication\WebApplication\Draw3.dxf");

            using (var ms = System.IO.File.OpenRead(@"P:\Demo\AutoCAD\WebApplication\Draw3.dxf"))
            {
                DxfHelperReader dxfHelperReader = new DxfHelperReader();
                var list1 = dxfHelperReader.Read(ms, int.Parse(type), layer);

                 //D:\学习\asp.net\vs2017\WebApplication\WebApplication\Draw3.dxf
                //
                return Json(new {list=list1}, JsonRequestBehavior.AllowGet);
            }         
        }
    }
}