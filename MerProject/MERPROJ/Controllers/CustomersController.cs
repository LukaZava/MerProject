using MERPROJ.DTO;
using MERPROJ.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MERPROJ.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly ICustomer _customerService;
        public CustomersController(ICustomer customerService)
        {
            _customerService = customerService;
        }
        [HttpGet]
        public async Task<ActionResult<PaginationDto<CustomerListDto>>> GetCustomers([FromQuery] GetQueryDto query)
        {
            var result = await _customerService.GetCustomersAsync(query);
            return Ok(result);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDetailsDto>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
        [HttpPost]
        public async Task<ActionResult<CustomerDetailsDto>> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var createdCustomer = await _customerService.CreateCustomerAsync(dto);

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = createdCustomer.Id },
                createdCustomer);
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
        {
            var updated = await _customerService.UpdateCustomerAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var deleted = await _customerService.DeleteCustomerAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpPost("bulk-deactivate")]
        public async Task<ActionResult<DeactivateCustomersResponseDto>> BulkDeactivate([FromBody] List<int> ids)
        {
            if (ids.Count > 1000)
            {
                return BadRequest("You can deactivate up to 1000 customers at once.");
            }

            var result = await _customerService.DeactivateCustomersAsync(ids);
            return Ok(result);
        }
        [HttpGet("stats")]
        public async Task<ActionResult<CustomerStatsDto>> GetStats()
        {
            var stats = await _customerService.GetStatsAsync();
            return Ok(stats);
        }
        [HttpGet("countries")]
        public async Task<ActionResult<List<string>>> GetCountries()
        {
            var countries = await _customerService.GetCountriesAsync();
            return Ok(countries);
        }

    }
}
