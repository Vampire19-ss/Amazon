using System;

namespace WebApplication1.Interfaces;

public interface ICloudinaryService
{
//public Task <string>  UploadImageAsync(IFormFile image);


public  Task<List<string>> UploadmultipleImageAsync(IEnumerable<IFormFile> images);
//public Task <string>  UploadVideoAsync(IFormFile video);
}
