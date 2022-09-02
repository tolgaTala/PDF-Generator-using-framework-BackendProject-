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
            //Viewe göndereceğim dto'yu oluşturuyorum.
            PdfDto pdfDto = new PdfDto();
            //dto için customeri ve ona ait productsları veri tabanından çekiyorum.
            var resultCustomer = _customerService.GetCustomerById(customerId);
            var resultProduct = _productService.GetProductsByCustomerId(resultCustomer.Data.CustomerId);
            List<List<Answers>> productsAnswers = new List<List<Answers>>();
            //veri tabanından her productın asnwerlarını çekiyorum
            for (int i = 0; i < resultProduct.Data.Count; i++)
            {
                var resultAnswer = _answersService.GetAnswersByProductId(resultProduct.Data[i].ProductId);
                //aşağıdaki işlemleri yapma sebebim veri tabanındaki verilerin değerlerini rastgele verdiğim için
                //total değelerin ilk başta 0 olması gerektiğinden sıfırlıyorum.
                resultProduct.Data[i].AllTotal = 0;
                resultProduct.Data[i].SubTotal = 0;
                //her productın hesaplama işlemlerini yapıyorum
                foreach (var item in resultAnswer.Data)
                {                    
                    //normalde veri tabanına eklenirken yapılması gereken işlemi ben elle doldurduğum için burada yapıyorum.
                    item.total = item.Qnty * item.Price;
                    //Her productın SubTotal ve AllTotal değerlerini dolduruyorum.
                    resultProduct.Data[i].SubTotal += item.total;
                    resultProduct.Data[i].AllTotal += item.total;
                    
                }
                //Sırayla productların answerlerını dolduruyorum.
                productsAnswers.Add(resultAnswer.Data);
                //En sonunda da customerın grandTotalını dolduruyorum.
                resultCustomer.Data.GrandTotal += resultProduct.Data[i].AllTotal;
            }        
            //Viewe gidecekk dtoyu dolduruyorum.
            pdfDto.Customer = resultCustomer.Data;
            pdfDto.Products = resultProduct.Data;
            pdfDto.ProductsAnswers = productsAnswers;
            //Viewi pdfe dönüştürüyorum.
            return await _generatePdf.GetPdf("Views/pdfPage.cshtml", pdfDto);
        }
        

    }
}
