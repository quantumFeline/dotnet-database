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

namespace InfoResourcesWebApplication.Controllers
{
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
            return RedirectToAction(nameof(Index), nameof(Resource), new { id = resource.ResourceId, name = resource.ResourceName});
        }

        private bool ResourceExists(int id)
        {
            return _context.Resource.Any(e => e.ResourceId == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid && fileExcel == null)
            {
                using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                {
                    await fileExcel.CopyToAsync(stream);
                    ImportFile(fileExcel, stream);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public void ImportFile(IFormFile fileExcel, FileStream stream)
        {
            
            using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
            {
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    ResourceType new_res;
                    List<ResourceType> types = (from res in _context.ResourceType
                                where res.ResourceTypeName.Contains(worksheet.Name)
                                select res).ToList();

                    if (types.Count > 0)
                    {
                        new_res = types[0];
                    }
                    else
                    {
                        new_res = new ResourceType();
                        new_res.ResourceTypeName = worksheet.Name;
                        new_res.ResourceTypeDescription = "from EXCEL";
                        _context.ResourceType.Add(new_res);
                    }
                    
                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                    {
                        try
                        {
                            Resource resource = new Resource();
                            resource.ResourceName = row.Cell(1).Value.ToString();
                            resource.Annotation = row.Cell(4).Value.ToString();
                            _context.Resource.Add(resource);

                            // Author
                            string authorName = row.Cell(2).Value.ToString();
                            if (authorName.Length > 0)
                            {
                                var a = (from aut in _context.Author
                                            where aut.FullName.Contains(authorName)
                                            select aut).ToList();

                                if (a.Count > 0)
                                {
                                    resource.Author = a[0].AuthorId;
                                }
                                else
                                {
                                    throw new InvalidDataException("Author not found in the database");
                                }
                            }

                            // ResourceType
                        }
                        catch (Exception e)
                        {
                            //logging самостійно :)

                        }
                    }
                }
            }
        }

    }
}
