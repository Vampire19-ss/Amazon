using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models.DomainModels;
using WebApplication1.Models.JunctionModels;
using WebApplication1.Models.ViewModels;
using WebApplication1.Types;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {

        private readonly sqlDbcontext dbContext;
        private readonly ITokenService tokenService;

        public ProductController(sqlDbcontext dbContext, ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
        }



        // GET: ProductController //we will show products here
        [HttpGet]
        public async Task<ActionResult> Index(ProductCategory category)//why idexasync here
        {
            try
            {

                if (category == ProductCategory.General)
                {
                    var products = await dbContext.Products.Where(p => p.IsActive == true).ToListAsync();
                    if (products == null || products.Count == 0)
                    {

                        ViewBag.Message = "No products Availble";
                        ViewBag.Category = category;
                        return View();
                    }
                    else
                    {
                        var viewModel = new ProductView
                        {
                            Products = products
                        };
                        ViewBag.Category = category;
                        return View(viewModel);

                    }

                }
                else
                {
                    var products = await dbContext.Products.Where(p => p.IsActive == true && p.Category == category).ToListAsync();
                    if (products == null || products.Count == 0)
                    {

                        ViewBag.Message = "No products Availble";
                        ViewBag.Category = category;
                        return View();
                    }
                    else
                    {
                        var viewModel = new ProductView
                        {
                            Products = products
                        };
                        ViewBag.Category = category;
                        return View(viewModel);

                    }
                }


            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
                throw;
            }
        }



        [HttpGet]
        public async Task<IActionResult> Details(Guid ProductId)
        {

            try
            {

                var product = await dbContext.Products.FindAsync(ProductId);


                var viewModel = new ProductView
                {
                    Product = product

                };

                return View(viewModel);    //empty view // after passing the view model view now has data 


            }
            catch (System.Exception ex)
            {

                ViewBag.ErrorMessage = ex.Message;
                return View("Error");

            }
        }


        //wrong controller
        /*[Authorize]//////////////////////////////////////////////////////////////////////
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid ProductId, string? Color, int Quantity, string? Size)
        {
            try
            {
                 var token = Request.Cookies["GradSchoolAuthToken"];
                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "User");


                var userId = tokenService.VerifyTokenAndGetId(token);
                if (userId == Guid.Empty)
                    return RedirectToAction("Login", "User");

                Guid? userId = HttpContext.Items["UserId"] as Guid?;
              

                if (Guid.Empty == userId)
                {
                    ViewBag.ErrorMessage = "UserId not Found  ppppp";
                    return View("error");
                }


                var product = await dbContext.Products.FindAsync(ProductId);
                var cart = await dbContext.Carts.FirstOrDefaultAsync(c => c.Userid == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        Userid = (Guid)userId,
                        CartTotal = 0,
                        CartProducts = []
                    };
                    await dbContext.Carts.AddAsync(cart);

                    ViewBag.ErrorMessage = "UserId not Found ";
                    return View("error");
                }


                var existingItem = await dbContext.CartProducts
                    .FirstOrDefaultAsync(cp => cp.CartId == cart.CartId
                     && cp.ProductId == ProductId
                      && cp.Color == Color
                       && cp.Size == Size);//one question here

                if (existingItem == null)
                {
                    var cartItem = new CartProduct
                    {
                        CartId = cart.CartId,
                        ProductId = ProductId,
                        Quantity = Quantity,
                        Color = Color,
                        Size = Size
                    };
                    await dbContext.CartProducts.AddAsync(cartItem);

                }
                else
                {
                    existingItem.Quantity += Quantity;

                }
                cart.CartTotal += Quantity * product.ProductPrice;
                await dbContext.SaveChangesAsync();


                return RedirectToAction("Cart", "User");
                ///all done

            }
            catch (System.Exception ex)
            {

                ViewBag.ErrorMessage = ex.Message;
                return View("Error");

            }

        }*/


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid ProductId, string? Color, int Quantity, string? Size)
        {
            try
            {

                if (HttpContext.Items["UserId"] is not Guid userId)
                {
                    TempData["ErrorMessage"] = "User not Found!!!!!!!!!!";
                    return RedirectToAction("Login", "User");
                }

                var product = await dbContext.Products.FindAsync(ProductId);

                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product Not Found!";
                    return RedirectToAction("Cart", "User");
                }



                var cart = await dbContext.Carts.FirstOrDefaultAsync(c => c.Userid == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        Userid = (Guid)userId,
                        CartTotal = 0,
                        CartProducts = []
                    };
                    await dbContext.Carts.AddAsync(cart);
                }


                var existingItem = await dbContext.CartProducts
                                                   .FirstOrDefaultAsync(cp => cp.CartId == cart.CartId && cp.ProductId == ProductId);

                if (existingItem == null)
                {
                    var cartProduct = new CartProduct
                    {
                        CartId = cart.CartId,
                        ProductId = ProductId,
                        Quantity = Quantity,
                        Color = Color,
                        Size = Size
                    };
                    await dbContext.CartProducts.AddAsync(cartProduct);

                }
                else
                {
                    existingItem.Quantity += Quantity;

                }
                cart.CartTotal += Quantity * product.ProductPrice;
                await dbContext.SaveChangesAsync();


                return RedirectToAction("Cart", "User");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }




    [Authorize]
    [HttpGet]
    public async Task<ActionResult> Removecart(Guid ProductId)
    {
       var existingcartproducts = await dbContext.CartProducts
        .Include(cp => cp.Product) 
        .FirstOrDefaultAsync(a => a.ProductId == ProductId);
        
        if (existingcartproducts == null)
            {
                TempData["ErrorMessage"] = "no cart products";
                return RedirectToAction("cart", "User");
            }

        dbContext.CartProducts.Remove(existingcartproducts);

        var cartid = existingcartproducts.CartId;

        var cart = await dbContext.Carts
            .Include(c => c.CartProducts)
            .ThenInclude(cp => cp.Product)
            .FirstOrDefaultAsync(c => c.CartId == cartid);

        cart.CartTotal = cart.CartTotal - (existingcartproducts.Product.ProductPrice*existingcartproducts.Quantity);

         await dbContext.SaveChangesAsync();

         TempData["SuccessMessage"] = "product removed";

        return RedirectToAction("cart", "User");

    }







    }
}
