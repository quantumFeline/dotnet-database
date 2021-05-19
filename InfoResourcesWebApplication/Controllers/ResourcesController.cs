using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InfoResourcesWebApplication;
using InfoResourcesWebApplication.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;

namespace InfoResourcesWebApplication.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class ResourcesController : Controller
    {
        private readonly InfoResourcesWebApplicationContext _context;

        public ResourcesController(InfoResourcesWebApplicationContext context)
        {
            _context = context;
        }

        // GET: Resources
        public async Task<IActionResult> Index()
        {
            return View(await _context.Resource
                .Include(n => n.AuthorNavigation)
                .Include(n => n.TypeNavigation)
                .ToListAsync());
        }

        // GET: Resources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resource
                .Include(n => n.AuthorNavigation)
                .Include(n => n.TypeNavigation)
                .FirstOrDefaultAsync(m => m.ResourceId == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // GET: Resources/Create
        public IActionResult Create()
        {
            var authors_list = new SelectList(_context.Author.ToList(), "AuthorId", "FullName");
            ViewData["authors_list"] = authors_list;
            var types_list = new SelectList(_context.ResourceType.ToList(), "ResourceTypeId", "ResourceTypeName");
            ViewData["types_list"] = types_list;
            return View();
        }

        // POST: Resources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResourceId,ResourceName,UrlAddress,Type,Author,Annotation,AddDate")] Resource resource)
        {
            if (ModelState.IsValid)
            {
                Resource existingResource = await _context.Resource.SingleOrDefaultAsync(r =>
                (r.UrlAddress == resource.UrlAddress && r.UrlAddress != null) || r.ResourceName == resource.ResourceName);

                if (existingResource != null)
                {
                    ModelState.AddModelError(string.Empty, "This resource already exists in the base.");
                    var authors_list = new SelectList(_context.Author.ToList(), "AuthorId", "FullName");
                    ViewData["authors_list"] = authors_list;
                    var types_list = new SelectList(_context.ResourceType.ToList(), "ResourceTypeId", "ResourceTypeName");
                    ViewData["types_list"] = types_list;
                    return View(resource);
                }

                _context.Add(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resource);
        }

        // GET: Resources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resource.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }
            var authors_list = new SelectList(_context.Author.ToList(), "AuthorId", "FullName");
            ViewData["authors_list"] = authors_list;
            var types_list = new SelectList(_context.ResourceType.ToList(), "ResourceTypeId", "ResourceTypeName");
            ViewData["types_list"] = types_list;
            return View(resource);
        }

        // POST: Resources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResourceId,ResourceName,UrlAddress,Type,Author,Annotation,AddDate")] Resource resource)
        {
            if (id != resource.ResourceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceExists(resource.ResourceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(resource);
        }

        // GET: Resources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resource
                .Include(n => n.AuthorNavigation)
                .Include(n => n.TypeNavigation)
                .FirstOrDefaultAsync(m => m.ResourceId == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // POST: Resources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resource = await _context.Resource.FindAsync(id);
            _context.Resource.Remove(resource);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index), nameof(Resource), new { id = resource.ResourceId, name = resource.ResourceName});
            return RedirectToAction(nameof(Index));
        }

        private bool ResourceExists(int id)
        {
            return _context.Resource.Any(e => e.ResourceId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid && fileExcel != null)
            {
                using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                {
                    await fileExcel.CopyToAsync(stream);
                    ImportFile(fileExcel, stream);
                }
                await _context.SaveChangesAsync();
                ViewData["ImportMessage"] = "File imported succefully.";
            } else
            {
                ViewData["ImportMessage"] = "Error while importing file: ";
                if (fileExcel == null)
                {
                    ViewData["ImportMessage"] += "file is null";
                } else
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Any())
                   .Select(x => new { x.Key, x.Value.Errors });
                    ViewData["ImportMessage"] += "model state is invalid - " + errors;
                }                
            }

            //return RedirectToAction(nameof(Index));
            return View();
        }

        [NonAction]
        protected void ImportFile(IFormFile fileExcel, FileStream stream)
        {
            
            using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
            {
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    ResourceType type;
                    List<ResourceType> types = (from res in _context.ResourceType
                                where res.ResourceTypeName.Contains(worksheet.Name)
                                select res).ToList();

                    if (types.Count > 0)
                    {
                        type = types[0];
                    }
                    else
                    {
                        type = new ResourceType();
                        type.ResourceTypeName = worksheet.Name;
                        type.ResourceTypeDescription = "from EXCEL";
                        _context.ResourceType.Add(type);
                    }
                    
                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                    {
                        //try
                        {
                            Resource resource = new Resource();
                            resource.ResourceName = row.Cell(1).Value.ToString();
                            resource.Type = type.ResourceTypeId;
                            resource.UrlAddress = row.Cell(5).Value.ToString();
                            resource.Annotation = row.Cell(6).Value.ToString();

                            var add_date = row.Cell(7).Value.ToString();
                            if (add_date.Length > 0)
                            {
                                resource.AddDate = DateTime.Parse(add_date);
                            } else
                            {
                                resource.AddDate = DateTime.Now;
                            }
                            _context.Resource.Add(resource);

                            // Author
                            string authorName = row.Cell(2).Value.ToString();
                            string authorDepartment = row.Cell(3).Value.ToString();
                            if (authorName.Length > 0)
                            {
                                var a = (from aut in _context.Author.ToList()
                                            where aut.FullName.Contains(authorName)
                                            select aut).ToList();

                                if (a.Count > 0)
                                {
                                    resource.Author = a[0] .AuthorId;
                                }
                                else
                                {
                                    Author author = new Author();
                                    var names = "surname, name patronymic".Split(new char[]{ ',', ' '});
                                    if (names.Length != 3)
                                    {
                                        throw new InvalidDataException("Author name can't be parsed");
                                    }
                                    author.LastName = names[0];
                                    author.FirstName = names[2]; // skip the empty entry
                                    author.Patronymic = names[3];

                                    // Set or add department
                                    Department department;
                                    List<Department> departments = (from d in _context.Department
                                                                where d.DepartmentName.Contains(worksheet.Name)
                                                                select d).ToList();

                                    if (departments.Count > 0) {
                                        department = departments[0];
                                    }
                                    else {
                                        throw new InvalidDataException("Unknown author UserDepartment");
                                    }

                                    author.Department = department.DepartmentId;
                                    _context.Author.Add(author);

                                    resource.Author = author.AuthorId;
                                }
                            }
                        }
                        /*catch (Exception)
                        {
                            throw new FileLoadException("Error while importing a database");
                        }*/
                    }
                }
            }
        }

        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var resourceTypes = _context.ResourceType.Include(n => n.Resources).ToList();
                foreach (var type in resourceTypes)
                {
                    var worksheet = workbook.Worksheets.Add(type.ResourceTypeName);

                    worksheet.Cell("A1").Value = "Назва";
                    worksheet.Cell("B1").Value = "Автор";
                    worksheet.Cell("C1").Value = "Факультет";
                    worksheet.Cell("D1").Value = "Тип ресурсу";
                    worksheet.Cell("E1").Value = "URL";
                    worksheet.Cell("F1").Value = "Аннотація";
                    worksheet.Cell("G1").Value = "Дата";
                    worksheet.Row(1).Style.Font.Bold = true;

                    List<Resource> resources = type.Resources.ToList();

                    for (int i = 0; i < resources.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = resources[i].ResourceName;
                        Author author = _context.Author.Find(resources[i].Author);
                        worksheet.Cell(i + 2, 2).Value = author.FullName;
                        worksheet.Cell(i + 2, 3).Value = author.Department;
                        ResourceType res_type = _context.ResourceType.Find(resources[i].Type);
                        worksheet.Cell(i + 2, 4).Value = res_type.ResourceTypeName;
                        worksheet.Cell(i + 2, 5).Value = resources[i].UrlAddress;
                        worksheet.Cell(i + 2, 6).Value = resources[i].Annotation;
                        worksheet.Cell(i + 2, 7).Value = resources[i].AddDate;
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"resources_db_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

    }
}
