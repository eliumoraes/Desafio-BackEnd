using MediatR;                // Para MediatR
using Infrastructure.Repositories;
using Infrastructure.Services;
using Application.Interfaces;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using Application.Commands.Users.RegisterDeliveryDriver;
using FluentValidation;
using Application.Commands.Users.UploadDriverLicenseImage;


var builder = WebApplication.CreateBuilder(args);

// Serviços essenciais
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Caminho do arquivo XML de documentação
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // XML no Swagger
    options.IncludeXmlComments(xmlPath);
});

// Configs do banco de dados
var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
Console.WriteLine($"Connection String: {postgresConnectionString}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

// Mediator
builder.Services.AddMediatR(
    config => 
    {
        config.RegisterServicesFromAssemblyContaining<RegisterDeliveryDriverRequestHandler>();
        config.RegisterServicesFromAssemblyContaining<UploadDriverLicenseImageRequestHandler>();
    }
);

// Fluent validaation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDeliveryDriverRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UploadDriverLicenseImageValidator>();

// Injeções, repositórios, services, etc
builder.Services.AddScoped<IUserRepository, UserRepository>();  // Repositório de usuário
builder.Services.AddScoped<IUserProfileRepository, UserProfileRespository>(); // Repositório do profile
builder.Services.AddScoped<IImageUploader, MinioImageUploader>();  // MinIO para salvar imagens
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();  // Serviço de hash de senhas

// salvando builder
var app = builder.Build();

// Configurações do swagger pra development
if (app.Environment.IsDevelopment())
{
    //Alterações específicas de env
    Console.WriteLine("Estou considerando DEV quando roda fora do Docker");
}

//Mantendo swagger pra todos os ambientes
app.UseSwagger();
app.UseSwaggerUI();

// Middlewares padrão
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
