using System;
using WebApplication1.Models.DomainModels;
using WebApplication1.Models.JunctionModels;

namespace WebApplication1.Models.ViewModels;

public class CartView
{
   public Cart? Cart {get;set;}
   public Address ? Address {get;set;}//added today
   public ICollection<CartProduct>?products{get;set;}=[];
}
