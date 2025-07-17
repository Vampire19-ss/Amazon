using System;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.JunctionModels;

namespace WebApplication1.Models.DomainModels;

public class Cart
{
    

public  Guid CartId { get; set; } = Guid.NewGuid();

public required Guid Userid { get; set; }  // Fk
[ForeignKey("Userid")]
public User? Buyer { get; set; }  // navigation property //  belonging to a user

//here we cont do like orders
public ICollection<CartProduct>? CartProducts { get; set; } = []; //  collection of products in the cart

public required decimal CartTotal { get; set; } = 0; 

}

