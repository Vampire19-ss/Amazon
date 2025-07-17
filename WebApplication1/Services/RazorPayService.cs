
using System;
using Razorpay.Api;

namespace WebApplication1.Services;

public class RazorPayService
{

    private readonly string key = "rzp_test_RgG9AwfnysizZu";         // test keys 
    private readonly string secret = "1CaqfrrZdfeKKOIdPypjADfr";       // appsettings.json for security reasons

    public Order? CreateOrder(int amount, string currency, Guid orderId)
    {


        RazorpayClient client = new(key, secret);     // connnected to razor pay client 


        Dictionary<string, object> options = new()
         {
                { "amount", amount * 100 },
                { "currency", currency },
                { "receipt", orderId },
                { "payment_capture", 1 }
            };


        Order order = client.Order.Create(options);
        
        return order;

    }





}





































/*using System;
using Razorpay.Api;

namespace WebApplication1.Services;

public class RazorPayService
{
    private readonly string key = "rzp_test_RgG9AwfnysizZu";         // test keys 
    private readonly string secret = "1CaqfrrZdfeKKOIdPypjADfr";       // appsettings.json for security reasons

    public Order? CreateOrder(int amount, string currency, Guid orderId)
    {
        RazorpayClient client = new(key, secret);     // connnected to razor pay client 

        Dictionary<string, object> options = new()
         {
                { "amount", amount * 100 },
                { "currency", currency },
                { "receipt", orderId },
                { "payment_capture", 1 }
            };

        var order = client.Order.Create(options); //here is dought but i will understand with time
        return order;
    }

}*/
