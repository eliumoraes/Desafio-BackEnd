using Application.Interfaces;
using Domain.Common.Implementations;
using Domain.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Infrastructure.Services;

public class MinioImageUploader : IImageUploader
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    public MinioImageUploader(IConfiguration configuration)
    {
        _bucketName = configuration["MinioSettings:BucketName"];
        _minioClient = new MinioClient()
            .WithEndpoint(configuration["MinioSettings:Endpoint"])
            .WithCredentials(configuration["MinioSettings.AccessKey"], configuration["MinioSettings:SecretKey"])
            .Build();
    }
    public async Task<IResult<string>> UploadImageAsync(IFormFile image)
    {
        try
        {
            var fileName = $"{Guid.NewGuid()}_{image.FileName}";

            bool bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_bucketName)
            );

            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_bucketName)
                );
            }

            using (var stream = image.OpenReadStream())
            {
                await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fileName)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(image.ContentType)
                );
            }

            var imageUrl = $"{_bucketName}/{fileName}";
            return Result<string>.Success(imageUrl);
        }
        catch (MinioException ex)
        {

            return Result<string>.Fail(
                new List<string> { $"Minio error: {ex.Message}"}
            );
        }
    }
}