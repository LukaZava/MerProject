using MERPROJ.DTO;
using MERPROJ.Interfaces;
using MERPROJ.Models;
using Microsoft.EntityFrameworkCore;

namespace MERPROJ.Services
{
    public class CustomerService : ICustomer
    {
        private readonly MerDbContext _context;
        public CustomerService(MerDbContext context)
        {
            _context = context;
        }
        public async Task<CustomerDetailsDto> CreateCustomerAsync(CreateCustomerDto dto)
        {
            var emailExists = await _context.Customers
                 .AnyAsync(c => c.Email == dto.Email);

            if (emailExists)
            {
                throw new InvalidOperationException("Customer with this email already exists");
            }

            var now = DateTime.UtcNow;

            var customer = new Customer
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                City = dto.City.Trim(),
                Country = dto.Country.Trim(),
                IsActive = dto.IsActive,
                CreatedAt = now,
                LastModifiedAt = null
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return new CustomerDetailsDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                City = customer.City,
                Country = customer.Country,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                LastModifiedAt = customer.LastModifiedAt
            };
        }

        public async Task<DeactivateCustomersResponseDto> DeactivateCustomersAsync(List<int> ids)
        {

            if (ids == null || ids.Count == 0)
            {
                return new DeactivateCustomersResponseDto { UpdatedCount = 0 };
            }

            var now = DateTime.UtcNow;

            var updatedCount = await _context.Customers
                .Where(c => ids.Contains(c.Id) && c.IsActive)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.IsActive, false)
                    .SetProperty(c => c.LastModifiedAt, now));

            return new DeactivateCustomersResponseDto
            {
                UpdatedCount = updatedCount
            };
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return false;
            }

            if (!customer.IsActive)
            {
                return true;
            }

            customer.IsActive = false;
            customer.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CustomerDetailsDto?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                 .AsNoTracking()
                 .Where(c => c.Id == id)
                 .Select(c => new CustomerDetailsDto
                 {
                     Id = c.Id,
                     FirstName = c.FirstName,
                     LastName = c.LastName,
                     Email = c.Email,
                     Phone = c.Phone,
                     City = c.City,
                     Country = c.Country,
                     IsActive = c.IsActive,
                     CreatedAt = c.CreatedAt,
                     LastModifiedAt = c.LastModifiedAt
                 })
                 .FirstOrDefaultAsync();
        }

        public async Task<PaginationDto<CustomerListDto>> GetCustomersAsync(GetQueryDto query)
        {
            var customersQuery = _context.Customers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();

                customersQuery = customersQuery.Where(c =>
                    c.FirstName.ToLower().Contains(search) ||
                    c.LastName.ToLower().Contains(search) ||
                    c.Email.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(query.City))
            {
                var city = query.City.Trim().ToLower();
                customersQuery = customersQuery.Where(c => c.City.ToLower() == city);
            }

            if (!string.IsNullOrWhiteSpace(query.Country))
            {
                var country = query.Country.Trim().ToLower();
                customersQuery = customersQuery.Where(c => c.Country.ToLower() == country);
            }

            if (query.IsActive.HasValue)
            {
                customersQuery = customersQuery.Where(c => c.IsActive == query.IsActive.Value);
            }

            customersQuery = ApplySorting(customersQuery, query.SortBy, query.SortDirection);

            var totalCount = await customersQuery.CountAsync();

            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var pageSize = query.PageSize < 1 ? 20 : query.PageSize;
            if (pageSize > 100) pageSize = 100;

            var items = await customersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerListDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    City = c.City,
                    Country = c.Country,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    LastModifiedAt = c.LastModifiedAt
                })
                .ToListAsync();
            return new PaginationDto<CustomerListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<CustomerStatsDto> GetStatsAsync()
        {
            var totalCount = await _context.Customers.CountAsync();
            var activeCount = await _context.Customers.CountAsync(c => c.IsActive);
            var inactiveCount = totalCount - activeCount;

            var topCities = await _context.Customers
                .AsNoTracking()
                .GroupBy(c => c.City)
                .Select(g => new CityCountDto
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.City)
                .Take(5)
                .ToListAsync();

            return new CustomerStatsDto
            {
                TotalCount = totalCount,
                ActiveCount = activeCount,
                InactiveCount = inactiveCount,
                TopCities = topCities
            };
        }

        public async Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return false;
            }

            var emailExists = await _context.Customers
                .AnyAsync(c => c.Email == dto.Email && c.Id != id);

            if (emailExists)
            {
                throw new InvalidOperationException("Customer with this email already exists");
            }

            customer.FirstName = dto.FirstName.Trim();
            customer.LastName = dto.LastName.Trim();
            customer.Email = dto.Email.Trim();
            customer.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
            customer.City = dto.City.Trim();
            customer.Country = dto.Country.Trim();
            customer.IsActive = dto.IsActive;
            customer.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        private static IQueryable<Customer> ApplySorting(
            IQueryable<Customer> query,
            string? sortBy,
            string? sortDirection)
        {
            var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy?.ToLower() switch
            {
                "id" => descending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),

                "firstname" => descending
                    ? query.OrderByDescending(c => c.FirstName).ThenByDescending(c => c.Id)
                    : query.OrderBy(c => c.FirstName).ThenBy(c => c.Id),

                "lastname" => descending
                    ? query.OrderByDescending(c => c.LastName).ThenByDescending(c => c.Id)
                    : query.OrderBy(c => c.LastName).ThenBy(c => c.Id),

                "email" => descending
                    ? query.OrderByDescending(c => c.Email).ThenByDescending(c => c.Id)
                    : query.OrderBy(c => c.Email).ThenBy(c => c.Id),

                "city" => descending
                    ? query.OrderByDescending(c => c.City).ThenByDescending(c => c.Id)
                    : query.OrderBy(c => c.City).ThenBy(c => c.Id),

                "country" => descending
                    ? query.OrderByDescending(c => c.Country).ThenByDescending(c => c.Id)
                    : query.OrderBy(c => c.Country).ThenBy(c => c.Id),

                "createdat" => descending
                    ? query.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
                    : query.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id),

                _ => descending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
            };
        }


        public async Task<List<string>> GetCountriesAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .Select(c => c.Country)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}

