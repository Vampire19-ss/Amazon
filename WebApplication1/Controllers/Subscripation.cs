using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models.DomainModels;
using WebApplication1.Services;
using WebApplication1.Types;

namespace WebApplication1.Controllers
{

    public class Subscripation : Controller
    {
        private readonly sqlDbcontext dbContext;
        private readonly ITokenService tokenService;
        private readonly RazorPayService razorpayService = new();
        public Subscripation(sqlDbcontext dbContext, ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
        }



        /* [HttpGet]
         public async Task<ActionResult> CreateSub(Guid id)
         {
             Guid? userId = HttpContext.Items["UserId"] as Guid?;
             if (userId == null)
             {
                 ViewBag.ErrorMessage = "No user Found!";
                 return RedirectToAction("login", "user");
             }
             var subscription = await dbContext.CreateSubscriptions.FirstOrDefaultAsync(a => a.id == id);
             if (subscription == null)
             {
                 ViewBag.ErrorMessage = "Subscription not found.";
                 return View("Error");
             }
             subscription.subescripetion = Subescripetion.Subescribed;
             ViewBag.ErrorMessage = "subescripation created!";
             return RedirectToAction("Subscriptions", "user");
         }



        [Authorize]
        [HttpGet]
        public async Task<ActionResult> CreateSub(Guid id)
        {

            Guid? userId = HttpContext.Items["UserId"] as Guid?;
            if (userId == null)
            {
                TempData["ErrorMessage"] = "No user found!";
                return RedirectToAction("Login", "User");
            }
            //var user = await dbContext.Users.FirstOrDefaultAsync(u=>u.Userid==userId);
            //var createsub = await dbContext.CreateSubscriptionss.FirstOrDefaultAsync(i=>i.Id==id);

            var viewModel = new Subscription
            {

                UserId = (Guid)userId,
                subescripetion = Subescripetion.Subescribed,
                //Subid=createsub.Id,
                Total = 0,

            };

            try
            {
                await dbContext.Subscriptions.AddAsync(viewModel);
                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Subscription created!";
                return RedirectToAction("Subscriptions", "User");


            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the subscription.";
                return View("Error"); // Fallback to Error view
            }

        }*/



        [Authorize]
[HttpGet]
public async Task<ActionResult> CreateSub(Guid id)
{
    Console.WriteLine("iddddddddddddd================="+id);
    Guid? userId = HttpContext.Items["UserId"] as Guid?;
    if (!userId.HasValue)
    {
        TempData["ErrorMessage"] = "No user found!";
        return RedirectToAction("Login", "User");
    }

    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Userid == userId.Value);
    if (user == null)
    {
        TempData["ErrorMessage"] = "User not found!";
        return RedirectToAction("Login", "User");
    }

    var createsub = await dbContext.CreateSubscriptionss.FirstOrDefaultAsync();
            //var existingItem = await dbContext.CreateSubscriptionss.AddAsync(id);
    if (createsub == null)
            {
                TempData["ErrorMessage"] = "Error please contact site owner!";
                return RedirectToAction("Subscriptions", "User");
            }

    var viewModel = new Subscription
    {
        Id = id, // If required by the model
        UserId = userId.Value,
        subescripetion = Subescripetion.Subescribed,
        Total = 0
    };

    try
    {
        await dbContext.Subscriptions.AddAsync(viewModel);
        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Subscription created!";
        return RedirectToAction("Subscriptions", "User");
    }
    
    catch (Exception ex)
    {
        TempData["ErrorMessage"] = "An unexpected error occurred.";
        // Log: ex.Message
        return RedirectToAction("Subscriptions", "User");
    }
}
        















    }
}







        
 