using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using WebApplication1.Data;
using WebApplication1.Interfaces;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthentication();    // policy add in future

//dependency enjection
builder.Services.AddSingleton<ITokenService ,TokenService>(); //is here instance of tokenservice is created 
//dependency enjection
builder.Services.AddSingleton<ICloudinaryService, CloudinaryService>();

 builder.Services.AddSingleton<IMailService , EmailService>();
 builder.Services.AddScoped<RazorPayService>();
//connects to the database
builder.Services.AddDbContext<sqlDbcontext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("local")));//where did all thest classes instated is my main question





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseExceptionHandler("/Home/Error");




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication(); //it helps us to use cookies line // ← Must come before UseAuthorization()//Enables the system to actually read and validate auth cookies //Makes HttpContext.User work Turns cookie data into a logged-in user identity

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();


