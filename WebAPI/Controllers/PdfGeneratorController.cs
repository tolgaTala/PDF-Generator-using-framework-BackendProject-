using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Entities.Dto;
using Microsoft.AspNetCore.Mvc;
using Wkhtmltopdf.NetCore;
using Entities.Concrete;
using System.Collections.Generic;

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

        [HttpGet("dekont")]
        public async Task<IActionResult> Getpdf(int customerId)
        {
            forPdfDto forPdfDto = new forPdfDto();
            //kullanıcı Idsine göre geliyor
            var resultCustomer = _customerService.Get(customerId);
            var resultProduct = _productService.GetAll(resultCustomer.Data.CustomerId);
            List<List<Answers>> resultAnswers = new List<List<Answers>>();
            for (int i = 0; i < resultProduct.Data.Count; i++)
            {
                var resultAnswer = _answersService.GetAll(resultProduct.Data[i].ProductId);
                resultAnswers.Add(resultAnswer.Data);
                resultProduct.Data[i].AllTotal = 0;
                resultProduct.Data[i].SubTotal = 0;
                foreach (var item in resultAnswer.Data)
                {                    
                    item.total = item.Qnty * item.Price;
                    resultProduct.Data[i].SubTotal += item.total;
                    resultProduct.Data[i].AllTotal += item.total;
                    
                }

                resultCustomer.Data.GrandTotal += resultProduct.Data[i].AllTotal;
            }
            
            


            forPdfDto.Customer = resultCustomer.Data;
            forPdfDto.Products = resultProduct.Data;
            forPdfDto.Answers = resultAnswers;

            return await _generatePdf.GetPdf("Views/pdfPage.cshtml", forPdfDto);
        }
        

    }
}
