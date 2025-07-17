using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.DomainModels;

public class Createblog22
{

    [Key]
    public  Guid  BlogId { get; set; } = Guid.NewGuid();
    public string? Title { get; set; }
    public  string? BlogImage { get; set; }
    public string? Shotdescription { get; set; }
    public string? Description { get; set; }

}
