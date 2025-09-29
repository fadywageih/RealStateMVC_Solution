using Microsoft.EntityFrameworkCore;
using RealState.BLL.Common.Services.AttachmentService;
using RealState.BLL.Models.Property;
using RealState.DAL.Models.Users;
using RealState.DAL.Presistance.UnitOfWork;

namespace RealState.BLL.Services.Property
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttachmentService _attachmentService;

        public PropertyService(IUnitOfWork unitOfWork, IAttachmentService attachmentService)
        {

            _unitOfWork = unitOfWork;
            _attachmentService = attachmentService;
        }
        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync(string? search = null)
        {
            var query = _unitOfWork.PropertyRepository.GetAllAsQuerable()
                .AsNoTracking()
                .Where(p => !p.IsDeleted);

            // دعم البحث في العنوان أو المدينة
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(search) ||
                    p.City.ToLower().Contains(search));
            }

            var properties = await query
                .Include(p => p.Images) // تضمين الصور
                .Select(p => new PropertyDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    PriceType = p.PriceType.ToString(), // enum → string
                    PropertyType = p.PropertyType.ToString(),
                    City = p.City,
                    ContactPhone = p.ContactPhone,
                    CreatedOn = p.CreatedOn,
                    ImageUrls = p.Images
                        .Where(img => !img.IsDeleted) // لو عندك IsDeleted في الصورة
                        .Select(img => img.Url)  // أو img.FileName حسب التصميم
                        .ToList()
                })
                .ToListAsync();

            return properties;
        }
        public async Task<int> CreatePropertiesAsync(CreatePropertyDto dto, int userId)
        {
            var property = new DAL.Models.Users.Property
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                PriceType = dto.PriceType,
                PropertyType = dto.PropertyType,
                City = dto.City,
                ContactPhone = dto.ContactPhone,
                UserId = userId,
                CreatedOn = DateTime.UtcNow
            };

            if (dto.Images != null && dto.Images.Any())
            {
                var imageUrls = new List<string>();
                foreach (var file in dto.Images)
                {
                    if (file.Length > 0)
                    {
                        var url = await _attachmentService.UploadFileAsync(file, "properties"); // ✅ مع await
                        imageUrls.Add(url);
                    }
                }

                property.Images = imageUrls.Select(url => new PropertyImage
                {
                    Url = url,
                    CreatedOn = DateTime.UtcNow
                }).ToList();
            }

            _unitOfWork.PropertyRepository.Add(property);
            return await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> DeletePropertiesAsync(int id)
        {
            var property = await _unitOfWork.PropertyRepository.GetByIdAsync(id);
            if (property == null || property.IsDeleted)
                return false;

            // احذف الملفات الفعلية أولًا
            foreach (var image in property.Images)
            {
                // مثال على الرابط: /File/PropertyImage?folder=properties&fileName=abc123.jpg
                var uri = new Uri("http://temp" + image.Url); // نستخدم URI للتحليل
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var folder = query["folder"];
                var fileName = query["fileName"];

                if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(fileName))
                {
                    _attachmentService.DeleteFile(folder, fileName);
                }
            }

            property.IsDeleted = true;
            _unitOfWork.PropertyRepository.Update(property);
            await _unitOfWork.CompleteAsync();
            return true;
        }



        public async Task<PropertyDetailsDto?> GetPropertiesIdAsync(int id)
        {
            var property = await _unitOfWork.PropertyRepository
        .GetAllAsQuerable()
        .Include(p => p.Images)
        .Include(p => p.User)
        .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (property == null)
                return null;

            return new PropertyDetailsDto
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                PriceType = property.PriceType.ToString(),
                PropertyType = property.PropertyType.ToString(),
                City = property.City,
                ContactPhone = property.ContactPhone,
                CreatedOn = property.CreatedOn,
                ImageUrls = property.Images
                    .Where(img => !img.IsDeleted)
                    .Select(img => img.Url)
                    .ToList()
            };
        }
        public async Task<int> UpdatePropertiesAsync(UpdatedPropertyDto dto)
        {
            // 1. جب العقار من قاعدة البيانات (مع الصور)
            var existingProperty = await _unitOfWork.PropertyRepository
                .GetAllAsQuerable()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == dto.Id && !p.IsDeleted);

            if (existingProperty == null)
                throw new Exception("Property not found.");

            // 2. حدّث الحقول الأساسية
            existingProperty.Title = dto.Title;
            existingProperty.Description = dto.Description;
            existingProperty.Price = dto.Price;
            existingProperty.PriceType = dto.PriceType;
            existingProperty.PropertyType = dto.PropertyType;
            existingProperty.City = dto.City;
            existingProperty.ContactPhone = dto.ContactPhone;

            // 3. حذف الصور المطلوبة
            if (dto.ImagesToDelete != null && dto.ImagesToDelete.Any())
            {
                var imagesToRemove = existingProperty.Images
                    .Where(img => !img.IsDeleted && dto.ImagesToDelete.Contains(img.Id))
                    .ToList();

                foreach (var img in imagesToRemove)
                {
                    // احذف الملف من السيرفر
                    try
                    {
                        var uri = new Uri("http://temp" + img.Url);
                        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                        var folder = query["folder"];
                        var fileName = query["fileName"];

                        if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(fileName))
                        {
                            _attachmentService.DeleteFile(folder, fileName);
                        }
                    }
                    catch { /* تجاهل أي خطأ في الحذف */ }

                    img.IsDeleted = true;
                }
            }

            // 4. إضافة صور جديدة
            if (dto.NewImages != null && dto.NewImages.Any())
            {
                var newImageUrls = new List<string>();
                foreach (var file in dto.NewImages)
                {
                    if (file.Length > 0)
                    {
                        var url = await _attachmentService.UploadFileAsync(file, "properties");
                        existingProperty.Images.Add(new PropertyImage { Url = url, CreatedOn = DateTime.UtcNow });
                    }
                }

                foreach (var url in newImageUrls)
                {
                    existingProperty.Images.Add(new PropertyImage
                    {
                        Url = url,
                        CreatedOn = DateTime.UtcNow
                    });
                }
            }

            // 5. حدّث العقار (الكائن الأصلي)
            _unitOfWork.PropertyRepository.Update(existingProperty);
            return await _unitOfWork.CompleteAsync();
        }
        public async Task<IEnumerable<PropertyDto>> GetMyPropertiesAsync(int userId)
        {
            var properties = await _unitOfWork.PropertyRepository
                .GetAllAsQuerable()
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.UserId == userId) // ← فقط عقارات المستخدم الحالي
                .Include(p => p.Images)
                .Select(p => new PropertyDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    PriceType = p.PriceType.ToString(),
                    PropertyType = p.PropertyType.ToString(),
                    City = p.City,
                    ContactPhone = p.ContactPhone,
                    CreatedOn = p.CreatedOn,
                    ImageUrls = p.Images
                        .Where(img => !img.IsDeleted)
                        .Select(img => img.Url)
                        .ToList()
                })
                .ToListAsync();

            return properties;
        }
    }
}
