using MediatR;                // Para MediatR
using Infrastructure.Repositories;
using Infrastructure.Services;
using Application.Interfaces;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using Application.Commands.Users.RegisterDeliveryDriver;
using FluentValidation;


var builder = WebApplication.CreateBuilder(args);

// Serviços essenciais
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configs do banco de dados
var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
Console.WriteLine($"Connection String: {postgresConnectionString}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

// Mediator
builder.Services.AddMediatR(
    config => 
    config.RegisterServicesFromAssemblyContaining<RegisterDeliveryDriverRequestHandler>()
);

// Fluent validaation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDeliveryDriverRequestValidator>();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares padrão
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
