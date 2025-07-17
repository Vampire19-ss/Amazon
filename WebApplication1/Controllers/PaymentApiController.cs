using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentApiController : ControllerBase
    {


        private readonly sqlDbcontext dbContext;
        private readonly ITokenService tokenService;
        private readonly RazorPayService razorpayService = new();

        public PaymentApiController(sqlDbcontext dbContext, ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
        }



  



       // api 

        [HttpPost("CreatePaymentIntent")]
        public IActionResult CreatePaymentIntent(PaymentIntentModel request)
        {

            if (Guid.Empty == request.OrderId  || request.Amount <= 0 || string.IsNullOrEmpty(request.Currency))
            {
                return BadRequest("Invalid payment details.");   // status code 400 
            }

            //  call payment gateway to create order // gernarate reciept id === our  orderid
            var order = razorpayService.CreateOrder(request.Amount, request.Currency, request.OrderId);


            if (order == null)
            {
                return StatusCode(500, new { message = "Something Went Wrong!" });
            }

            return Ok(new
            {
                orderId = order["id"].ToString(),
                entity = order["entity"].ToString(),
                amount = order["amount"],
                amountPaid = order["amount_paid"],
                amountDue = order["amount_due"],
                currency = order["currency"].ToString(),
                receipt = order["receipt"].ToString(),
                status = order["status"].ToString(),
                attempts = order["attempts"],
                createdAt = order["created_at"]
            });
        }
    }
}











































/*using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentApiController : ControllerBase
    {


        private readonly sqlDbcontext dbContext;
        private readonly ITokenService tokenService;
        private readonly RazorPayService razorpayService = new();

        public PaymentApiController(sqlDbcontext dbContext, ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
        }

        [HttpPost("CreatePaymentIntent")]
        public IActionResult CreatePaymentIntent([FromBody]PaymentIntentModel request)
        {
            if (Guid.Empty == request.OrderId || request.Amount <= 0 || string.IsNullOrEmpty(request.Currency))
            {
                return BadRequest("Invalid payment details.");   // status code 400 
            }

            //  call payment gateway to create reciept 
            var order = razorpayService.CreateOrder(request.Amount, request.Currency, request.OrderId);


            if (order == null)
            {
                return StatusCode(500, new { message = "Something Went Wrong!" });
            }

            return Ok(new
            {
                orderId = order["id"].ToString(),
                entity = order["entity"].ToString(),
                amount = order["amount"],
                amountPaid = order["amount_paid"],
                amountDue = order["amount_due"],
                currency = order["currency"].ToString(),
                receipt = order["receipt"].ToString(), //database orderid
                status = order["status"].ToString(),
                attempts = order["attempts"],
                createdAt = order["created_at"],
            });

        }
        

        
    }
}*/
//imp
// compleate todays// twomarrow morning  little bit repeat //confusion clear with time