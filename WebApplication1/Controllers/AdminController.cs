using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.DomainModels;
using WebApplication1.Types;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly ITokenService tokenService;
        private readonly sqlDbcontext dbContext;
        private readonly ICloudinaryService cloudinaryService;

        public AdminController(ITokenService tokenService, sqlDbcontext dbContext, ICloudinaryService cloudinaryService)
        {
            this.tokenService = tokenService;
            this.dbContext = dbContext;
            this.cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Dashboard()
        {
            try
            {
                Guid? userId = HttpContext.Items["UserId"] as Guid?;
                var user = dbContext.Users.FirstOrDefault(u => u.Userid == userId);

                if (user == null)
                {
                    return RedirectToAction("Index", "Product");
                    //=>main q. :-//it is temporary bcz the only question i asked to sir
                    // // redirect to return Url 
                    //  HttpContext.Session.Remove("ReturnUrl");
                }

                if (user.Role == Role.User)
                {
                    return RedirectToAction("Index", "Product");
                }
                else if (user.Role == Role.StoreKeeper)
                {
                    return RedirectToAction("Index", "Product");
                }
                else if (user.Role == Role.Admin)
                {
                    return View();
                }

                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View("Error");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult CreateProduct()
        {
            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Createproduct(Product req, IEnumerable<IFormFile> image)
        {

            try
            {
                if (image == null || !image.Any())
                {
                    ViewBag.ErrorMessage = "Kindly Select the image File";
                    return View();
                }


                var secureUrls = await cloudinaryService.UploadmultipleImageAsync(image);


                if (string.IsNullOrEmpty(req.ProductName) ||
                string.IsNullOrEmpty(req.ProductDescription))

                {
                    ViewBag.ErrorMessage = "All details with * are required";
                    return View();
                }

                req.ProductImage = string.Join(",", secureUrls);

                await dbContext.Products.AddAsync(req);
                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product created SuccesFully!";

                // return View();
                return RedirectToAction("Index", "Product");

            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
                throw;
            }

        }

        [Authorize]
        [HttpGet]
        public ActionResult CreateBlog()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateBlog(Createblog22 blog, IEnumerable<IFormFile> image)
        {
            try
            {
                if (blog == null)
                {
                    ViewBag.ErrorMessage = "Blog data is missing.";
                    return View("CreateBlog");
                }

                if (image == null || !image.Any())
                {
                    ViewBag.ErrorMessage = "Please select at least one image file.";
                    return View("CreateBlog", blog);
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "All fields marked with * are required.";
                    return View("CreateBlog", blog);
                }

                var secureUrls = await cloudinaryService.UploadmultipleImageAsync(image);
                if (secureUrls == null || !secureUrls.Any())
                {
                    ViewBag.ErrorMessage = "Failed to upload images to Cloudinary.";
                    return View("CreateBlog", blog);
                }

                blog.BlogImage = string.Join(",", secureUrls);

                await dbContext.Createblogs22.AddAsync(blog);
                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Blog created successfully!";
                return RedirectToAction("BlogList", "Admin");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBlog: {ex.Message}\nStackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "An error occurred while creating the blog.";
                return View("Error");
            }
        }




        [Authorize]
        [HttpGet]
        public ActionResult BlogList()
        {
            var blogs = dbContext.Createblogs22.ToList();

            if (blogs == null || !blogs.Any())
            {
                TempData["Message"] = "No blogs found. Create a new blog.";
                return View("BlogList", new List<Createblog22>()); // Return empty list instead of redirecting
            }

            var viewModel = blogs.Select(item => new Createblog22
            {
                BlogId = item.BlogId,
                Title = item.Title,
                Description = item.Description,
                Shotdescription = item.Shotdescription,
                BlogImage = item.BlogImage
            }).ToList();

            return View("BlogList", viewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> EditBlog(Guid blogId)
        {
            if (blogId == Guid.Empty)
            {
                ViewBag.ErrorMessage = "Invalid blog ID.";
                return View("EditBlog", new Createblog22());
            }

            var existingBlog = await dbContext.Createblogs22.FirstOrDefaultAsync(b => b.BlogId == blogId);
            if (existingBlog == null)
            {
                ViewBag.ErrorMessage = "No blog found for the specified ID.";
                return View("EditBlog", new Createblog22());
            }

            var viewModel = new Createblog22
            {
                Title = existingBlog.Title ?? string.Empty,
                Shotdescription = existingBlog.Shotdescription ?? string.Empty,
                Description = existingBlog.Description ?? string.Empty,
                BlogImage = existingBlog.BlogImage ?? string.Empty,
                BlogId = existingBlog.BlogId
            };

            return View("EditBlog", viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> GetEditblog(Createblog22 blog, IFormFile[] image)
        {
            try
            {
                if (blog == null || blog.BlogId == Guid.Empty)
                {
                    ViewBag.ErrorMessage = "Invalid blog ID.";
                    return View("EditBlog", blog ?? new Createblog22());
                }

                var existingBlog = await dbContext.Createblogs22.FirstOrDefaultAsync(b => b.BlogId == blog.BlogId);
                if (existingBlog == null)
                {
                    ViewBag.ErrorMessage = "No blog found for the specified ID.";
                    return View("EditBlog", blog);
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "All fields marked with * are required.";
                    return View("EditBlog", blog);
                }

                if (image != null && image.Any())
                {
                    var secureUrls = await cloudinaryService.UploadmultipleImageAsync(image);
                    if (secureUrls == null || !secureUrls.Any())
                    {
                        ViewBag.ErrorMessage = "Failed to upload images to Cloudinary.";
                        return View("EditBlog", blog);
                    }
                    existingBlog.BlogImage = string.Join(",", secureUrls);
                }

                existingBlog.Title = blog.Title;
                existingBlog.Shotdescription = blog.Shotdescription;
                existingBlog.Description = blog.Description;

                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Blog updated successfully!";
                return RedirectToAction("BlogList", "Admin");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditBlog: {ex.Message}\nStackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "An error occurred while updating the blog.";
                return View("EditBlog", blog);
            }
        }




        [Authorize]
        [HttpGet]
        public IActionResult Getsupport()
        {

            var supports = dbContext.supports.ToList();


            if (supports == null || !supports.Any())
            {

                return RedirectToAction("Support", "User");
            }


            var viewModel = supports.Select(item => new Support
            {
                SupportId = item.SupportId,
                Name = item.Name,
                Description = item.Description,
                Phone = item.Phone,
                SupportImage = item.SupportImage
            }).ToList();

            return View(viewModel);
        }


        /* if (blogid == Guid.Empty)
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

             return View(viewModel);*/



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Supportdetails(Guid supportid)
        {



            if (supportid == Guid.Empty)
            {
                TempData["SuccessMessage"] = "no supports";
                return View();
            }

            var existingsupport = await dbContext.supports.FirstOrDefaultAsync(b => b.SupportId == supportid);

            if (existingsupport == null)
            {

                TempData["SuccessMessage"] = "no suport found";
                return View();
            }

            var viewModel = new Support
            {
                Name = existingsupport.Name ?? string.Empty,
                SupportImage = existingsupport.SupportImage ?? string.Empty,
                Description = existingsupport.Description ?? string.Empty,
                Phone = existingsupport.Phone,
            };

            return View(viewModel);

        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Deletesupport(Guid supportid)
        {

            if (supportid == Guid.Empty)
            {
                TempData["SuccessMessage"] = "no supports";
                return RedirectToAction("Getsupport", "Admin");
            }

            var support = await dbContext.supports.FirstOrDefaultAsync(b => b.SupportId == supportid);
            dbContext.supports.Remove(support);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "support Deleted";
            return RedirectToAction("Getsupport", "Admin");
        }


        [Authorize]
        [HttpGet]
        public ActionResult CreateSubscripation()
        {
            return View();
        }


       
        [HttpPost]
        public async Task<ActionResult> CreateSubscripation(CreateSubscriptionss subscription)
        {

            if (subscription.CreateSubscriptionName == null || subscription.Description == null )
            {
                ViewBag.ErrorMessage = "All fields are required";
                return View();
            }
            await dbContext.CreateSubscriptionss.AddAsync(subscription);
            await dbContext.SaveChangesAsync();
            ViewBag.SuccessMessage = "subescreaption created secessfully";

            return View();
        }


    }
}
