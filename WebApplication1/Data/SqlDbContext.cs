using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Models.DomainModels;
using WebApplication1.Models.JunctionModels;

namespace WebApplication1.Data;

public class sqlDbcontext : DbContext
{
    public sqlDbcontext(DbContextOptions<sqlDbcontext> options) : base(options) { } // 11th aprial i will understand it 
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Cart> Carts { get; set; }

    public DbSet<Createblog22> Createblogs22 { get; set; }
    public DbSet<Support> supports { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<CreateSubscriptionss> CreateSubscriptionss { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) // it is called fluient api which means it ovverides default ef core functions
    {

        modelBuilder.Entity<Address>()
        .HasOne(a => a.Buyer)
        .WithOne(b => b.Address)
        .HasForeignKey<Address>(a => a.UserId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete


        modelBuilder.Entity<Cart>()
        .HasOne(c => c.Buyer)
        .WithOne(b => b.Cart)
        .HasForeignKey<Cart>(c => c.Userid)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete


        modelBuilder.Entity<Order>()
        .HasOne(o => o.Buyer)
        .WithMany(b => b.Orders)
        .HasForeignKey(o => o.UserId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete


        modelBuilder.Entity<Order>() //new fluient api
       .HasOne(o => o.Address)
       .WithMany(a => a.Orders)
       .HasForeignKey(o => o.AddressId)
       .OnDelete(DeleteBehavior.Restrict);



        //// tmarrow junction relation  and more practiceeee nessary   
        // // here is tricky understand twmarrow and have one question
        ///
        modelBuilder.Entity<CartProduct>()
        .HasKey(cp => new { cp.CartId, cp.ProductId }); // composite key


        modelBuilder.Entity<CartProduct>()
        .HasOne(cp => cp.Cart)
        .WithMany(c => c.CartProducts) // cart can have many products
        .HasForeignKey(cp => cp.CartId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete


        modelBuilder.Entity<CartProduct>()
        .HasOne(cp => cp.Product)       // every carTproduct is having one uniq product with many carts having the same product
        .WithMany(p => p.CartProducts)
        .HasForeignKey(cp => cp.ProductId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete





        modelBuilder.Entity<OrderProduct>()
       .HasKey(op => new { op.OrderId, op.ProductId }); // composite key


        modelBuilder.Entity<OrderProduct>()
        .HasOne(op => op.Order)
        .WithMany(o => o.OrderProducts)
        .HasForeignKey(op => op.OrderId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        modelBuilder.Entity<OrderProduct>()
        .HasOne(op => op.Product)          // every orderproduct is having product with many orders having the same product
        .WithMany(p => p.OrderProducts)
        .HasForeignKey(op => op.ProductId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        // dont understand it well 18 april friday now friday i understand all
        //done dna doneeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee

        modelBuilder.Entity<Subscription>()
                .HasOne(b => b.Buyer)
                .WithOne(s => s.subscriptions)
                .HasForeignKey<Subscription>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
    }


}
