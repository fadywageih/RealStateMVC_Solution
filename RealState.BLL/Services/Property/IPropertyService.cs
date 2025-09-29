using RealState.BLL.Models.Property;

namespace RealState.BLL.Services.Property
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync(string? search);
        Task<PropertyDetailsDto?> GetPropertiesIdAsync(int id);
        Task<int> CreatePropertiesAsync(CreatePropertyDto dto, int userId); 
        Task<int> UpdatePropertiesAsync(UpdatedPropertyDto PropertiesDto);
        Task<bool> DeletePropertiesAsync(int id);
        Task<IEnumerable<PropertyDto>> GetMyPropertiesAsync(int userId);
    }
}
