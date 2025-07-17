using System;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.JunctionModels;
using WebApplication1.Types;

namespace WebApplication1.Models.DomainModels;

public class Product
{
   

    [Key]
public  Guid ProductId { get; set; } = Guid.NewGuid();
public required string ProductName { get; set; }
public required string ProductDescription { get; set; }
public required string ProductImage { get; set; }
public required decimal ProductPrice { get; set; }
public required int ProductStock { get; set; }
public required ProductCategory Category { get; set; } = ProductCategory.General;


public string? Size {get;set;}
public string? Color {get ;set;}
public string? Weight {get;set;}



public ICollection<CartProduct>?  CartProducts{ get; set; } = [];   // navigation property //  collection of products in the cart
public ICollection<OrderProduct>? OrderProducts { get; set; } = []; //  collection of products in the order



public required bool IsDeleted { get; set; } = false;
public required bool IsActive { get; set; } = true;
public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
