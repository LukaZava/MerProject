using MERPROJ.DTO;

namespace MERPROJ.Interfaces
{
    public interface ICustomer
    {
        Task<PaginationDto<CustomerListDto>> GetCustomersAsync(GetQueryDto query);
        Task<CustomerDetailsDto?> GetCustomerByIdAsync(int id);
        Task<CustomerDetailsDto> CreateCustomerAsync(CreateCustomerDto dto);
        Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDto dto);
        Task<bool> DeleteCustomerAsync(int id);
        Task<DeactivateCustomersResponseDto> DeactivateCustomersAsync(List<int> ids);
        Task<CustomerStatsDto> GetStatsAsync();
        Task <List<string>> GetCountriesAsync();
    }
}
