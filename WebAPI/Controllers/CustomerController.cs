using Business.Abstract;
using Entities.Concrete;
using Entities.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        ICustomerService _customerService;


        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        
        [HttpGet("getcustomerbyId")]
        public IActionResult GetCustomerById(int customerId)
        {
            var result = _customerService.GetCustomerById(customerId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
         
    }
}
