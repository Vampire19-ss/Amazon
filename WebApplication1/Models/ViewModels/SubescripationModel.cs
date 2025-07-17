using System;

namespace WebApplication1.Models.ViewModels;

public class SubescripationModel
{
    public int Amount { get; set; }
    public string? Currency { get; set; }
    public  Guid id { get; set; } = Guid.NewGuid();
}
