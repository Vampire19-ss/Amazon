using System;
using WebApplication1.Models.DomainModels;

namespace WebApplication1.Models.ViewModels;

public class ProductView
{
public ICollection<Product> Products {get;set;} =[]; 
public Product? Product {get;set;}
}
