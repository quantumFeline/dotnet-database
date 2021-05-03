using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InfoResourcesWebApplication.Data;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InfoResourcesWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : Controller
    {
        private readonly InfoResourcesWebApplicationContext _context;

        public ChartsController(InfoResourcesWebApplicationContext context)
        {
           _context = context;
        }

        [HttpGet("JsonTypeStats")]
        public JsonResult JsonTypeStats()
        {
            List<ResourceType> types = _context.ResourceType.Include(r => r.Resources).ToList();
            List<object> types_to_resources = new List<object>();
            types_to_resources.Add(new string[] { "Тип ресурсу", "Кількість ресурсів" });
            foreach(var type in types)
            {
                types_to_resources.Add(new object[] { type.ResourceTypeName, type.Resources.Count });
            }

            return new JsonResult(types_to_resources);
        }
    }
}
