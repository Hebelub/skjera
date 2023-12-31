using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using prosjekt.Data;
using prosjekt.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;


namespace prosjekt.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrganizationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Organization
        //SearchOrganizations
        public async Task<IActionResult> Index(string searchString)
        {
            var organization = from m in _context.OrganizationModels
                select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                organization = organization.Where(s => s.Name.ToLower().Contains(searchString.ToLower()));
            }

            return View(await organization.ToListAsync());
        }


        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }


        // GET: Organization/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var organization = await _context.OrganizationModels.FindAsync(id);

            if (organization == null)
            {
                return NotFound();
            }

            return View(organization);
            // return RedirectToAction("Organization", "Event", new { area = "", id });
        }

        // GET: Organization/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.FormType = FormType.Create;
            return View("_OrganizationForm");
        }

        private string UploadLogo(IFormFile file)
        {
            string uniqueFileName = string.Empty;
            
            if (file != null)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/uploads");
                uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
            }

            return uniqueFileName;
        }
        

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([FromForm][Bind("Name,Description,Logo")] OrganizationModel organization)
        {
            organization.LogoUrl = UploadLogo(organization.Logo);
            
            if (!ModelState.IsValid)
            {
                ViewBag.FormType = FormType.Create;
                return View("_OrganizationForm", organization);
            }

            var user = await _userManager.GetUserAsync(User);

            // The user who created this has all the access-rights
            var userOrganizationAccess = new UserOrganizationRelation(user, organization, AccessRight.FullAccess);

            _context.Add(organization);
            _context.Add(userOrganizationAccess);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Organization/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!OrganizationAccess(id ?? 0).CanEditOrganization)
            {
                return NotFound();
            }

            var organizationModel = await _context.OrganizationModels.FindAsync(id);

            if (organizationModel == null)
            {
                return NotFound();
            }

            ViewBag.FormType = FormType.Edit;
            return View("_OrganizationForm", organizationModel);
        }

        // POST: Organization/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Logo,LogoUrl")] OrganizationModel organizationModel)
        {
            organizationModel.Id = id;

            organizationModel.LogoUrl = UploadLogo(organizationModel.Logo);

            if (!OrganizationAccess(id).CanEditOrganization)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.FormType = FormType.Edit;
                return View("_OrganizationForm", organizationModel);
            }

            try
            {
                _context.Update(organizationModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizationModelExists(organizationModel.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Organization/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!OrganizationAccess(id ?? 0).CanDeleteOrganization)
            {
                return NotFound();
            }

            var organizationModel = await _context.OrganizationModels
                .FirstOrDefaultAsync(m => m.Id == id);

            if (organizationModel == null)
            {
                return NotFound();
            }

            return View(organizationModel);
        }

        // POST: Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var organizationModel = await _context.OrganizationModels.FindAsync(id);

            if (!OrganizationAccess(id).CanDeleteOrganization)
            {
                return NotFound();
            }

            if (organizationModel != null)
            {
                _context.OrganizationModels.Remove(organizationModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrganizationModelExists(int id)
        {
            return _context.OrganizationModels.Any(e => e.Id == id);
        }

        private AccessRight OrganizationAccess(int organizationId)
        {
            return User.IsInRole("Admin")
                ? AccessRight.FullAccess
                : _userManager.GetUserAsync(User).Result.GetRelationToOrganizationAsync(organizationId).Result
                    .AccessRight;
        }
    }
}