using Business.Abstract;
using Microsoft.AspNetCore.Http;
using Entities.Dto;
using Microsoft.AspNetCore.Mvc;
using Entities.Concrete;
using System.Collections.Generic;
using System.Data;
using System.IO;
using IronPdf;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfGeneratorController : ControllerBase
    {
        ICustomerService _customerService;
        IProductService _productService;
        IAnswersService _answersService;

        public PdfGeneratorController(ICustomerService customerService, IProductService productService,
            IAnswersService answersService)
        {
            _answersService = answersService;
            _customerService = customerService;
            _productService = productService;
        }


        [HttpGet("generatepdf")]
        public async Task<IActionResult> GeneratePdfWithQIronPdf(int customerId)
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

            string html = " <!DOCTYPE html>\r\n<html>\r\n<head>\r\n   <link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap@3.4.1/dist/css/bootstrap.min.css\" integrity=\"sha384-HSMxcRTRxnN+Bdg0JdbxYKrThecOKuH5zCYotlSAcp1+c8xmyTe9GYg1l9a69psu\" crossorigin=\"anonymous\">\r\n\r\n    <style>\r\n        .mainContain{\r\n            \r\n            width:100%;\r\n padding:0; margin-top:-24px;       }\r\n        .ortala{\r\n            width:30%;\r\n            margin-left: 200px;\r\n        }\r\n        .conteiner{\r\n            width:50%;            \r\n        }\r\n        .imagePlace{\r\n            float:right;\r\n            width:50%;\r\n            margin-top:-400px;\r\n            margin-right:-10px;\r\n        }\r\n        .tableCustomer {\r\n            padding:10px;\r\n            width: 100%;\r\n            border: 0.6px rgba(0, 0, 0, 0.5) solid;\r\n            font-size: 11px;\r\n        }\r\n        .tableCustomer tr td:nth-child(1) {\r\n            text-align: right;\r\n        }\r\n        .tableCustomer tr td {\r\n            padding:2px;\r\n        }\r\n        .tableCustomer td:nth-child(3){\r\n            padding-top:10px;\r\n        }\r\n        .tableContainer{\r\n            margin-bottom: 5px;\r\n        }\r\n        .tableRef{            \r\n            padding: 0;\r\n            margin:20px 0;\r\n            width:265px;\r\n            border: 0.6px solid rgba(0, 0, 0, 0.5);\r\n            border-collapse: collapse;\r\n            color: red;\r\n        }\r\n        .tableRef td {\r\n            font-size: 10px;\r\n            border: 0.6px solid rgba(0, 0, 0, 0.5);\r\n        }\r\n        .baslik{\r\n            color:red;\r\n            margin-left:100px;\r\n        }\r\n        .totals{\r\n            margin-left: 365px;\r\n        }\r\n        .tableProducts td:nth-child(6) {\r\n            text-align: right;\r\n        }\r\n        .tableProducts td:nth-child(7) {\r\n            text-align:right;\r\n        }\r\n        .totalsTable{\r\n           padding:10px;\r\n        }\r\n        .totalsTable td:nth-child(1){\r\n           padding-bottom: 20px;\r\n        }\r\n        .totalsTable td:nth-child(2) {\r\n            padding-left: 50px;\r\n            padding-bottom:20px;\r\n            text-align: right;\r\n        }\r\n        .grandT{\r\n            margin-left: 325px;\r\n            margin-top:60px;\r\n        }\r\n        .grandTotalsTable td:nth-child(1) {\r\n            padding-right: 20px;\r\n        }\r\n        .grandTotalsTable td:nth-child(2) {\r\n            padding-left: 50px;\r\n            text-align: right;\r\n            padding-right:5px;\r\n            border:0.6px solid black;\r\n        }\r\n        .grandTotalsTable{\r\n            border-spacing:20px;\r\n        }\r\n    </style>\r\n</head>\r\n";
            html += "<body>\r\n" +
                "    <div class=\"mainContain\">\r\n" +
                "        <div class=\"conteiner\">\r\n" +
                "            <h2 class=\"baslik\">QUOTAION</h2>\r\n" +
                "            <div class=\"tableContainer\">\r\n" +
                "                <table class=\"tableCustomer bg-info\">\r\n" +
                "                    <tr>\r\n" +
                "                        <td></td>\r\n" +
                "                        <td></td>\r\n";
            html +=
                "                        <td>" + pdfDto.Customer.CustomerName + "</td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>TO</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.To + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>ATTN</b></td>\r\n                        <td>: </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.Attn + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>VESSEL</b></td>\r\n                        <td><b>: </b> </td>\r\n ";
            html +=
                "                       <td><p>" + pdfDto.Customer.Vessel + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>DELİVERY PORT</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.DeliveryPort + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>REF</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.Ref + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>CUR</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.Cur + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>DISCOUNT (%)</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.Discount + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>PAYMENT TERMS</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.PaymentTerms + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>DELIVERY CHARGES</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.DeliveryCharges + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>BOAT SERVİCE FEE</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.BoatServiceFee + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>FREIGHT COST</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.FreightCost + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n " +
                "                       <td><b>CUSTOMS COST</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.CustomsCost + "</p></td>\r\n                    </tr>\r\n                    <tr>\r\n" +
                "                        <td><b>INCOTERMS</b></td>\r\n                        <td><b>: </b> </td>\r\n";
            html +=
                "                        <td><p>" + pdfDto.Customer.Incoterms + "</p></td>\r\n                    </tr>\r\n                </table>\r\n            </div>\r\n        </div>\r\n" +
                "        <div class=\"imagePlace\">\r\n            \r\n";
            html +=
                "           <img src=\"" + pdfDto.Customer.PhotoPath + "\" width=\"400\" height=\"230\" />\r\n        </div>\r\n    </div>";
            int sayac = 0;
            
            
                    foreach (var item in pdfDto.Products) {
                html +=
            "        <div class=\"ortala\">\r\n" +
            "            <table class=\"tableRef bg-info\">\r\n" +
            "                    <tr>\r\n" +
            "                        <td><h5>Req. NO: " + item.RefNo + "</h5></td>\r\n" +
            "                     </tr >"+
            "                     <tr >"+
            "                        <td><h5>P.O. NO: " + item.PONO + "</h5></td>\r\n" +
            "                    </tr>\r\n" +
            "            </table>\r\n" +
            "        </div>\r\n" +
            "        <table class=\"table tableProducts table-bordered\">\r\n" +
            "            <thead>\r\n" +
            "                <tr>\r\n" +
            "                    <th></th>\r\n" +
            "                    <th colspan=\"2\"></th>\r\n" +
            "                    <th colspan=\"3\">REQUIRED</th>\r\n" +
            "                    <th></th>\r\n" +
            "                    <th class=\"bg-info\" colspan=\"3\">ALTERNATİVE PACKAGING</th>\r\n" +
            "                    <th class=\"bg-info text-danger\">REMARKS</th>\r\n" +
            "                </tr>\r\n" +
            "                <tr>\r\n " +
            "                   <th>NO</th>\r\n" +
            "                    <th>Code</th>\r\n" +
            "                    <th>Description</th>\r\n" +
            "                    <th>Qnty</th>\r\n" +
            "                    <th>Unit</th>\r\n" +
            "                    <th>Price</th>\r\n" +
            "                    <th class=\"bg-warning bg-opacity-25\">Total</th>\r\n" +
            "                    <th class=\"bg-info\">Qnty</th>\r\n" +
            "                    <th class=\"bg-info\">Unit</th>\r\n" +
            "                    <th class=\"bg-info\">Price</th>\r\n" +
            "                    <th class=\"bg-info\"></th>\r\n" +
            "                </tr>" +
            "               </thead>\r\n" +
            "            <tbody>";
                
               foreach (var answers in pdfDto.ProductsAnswers) {                        
                                   if (answers[sayac].ProductId==item.ProductId) {                        
                                        foreach (var answer in answers) {
                                            html += "<tr>\r\n" +
                        "                                <td>" + answer.No + "</td>\r\n" +
                        "                                <td>" + answer.Code + "</td>\r\n" +
                        "                                <td>" + answer.Description + "</td>\r\n" +
                        "                                <td>" + answer.Qnty + "</td>\r\n" +
                        "                                <td>" + answer.Unit + "</td>\r\n" +
                        "                                <td>" + answer.Price + "</td>\r\n" +
                        "                                <td class=\"bg-warning bg-opacity-25\">" + answer.total + "</td>\r\n" +
                        "                                <td class=\"bg-info\">" + answer.AlterNativeQnty + "</td>\r\n" +
                        "                                <td class=\"bg-info\">" + answer.AlterNativeUnit + "</td>\r\n" +
                        "                                <td class=\"bg-info\">" + answer.AlterNativePrice + "</td>\r\n" +
                        "                                <td class=\"bg-info text-danger\">" + answer.Remarks + "</td>\r\n" +
                        "                            </tr>";
                                                }
                                    }
                }
                html +="</tbody>\r\n" +
                    "        </table>\r\n" +
                    "        <div class=\"totals\">\r\n" +
                    "            <table class=\"totalsTable\">\r\n" +
                    "                <tr>\r\n" +
                    "                    <td><b>SUBTOTAL :</b></td>\r\n" +
                    "                    <td>" + item.SubTotal + "</td>\r\n" +
                    "                </tr>\r\n" +
                    "                <tr>\r\n" +
                    "                    <td><b>TOTAL :</b></td>\r\n" +
                    "                    <td>" + item.AllTotal + "</td>\r\n" +
                    "                </tr>\r\n" +
                    "            </table>\r\n" +
                    "        </div>   ";
                 }
                        html +=
                "    <div class=\" grandT\">\r\n" +
                "        <table class=\"grandTotalsTable\">\r\n" +
                "            <tr>\r\n" +
                "                <td><b>GRAND TOTAL :</b></td>\r\n" +
                "                <td class=\"bg-info\">"+pdfDto.Customer.GrandTotal+"</td>\r\n" +
                "            </tr>\r\n" +
                "        </table>\r\n" +
                "    </div>\r\n" +
                "    </body>" +
                "</html>";
            //kütüphanenin renderer objesini newliyorum
            var Renderer = new ChromePdfRenderer();
            //stringe dönüştürdüğüm html kodlarını pdf dönüştürmesi için renderer'e veriyorum ve pdfDocument oluşturuyor.
            var pdf = Renderer.RenderHtmlAsPdf(html);
            //oluşan PdfDocumentin binery datasını bir byte arrayine eşiliyorum
            byte[] byteFormat = pdf.BinaryData;
            //eşitlediğim byte arrayını kullanıcıya gösteriyorum.
            return File(byteFormat, "application/pdf");
        }
    }
}
