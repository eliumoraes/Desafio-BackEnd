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
    private readonly string _endpoint;
    public MinioImageUploader(IConfiguration configuration)
    {
        _bucketName = configuration["MinioSettings:BucketName"];

        _endpoint = configuration["MinioSettings:Endpoint"];
        var accessKey = configuration["MinioSettings:AccessKey"];
        var secretKey = configuration["MinioSettings:SecretKey"];

        if (string.IsNullOrEmpty(_endpoint) || string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        {
            throw new MinioException("MinIO credentials are not properly set.");
        }

        _minioClient = new MinioClient()
            .WithEndpoint(_endpoint)
            .WithCredentials(accessKey, secretKey)
            .Build();
    }
    public async Task<IResult<string>> UploadImageAsync(IFormFile image)
    {
        try
        {
            var fileName = $"{Guid.NewGuid()}_{image.FileName}".Trim();

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

            var imageLocation = $"{_bucketName}/{fileName}";
            return Result<string>.Success(imageLocation);
        }
        catch (MinioException ex)
        {
            //Log error
            return Result<string>.Fail(
                new List<string> { $"Minio error: {ex.Message}" }
            );
        }
    }

    public async Task<IResult<string>> DeleteImageAsync(string imageLocation)
    {
         try
        {
            var fileName = imageLocation.Substring(imageLocation.LastIndexOf('/') + 1);

            await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
            );

            return Result<string>.Success(imageLocation);
        }
        catch (MinioException ex)
        {
            return Result<string>.Fail(
                new List<string> { $"Minio error: {ex.Message}" }
            );
        }
    }
}