using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DomainModels;

public class Support
{

    [Key]
    public Guid SupportId { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? SupportImage { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
}
