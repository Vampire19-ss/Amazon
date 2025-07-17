using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Models.DomainModels;
using WebApplication1.Models.JunctionModels;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {

        private readonly sqlDbcontext dbContext;    // encapsulated feilds
        private readonly ITokenService tokenService;

        private readonly IMailService mailService;

        public OrderController(sqlDbcontext dbContext, ITokenService tokenService, IMailService mailService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
            this.mailService = mailService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> CheckOut(Guid CartId)
        {
            Guid? userId = HttpContext.Items["UserId"] as Guid?;
            if (userId == null)
            {
                ViewBag.ErrorMessage = "No user Found!";
                return RedirectToAction("login","user");
            }

            var address = await dbContext.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);
            


            var cart = await dbContext.Carts.FindAsync(CartId);


            if (cart == null)
            {
                ViewBag.ErrorMessage = "No Cart Found!";
                return View();
            }

            var cartProducts = await dbContext.CartProducts.Include(cp => cp.Product).Where(cp => cp.CartId == cart.CartId).ToListAsync(); //understud

            var viewmodel = new CartView//three cread opprations
            {

                Cart = cart,//iss cart ka bill fataga oor be kaam ha isska
                Address = address,
                products = cartProducts

            };

            return View(viewmodel);

        }








        [HttpGet]
        public async Task<ActionResult> Create(Guid CartId)
        {
            try
            {

                Guid? userId = HttpContext.Items["UserId"] as Guid?;
                  var user = await dbContext.Users.FindAsync(userId);
                

                var address = await dbContext.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

                var cart = await dbContext.Carts
                    .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                    .FirstOrDefaultAsync(c => c.CartId == CartId);

                if (cart != null && address != null && cart.CartProducts?.Count > 0)
                {
                    var order = new Order
                    {
                        UserId = (Guid)userId,
                        TotalPrice = cart.CartTotal,
                        AddressId = address.AddressId,
                        OrderProducts = [],
                        OrderStatus = Types.OrderStatus.Pending
                    };

                    var orderProducts = cart.CartProducts.Select(cp => new OrderProduct//here is confusion little bit
                    {
                        OrderId = order.OrderId,
                        ProductId = cp.ProductId,
                        Quantity = cp.Quantity,
                        Size = cp.Size,
                        Color = cp.Color,
                        Weight = cp.Weight,
                    }).ToList();

                    order.OrderProducts = orderProducts;
                    await dbContext.Orders.AddAsync(order);

                    //await mailService.SendEmailAsync(user.Email, "Order Succesfull", $"Your Order of Rs {order.TotalPrice} has been created ", true);
                    if (cart?.CartProducts != null)

                    {
                        dbContext.CartProducts.RemoveRange(cart.CartProducts);
                        cart.CartTotal = 0;
                    }

                    await dbContext.SaveChangesAsync();

                    await mailService.SendEmailAsync(user?.Email, "Order Succesfull", $"Your Order of Rs {order.TotalPrice} has been created ", true);

                    
                    return RedirectToAction("Payment", "Order", new { order.OrderId });

                }
                else
                {
                    TempData["ErrorMessage"] = "Some Error with Cart! or no Adress .Try again!";
                    return RedirectToAction("checkout", new { CartId });

                }

            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();

            }

        }


        [HttpGet]
        public async Task<ActionResult> Payment(Guid OrderId)
        {

            try
            {
                var order = await dbContext.Orders.FindAsync(OrderId);

                if (order == null)
                {
                    ViewBag.ErrorMessage = "No order Found!";
                    return View();
                }

                var viewModel = new OrderView
                {
                    Order = order
                };

                return View(viewModel);
            }
            catch (System.Exception ex)
            {

                ViewBag.ErrorMessage = ex.Message;
                return View();
            }


        }



        [HttpGet]
        public async Task<ActionResult> PaymentSuccess(Guid OrderId)//here is error
        {
            try
            {
                var order = await dbContext.Orders.FindAsync(OrderId);
                var user = await dbContext.Users.FindAsync(OrderId=OrderId);


                if (order == null)
                {
                    ViewBag.ErrorMessage = "No order Found!";
                    return View();
                }
                order.PaymentStatus = Types.PaymentStatus.Succesfull;
                order.OrderStatus = Types.OrderStatus.confirmed;
                order.PaymentMode = Types.PaymentMode.RazorPay;
                await dbContext.SaveChangesAsync();

               //here i use
               
                //await mailService.SendEmailAsync("www.jack8082@gmail.com", "Order Succesfull", $"Your Order of Rs {order.TotalPrice} has been created ", true);
                
                //TempData["SuccessMessage"] = "Your order Placed Succesfully!";
                // return RedirectToAction("Orders", "User", new { order?.UserId });


                return View();

            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }



        //how to get PaymentFailure controller not definet payment.cs
        [HttpGet]
        public async Task<ActionResult> PaymentFailure(Guid OrderId)
        {
            try
            {
                var order = await dbContext.Orders.FindAsync(OrderId);

                if (order == null)
                {
                    ViewBag.ErrorMessage = "No order Found!";
                    return View();
                }
                order.PaymentStatus = Types.PaymentStatus.Error;
                order.OrderStatus = Types.OrderStatus.Pending;
                order.PaymentMode = Types.PaymentMode.RazorPay;

                await dbContext.SaveChangesAsync();

                TempData["ErrorMessage"] = "Some Error in the Payment gateway !";

                return RedirectToAction("Orders", "Users", new { order?.UserId });

            }
            catch (System.Exception ex)
            {

                ViewBag.ErrorMessage = ex.Message;
                return View();
            }


        }




        [HttpGet]
        public async Task<ActionResult> Recent()
        {
            if (HttpContext.Items["UserId"] is not Guid userId)
            {
                TempData["ErrorMessage"] = "User not Found!!!!!!!!!!";
                return RedirectToAction("Login", "User");
            }
            var Orders = await dbContext.Orders.Where(O => O.UserId == userId).ToListAsync();
            if (Orders == null)
            {
                ViewBag.ErrorMessage = "no recent orders";
                return View();
            }
            var viewModel = new OrderView
            {
                Orders=Orders
            };

            return View(viewModel);
        }


       



    }

}






































































































































/*namespace WebApplication1.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        // GET: OrderController


        // GET: OrderController

        private readonly sqlDbcontext dbContext;    // encapsulated feilds
        private readonly ITokenService tokenService;
        //private readonly IMailService mailService;

        public OrderController(sqlDbcontext dbContext, ITokenService tokenService )
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
           // this.mailService = mailService;

        }

       /* [Authorize]
        [HttpGet]
        public async Task<ActionResult> CheckOut(Guid CartId)
        {
            Guid? userId = HttpContext.Items["UserId"] as Guid?;
            if (userId == null)
            {
                ViewBag.ErrorMessage = "No user Found!";
                return RedirectToAction("login","user");
            }

            var address = await dbContext.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);
            if (address == null)
            {
                ViewBag.ErrorMessage = "No adress Found!";
                return RedirectToAction("login","user");
            }


            var cart = await dbContext.Carts.FindAsync(CartId);


            if (cart == null)
            {
                ViewBag.ErrorMessage = "No Cart Found!";
                return View();
            }

            var cartProducts = await dbContext.CartProducts.Include(cp => cp.Product).Where(cp => cp.CartId == cart.CartId).ToListAsync(); //understud

            var viewmodel = new CartView//three cread opprations
            {

                Cart = cart,//iss cart ka bill fataga oor be kaam ha isska
                Address = address,
                products = cartProducts

            };

            return View(viewmodel);

        }








        [HttpGet]
        public async Task<ActionResult> Create(Guid CartId)
        {
            try
            {

                Guid? userId = HttpContext.Items["UserId"] as Guid?;
                  var user = await dbContext.Users.FindAsync(userId);
                

                var address = await dbContext.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

                var cart = await dbContext.Carts
                    .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                    .FirstOrDefaultAsync(c => c.CartId == CartId);

                if (cart != null && address != null && cart.CartProducts?.Count > 0)
                {
                    var order = new Order
                    {
                        UserId = (Guid)userId,
                        TotalPrice = cart.CartTotal,
                        AddressId = address.AddressId,
                        OrderProducts = [],
                        OrderStatus = Types.OrderStatus.Pending
                    };

                    var orderProducts = cart.CartProducts.Select(cp => new OrderProduct//here is confusion little bit
                    {
                        OrderId = order.OrderId,
                        ProductId = cp.ProductId,
                        Quantity = cp.Quantity,
                        Size = cp.Size,
                        Color = cp.Color,
                        Weight = cp.Weight,
                    }).ToList();

                    order.OrderProducts = orderProducts;
                    await dbContext.Orders.AddAsync(order);

                    if (cart?.CartProducts != null)
                    {
                        dbContext.CartProducts.RemoveRange(cart.CartProducts);
                        cart.CartTotal = 0;
                    }

                    await dbContext.SaveChangesAsync();

                 // await mailService.SendEmailAsync(user.Email, "Order Succesfull", $"Your Order of Rs {order.TotalPrice} has been created ", true);

                    
                    return RedirectToAction("Payment", "Order", new { order.OrderId });

                }
                else
                {
                    TempData["ErrorMessage"] = "Some Error with Cart!.Try again!";
                    return RedirectToAction("checkout", new { CartId });

                }

            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();

            }

        }


        [HttpGet]
        public async Task<ActionResult> Payment(Guid OrderId)
        {

            try
            {
                var order = await dbContext.Orders.FindAsync(OrderId);

                if (order == null)
                {
                    ViewBag.ErrorMessage = "No order Found!";
                    return View();
                }

                var viewModel = new OrderView
                {
                    Order = order
                };

                return View(viewModel);
            }
            catch (System.Exception ex)
            {

                ViewBag.ErrorMessage = ex.Message;
                return View();
            }


        }



        [HttpGet]
        public async Task<ActionResult> PaymentSuccess(Guid OrderId)//here is error
        {
            try
            {
                var order = await dbContext.Orders.FindAsync(OrderId);
                //var user = await dbContext.Users.FindAsync(OrderId=OrderId);


                if (order == null)
                {
                    ViewBag.ErrorMessage = "No order Found!";
                    return View();
                }
                order.PaymentStatus = Types.PaymentStatus.Succesfull;
                order.OrderStatus = Types.OrderStatus.confirmed;
                order.PaymentMode = Types.PaymentMode.RazorPay;
                await dbContext.SaveChangesAsync();

               //here i use
               
                //await mailService.SendEmailAsync(user.Email, "Order Succesfull", $"Your Order of Rs {order.TotalPrice} has been created ", true);
                
                TempData["SuccessMessage"] = "Your order Placed Succesfully!";
                // return RedirectToAction("Orders", "User", new { order?.UserId });


                return View();

            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }



        //how to get PaymentFailure controller not definet payment.cs
        [HttpGet]
        public async Task<ActionResult> PaymentFailure(Guid OrderId)
        {
            try
            {
                var order = await dbContext.Orders.FindAsync(OrderId);

                if (order == null)
                {
                    ViewBag.ErrorMessage = "No order Found!";
                    return View();
                }
                order.PaymentStatus = Types.PaymentStatus.Error;
                order.OrderStatus = Types.OrderStatus.Pending;
                order.PaymentMode = Types.PaymentMode.RazorPay;

                await dbContext.SaveChangesAsync();

                TempData["ErrorMessage"] = "Some Error in the Payment gateway !";

                return RedirectToAction("Orders", "Users", new { order?.UserId });

            }
            catch (System.Exception ex)
            {

                ViewBag.ErrorMessage = ex.Message;
                return View();
            }


        }




        [HttpGet]
        public async Task<ActionResult> Recent()
        {
            if (HttpContext.Items["UserId"] is not Guid userId)
            {
                TempData["ErrorMessage"] = "User not Found!!!!!!!!!!";
                return RedirectToAction("Login", "User");
            }
            var Orders = await dbContext.Orders.Where(O => O.UserId == userId).ToListAsync();
            if (Orders == null)
            {
                ViewBag.ErrorMessage = "no recent orders";
                return View();
            }
            var viewModel = new OrderView
            {
                Orders=Orders
            };

            return View(viewModel);
        }







    }
}*/





// twmarrow is important   services
