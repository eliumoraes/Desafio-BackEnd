using Infrastructure.Repositories;
using Infrastructure.Services;
using Application.Interfaces;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using Application.Commands.Users.RegisterDeliveryDriver;
using FluentValidation;
using Application.Commands.Users.UploadDriverLicenseImage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Commands.Users.UserAuthentication;
using Application.Commands.Motorcycles.RegisterMotorcycle;
using Infrastructure.Messagin;


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


    // Esquema de autenticação via JWT
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        // Prefixo 'Bearer' no cabeçalho"
        Description = "JWT Auth header using Beaerer (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Tken definition for authorized operations
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configurações do token
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Configs do banco de dados
var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnectionString");
Console.WriteLine($"Connection String: {postgresConnectionString}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));   

// RabbitMqEventConsumer
builder.Services.AddSingleton<RabbitMqEventConsumer>();

// Mediator
builder.Services.AddMediatR(
    config =>
    {
        config.RegisterServicesFromAssemblyContaining<UserAuthenticationRequestHandler>();
        config.RegisterServicesFromAssemblyContaining<RegisterDeliveryDriverRequestHandler>();
        config.RegisterServicesFromAssemblyContaining<UploadDriverLicenseImageRequestHandler>();        
        config.RegisterServicesFromAssemblyContaining<RegisterMotorcycleRequestHandler>();
    }
);

// Fluent validaation
builder.Services.AddValidatorsFromAssemblyContaining<UserAuthenticationRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDeliveryDriverRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UploadDriverLicenseImageValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterMotorcycleRequestValidator>();

// Injeções, repositórios, services, etc
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>(); // Implementação do token JWT
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();  // Serviço de hash de senhas
builder.Services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();  // Publicador de eventos
builder.Services.AddScoped<IUserRepository, UserRepository>();  // Repositório de usuário
builder.Services.AddScoped<IUserProfileRepository, UserProfileRespository>(); // Repositório do profile
builder.Services.AddScoped<IImageUploader, MinioImageUploader>();  // MinIO para salvar imagens
builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>(); // Repositório de motos

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

//
var rabbitMqConsumer = app.Services.GetRequiredService<RabbitMqEventConsumer>();
rabbitMqConsumer.StartConsuming();

var teste = "eliu";

app.Run();

