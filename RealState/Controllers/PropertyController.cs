using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealState.BLL.Models.Property;
using RealState.BLL.Services.Property;
using RealState.DAL.Common.Enums;
using RealState.DAL.Presistance.UnitOfWork;
using System.Security.Claims;

namespace RealState.Controllers
{
    [Authorize]
    public class PropertyController : Controller
    {
        #region Service

        private readonly IPropertyService _PropertyService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PropertyController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PropertyController(
            IPropertyService employeeService,
            IWebHostEnvironment webHostEnvironment,
            ILogger<PropertyController> logger,
            IUnitOfWork unitOfWork

            ) // ASK CLR for Creating Object from PropertyService Implicitly
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _PropertyService = employeeService;
        }
        #endregion
        #region Index
        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            var employees = await _PropertyService.GetAllPropertiesAsync(search);

            return View(employees);
        }
        #endregion
        #region Create
        #region  - GET
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        #endregion
        #region Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePropertyDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            // 🔍 ابحث عن User المخصص المرتبط بـ ApplicationUser
            var customUser = await _unitOfWork.UserRepository
                .GetAllAsQuerable()
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

            if (customUser == null)
            {
                ModelState.AddModelError(string.Empty, "User profile not found.");
                return View(dto);
            }

            try
            {
                await _PropertyService.CreatePropertiesAsync(dto, customUser.Id); // ← int
                TempData["SuccessMessage"] = "Property created successfully!";
                return RedirectToAction("Index", "Property");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }
        #endregion
        #endregion
        #region Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var property = await _PropertyService.GetPropertiesIdAsync(id);
            if (property == null)
                return NotFound();

            return View(property);
        }
        #endregion
        #region My Offers
        [HttpGet]
        public async Task<IActionResult> MyOffers()
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized();

            var customUser = await _unitOfWork.UserRepository
                .GetAllAsQuerable()
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);

            if (customUser == null)
                return BadRequest("User profile not found.");

            var myProperties = await _PropertyService.GetMyPropertiesAsync(customUser.Id);
            return View(myProperties);
        }
        #endregion
        #region Edit
        #region Get
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id is null)
                return BadRequest();
            var Property = await _PropertyService.GetPropertiesIdAsync(id.Value);
            if (Property is null)
            {
                return NotFound();

            }
            return View(new UpdatedPropertyDto()
            {
                Id = Property.Id,
                Title = Property.Title,
                Description = Property.Description,
                Price = Property.Price,
                PriceType = Enum.Parse<PriceType>(Property.PriceType),
                PropertyType = Enum.Parse<PropertyType>(Property.PropertyType),
                City = Property.City,
                ContactPhone = Property.ContactPhone

            });
        }
        #endregion
        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken] // To Prevent CSRF Attacks
        public async Task<IActionResult> Edit( UpdatedPropertyDto PropertyDto)
        {
            if (!ModelState.IsValid)
                return View(PropertyDto);
            var massage = string.Empty;
            try
            {
                var updated = await _PropertyService.UpdatePropertiesAsync(PropertyDto) > 0;
                if (updated)
                    return RedirectToAction(nameof(Index));
                massage = "Property is not Updated";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (_webHostEnvironment.IsDevelopment())
                    massage = ex.Message;
                else massage = "the Property is not Created";

            }
            ModelState.AddModelError(string.Empty, massage);
            return View(PropertyDto);

        }
        #endregion
        #endregion
        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _PropertyService.DeletePropertiesAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Property deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete property.";
            }
            return RedirectToAction("MyOffers");
        }
        #endregion
    }
}
