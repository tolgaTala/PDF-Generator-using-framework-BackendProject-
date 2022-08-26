using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Entities.Dto;
using Microsoft.AspNetCore.Mvc;
using Wkhtmltopdf.NetCore;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfGeneratorController : ControllerBase
    {
        ICustomerService _customerService;
        IProductService _productService;
        IAnswersService _answersService;
        readonly IGeneratePdf _generatePdf;

        public PdfGeneratorController(ICustomerService customerService, IProductService productService,
            IAnswersService answersService, IGeneratePdf generatePdf)
        {
            _answersService = answersService;
            _customerService = customerService;
            _productService = productService;
            _generatePdf = generatePdf;
        }

        [HttpGet("getpdf")]
        public async Task<IActionResult> Getpdf()
        {
            var resultCustomer = _customerService.GetAll();
            var resultProduct = _productService.GetAll(resultCustomer.Data[0].CustomerId);
            var resultAnswers = _answersService.GetAll(resultProduct.Data[0].ProductId);

            forPdfDto forPdfDto = new forPdfDto();
            forPdfDto.Customer = resultCustomer.Data[0];
            forPdfDto.Products = resultProduct.Data;
            forPdfDto.Answers = resultAnswers.Data;

            return await _generatePdf.GetPdf("Views/pdfPage.cshtml", forPdfDto);
        }
    }
}
