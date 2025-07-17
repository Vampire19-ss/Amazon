using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.DomainModels;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;
using WebApplication1.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace WebApplication1.Controllers;

public class UserController : Controller
{

    private readonly sqlDbcontext sqlDbcontext;
    private readonly ICloudinaryService cloudinaryService;
    public readonly ITokenService tokenService;
    private readonly IMailService mailService;
    public UserController(sqlDbcontext sqlDbcontext, ITokenService tokenService, ICloudinaryService cloudinaryService, IMailService mailService)// how it works and what is in it where it coms from(is ma aa kya raha ha)
    {
        this.sqlDbcontext = sqlDbcontext;
        this.tokenService = tokenService;
        this.cloudinaryService = cloudinaryService;
        this.mailService = mailService;
    }


    ////////////////////////////////////////////////////////////////////



    //login logic

    [HttpGet]
    public IActionResult login()
    {
        return View();
    }



    [HttpPost]
    public async Task<IActionResult> Login(LoginView modal)

    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.errormassage = "all crindatials are required";
                return View();
            }

            var Existinguser = await sqlDbcontext.Users.FirstOrDefaultAsync(u => u.Email == modal.Email);
            if (Existinguser == null)
            {
                ViewBag.errormassage = "user not found ";
                return View();
            }
            var checkpass = BCrypt.Net.BCrypt.Verify(modal.Password, Existinguser.Password);
            if (checkpass)
            {

                var token = tokenService.CreateToken(Existinguser.Username, Existinguser.Userid, modal.Email, 60 * 24);
                //Console.WriteLine(token);
                HttpContext.Response.Cookies.Append("GradSchoolAuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });

                var returnUrl = HttpContext.Session.GetString("ReturnUrl");
                // token ko cookies may save kerna .... // done 9 april 
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    // redirect to return Url 
                    HttpContext.Session.Remove("ReturnUrl");
                    return Redirect(returnUrl);
                }
            }
            else
            {
                ViewBag.errormassage = "incorect password";
                return View();
            }

        }
        catch (Exception ex)
        {
            ViewBag.errormassage = ex.Message;
            return View("Error");
        }
    }





    //regisror logic

    [HttpGet]
    public IActionResult Registor()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registor(User modal)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.errormassage = "all crindatials are required";
                return View();
            }
            var user = await sqlDbcontext.Users.FirstOrDefaultAsync(u => u.Email == modal.Email);
            if (user != null)
            {
                ViewBag.errormassage = "user already existes ";
                return View();
            }

            var inceipt = BCrypt.Net.BCrypt.HashPassword(modal.Password);
            modal.Password = inceipt;
            await sqlDbcontext.Users.AddAsync(modal);
            await sqlDbcontext.SaveChangesAsync();
            ViewBag.SuccessMessage = "User created secessfully";
            return RedirectToAction("login");

        }
        catch (Exception ex)
        {
            ViewBag.errormassage = ex.Message;
            return View("Error");
        }

    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Cart(User modal)
    {
        try
        {
            /* var token = Request.Cookies["GradSchoolAuthToken"];
             if (string.IsNullOrEmpty(token))
                 return RedirectToAction("Login", "User");


             var userId = tokenService.VerifyTokenAndGetId(token);
             if (userId == Guid.Empty)
                 return RedirectToAction("Login", "User");*/

            Guid? userId = HttpContext.Items["UserId"] as Guid?;

            var Cart = await sqlDbcontext.Carts.FirstOrDefaultAsync(c => c.Userid == userId);

            if (Cart == null)
            {
                ViewBag.ErrorMessage = "Cart Not Found!";
                return View();
            }
            var products = await sqlDbcontext.CartProducts.Include(p => p.Product).Where(cp => cp.CartId == Cart.CartId).ToListAsync();//understand include where cartproduct productid == product.productid
            var viewModel = new CartView
            {
                Cart = Cart,
                products = products
            };


            return View(viewModel);
        }
        catch (System.Exception ex)
        {

            ViewBag.ErrorMessage = ex.Message;
            return View();

        }
    }




    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateAddress(Address address, Guid CartId)
    {



        Guid? userId = HttpContext.Items["UserId"] as Guid?;

        if (!ModelState.IsValid)
        {

            TempData["ErrorMessage"] = "All the  feilds with the * are required ";
            return RedirectToAction("CheckOut", "Order", new { CartId }); //here i understand redirect to action

        }


        var existingAddress = await sqlDbcontext.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);//one to one relationship

        if (existingAddress == null)
        {

            // create 
            address.UserId = (Guid)userId;    // required 
            await sqlDbcontext.Addresses.AddAsync(address);

        }
        else
        {
            // update 

            existingAddress.FirstName = address.FirstName;
            existingAddress.LastName = address.LastName;
            existingAddress.Street = address.Street;
            existingAddress.City = address.City;
            existingAddress.District = address.District;
            existingAddress.State = address.State;
            existingAddress.Country = address.Country;
            existingAddress.Pincode = address.Pincode;
            existingAddress.Phone = address.Phone;
            existingAddress.Landmark = address.Landmark;

        }


        await sqlDbcontext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Address Updated!";
        return RedirectToAction("CheckOut", "Order", new { CartId });

    }//all done here 



    [Authorize]
    [HttpGet]
    public ActionResult Orders(Guid UserId)
    {

        return View();


    }



    [HttpGet]
    public ActionResult Viewblog()
    {

        var blogs = sqlDbcontext.Createblogs22.ToList();
       

        if (blogs == null || !blogs.Any())
        {


            TempData["SuccessMessage"] = " no blog avalible!";
            return RedirectToAction("Index", "Product");
            
           // return RedirectToAction("Createblog", "Admin");
        }


        var viewModel = blogs.Select(item => new Createblog22
        {
            BlogId = item.BlogId,
            Title = item.Title,
            Description = item.Description,
            Shotdescription = item.Shotdescription,
            BlogImage = item.BlogImage
        }).ToList();

        // Return the view with the list of blogs
        return View(viewModel);
    }




    [HttpGet]
    public async Task<ActionResult> Blogdetails(Guid blogid)
    {
        if (blogid == Guid.Empty)
        {
            TempData["SuccessMessage"] = "no blogid";
            return View();
        }

        var existingblog = await sqlDbcontext.Createblogs22.FirstOrDefaultAsync(b => b.BlogId == blogid);

        if (existingblog == null)
        {

            TempData["SuccessMessage"] = "no blog found";
            return View();
        }

        var viewModel = new Createblog22
        {
            Title = existingblog.Title ?? string.Empty,
            Shotdescription = existingblog.Shotdescription ?? string.Empty,
            Description = existingblog.Description ?? string.Empty,
            BlogImage = existingblog.BlogImage ?? string.Empty
        };

        return View(viewModel);
    }


    [HttpGet]
    public IActionResult AboutUs()
    {
        return View();
    }







    [Authorize]
    [HttpGet]
    public IActionResult Support()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Support(Support support, IEnumerable<IFormFile> image)
    {
        try
        {
            if (support == null)
            {
                ViewBag.ErrorMessage = "support data is missing.";
                return View("Support");
            }

            if (image == null || !image.Any())
            {
                ViewBag.ErrorMessage = "Please select at least one image file.";
                return View("Support");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "All fields marked with * are required.";
                return View("Support");
            }

            var secureUrls = await cloudinaryService.UploadmultipleImageAsync(image);
            if (secureUrls == null || !secureUrls.Any())
            {
                ViewBag.ErrorMessage = "Failed to upload images to Cloudinary.";
                return View("Support");
            }

            support.SupportImage = string.Join(",", secureUrls);

            await sqlDbcontext.supports.AddAsync(support);
            await sqlDbcontext.SaveChangesAsync();
            TempData["SuccessMessage"] = " Support created successfully!";
            return RedirectToAction("Index", "Product");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Support: {ex.Message}\nStackTrace: {ex.StackTrace}");
            ViewBag.ErrorMessage = "An error occurred while creating the blog.";
            return View("Error");
        }

    }

    [HttpGet]
    public async Task<ActionResult> Socialmedia()
    {

        return View();
    }





    [HttpGet]
    public async Task<ActionResult> ForgotPassWord()
    {
        try
        {


            return View();
        }
        catch (System.Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }


    [HttpPost]
    public async Task<ActionResult> ForgotPassWord(Forgetview req)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        Console.WriteLine($"Email: {req.Email}");


        var user = await sqlDbcontext.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user == null)
        {
            ViewBag.ErrorMessage = "No user found with that email!";
            return View();
        }


        Random random = new Random();
        int randomNumber = random.Next(100, 999999);
        string otp = randomNumber.ToString();
        user.Otp = otp;
        await sqlDbcontext.SaveChangesAsync();

        Console.WriteLine($"OTP: {user.Otp}");



        await mailService.SendEmailAsync(user.Email, "Forgot Password", $"Your OTP is: {otp}", true);



        return RedirectToAction("VerifyOtp", "User", new { userId = user.Userid.ToString() });
    }


    [HttpGet]
    public async Task<ActionResult> VerifyOtp()
    {

        HttpContext.Session.SetString("UserId", Request.Query["userId"].ToString()); //not understand yet
        var user1 = HttpContext.Session.GetString("UserId");
        // Console.WriteLine("888888="+user1);
        return View();


    }



    [HttpPost]
    public async Task<ActionResult> VerifyOtp(string userId, string password, string otp)
    {
        //var user1 = HttpContext.Request.Query["userId"].FirstOrDefault();

        //HttpContext.Session.SetString("UserId", Request.Query["userId"].ToString());


        // var storedUserId = HttpContext.Session.GetString("UserId");
        // Console.WriteLine("------" + storedUserId);
        userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(otp) || string.IsNullOrEmpty(password))
        {
            ViewBag.ErrorMessage = "All fields are required";

            return View();
        }
        var user = await sqlDbcontext.Users.FirstOrDefaultAsync(u => u.Userid.ToString() == userId.ToString());

        if (user == null)
        {
            ViewBag.ErrorMessage = "Invalid request";
            return View();
        }
        if (user.Otp != otp)
        {
            ViewBag.ErrorMessage = "Invalid or expired OTP";
            return View();
        }
        user.Password = BCrypt.Net.BCrypt.HashPassword(password);
        await sqlDbcontext.SaveChangesAsync();

        return RedirectToAction("Login", "User");


    }

    [HttpGet]
    public ActionResult Press_Releases()
    {
        return View();
    }




    [HttpGet]
    public async Task<ActionResult> Subscriptions()
    {
        var viewmodel = await sqlDbcontext.CreateSubscriptionss
            .Select(item => new CreateSubscription
            {
                CreateSubscriptionName = item.CreateSubscriptionName,
                Description = item.Description,
                Total = item.Total
            })
            .ToListAsync();
        return View(viewmodel);
    }


 /////  => not working
    /*[Authorize]
    [HttpGet]
    public async Task<ActionResult> CreateSub(Guid id)
    {
       
        Guid? userId = HttpContext.Items["UserId"] as Guid?;
        if (userId == null)
        {
            TempData["ErrorMessage"] = "No user found!";
            return RedirectToAction("Login", "User");
        }
        //var subscription = await dbContext.CreateSubscriptions.FirstOrDefaultAsync(a => a.id == id);


        try
        {

            var subscription = await sqlDbcontext.CreateSubscriptions.FirstOrDefaultAsync();
            
             //var subscription = await sqlDbcontext.CreateSubscriptions.FindAsync(id);
            if (subscription == null)
            {
                ViewBag.ErrorMessage = " sub not fount";
                return View();
            }

            //subscription.subescripetion = Subescripetion.Subescribed; // Corrected property and enum
            await sqlDbcontext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Subscription created!";
            return RedirectToAction("Subscriptions", "User");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "An error occurred while creating the subscription.";
            return View("Error"); // Fallback to Error view
        }
            

        }*/



}

















/*<form  method="post"action="@Url.Action("ForgotPassWord", "User")" >
<input type="text" name="otp">
<input type="text" name="password">
<button type="submit"></button>
</form>*/