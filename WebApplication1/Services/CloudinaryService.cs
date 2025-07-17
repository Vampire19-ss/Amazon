using System;
using System.Reflection.Metadata;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using WebApplication1.Interfaces;

namespace WebApplication1.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary cloudinary;
    private string url;
    public CloudinaryService(IConfiguration configuration)
    {
        //this.cloudinary = cloudinary;

        this.url = configuration["Cloudinary:CLOUDINARY_URL"] ?? throw new ArgumentNullException("Cloudinary Url is not configured.");


        /*try
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
        }
        catch (System.Exception ex)
        {

            throw new InvalidOperationException("Failed to load .env file.", ex);
        }


        var CloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");*/

        this.cloudinary = new Cloudinary(url) { Api = { Secure = true } };



    }


    // uploads only one image

    /* public async Task<string> UploadImageAsync(IFormFile image)
     {
         if (image == null || image.Length == 0)
         {
             throw new ArgumentException("File is invalid.");
         }

         using var stream = image.OpenReadStream();


         var uploadParams = new ImageUploadParams
         {
             File = new FileDescription(image.FileName, stream),
             UseFilename = true,
             UniqueFilename = false,
             Overwrite = true,
             Folder = "Shahid"
             // Transformation = new Transformation().Width(150).Height(150).Crop("fill")
         };

         var uploadResult = await cloudinary.UploadAsync(uploadParams);

         if (uploadResult.Error != null)
         {
             throw new InvalidOperationException($"Cloudinary upload failed: {uploadResult.Error.Message}");
         }

         return uploadResult.SecureUrl.ToString();
     }*/





    // upload multiple images but not parelall

    public async Task<List<string>> UploadmultipleImageAsync(IEnumerable<IFormFile> images)
    {
        if (images == null || !images.Any())
        {
            throw new ArgumentException("Files are invalid.");
        }

        var imageUrls = new List<string>();

        foreach (var image in images)
        {

            using var stream = image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, stream),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true,
                Folder = "Shahid"
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new InvalidOperationException($"Cloudinary upload failed: {uploadResult.Error.Message}");
            }

            imageUrls.Add(uploadResult.SecureUrl.ToString());
        }

        return imageUrls;
    }





//upload multiple images parelall
   /*public async Task<List<string>> UploadmultipleImageAsync(IEnumerable<IFormFile> images)
{
    if (images == null || !images.Any())
    {
        throw new ArgumentException("Files are invalid.");
    }

    var uploadTasks = new List<Task<ImageUploadResult>>();

    foreach (var image in images)
    {
         if (image.Length == 0) continue;
        using var stream = image.OpenReadStream();
        
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true,
            Folder = "Shahid"
        };

        uploadTasks.Add(cloudinary.UploadAsync(uploadParams));
    }

    try
    {
        var uploadResults = await Task.WhenAll(uploadTasks);
        return [.. uploadResults.Select(r => r.SecureUrl?.ToString())];
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Cloudinary upload failed: {ex.Message}");
    }
}*/













    public async Task<string> UploadVideoAsync(IFormFile video)
    {

        try
        {
            if (video == null || video.Length == 0)
            {
                throw new ArgumentException("File is invalid.");
            }

            using var stream = video.OpenReadStream();


            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(video.FileName, stream),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true,
                Folder = "Shahid"
                // Transformation = new Transformation().Width(150).Height(150).Crop("fill")
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new InvalidOperationException($"Cloudinary upload failed: {uploadResult.Error.Message}");
            }

            return uploadResult.SecureUrl.ToString();

        }
        catch (System.Exception)
        {

            throw;
        }
    }
}
