using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using GreeenGarden.Data.Entities;
using GreeenGarden.Business.Service;
using GreeenGarden.Business.Service.UserService;
using GreeenGarden.Data.Repositories.UserRepo;
using GreeenGarden.Business.Service.SecretService;
using GreeenGarden.Business.Service.CategogyService;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using GreeenGarden.Business.Service.ProductService;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Business.Service.ProductItemService;
using GreeenGarden.Data.Repositories.OrderRepo;
using GreeenGarden.Business.Service.OrderService;


using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Service.PaymentService;
using GreeenGarden.Business.Service.SizeService;
using GreeenGarden.Data.Repositories.SizeRepo;
using GreeenGarden.Business.Service.SubProductService;
using GreeenGarden.Data.Repositories.SubProductRepo;

var builder = WebApplication.CreateBuilder(args);
// Database
builder.Services.AddDbContext<GreenGardenDbContext>(option => option.UseSqlServer(SecretService.GetConnectionString()));


// Add services to interface

builder.Services.AddScoped<ICategogyService, CategogyService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductItemService, ProductItemService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<ISubProductService, SubProductService>();

builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddScoped<IMoMoService, MoMoServices>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IImageService, ImageService>();
//
builder.Services.AddTransient<IUserRepo, UserRepo>();
builder.Services.AddTransient<ICategoryRepo, CategoryRepo>();
builder.Services.AddTransient<IProductRepo, ProductRepo>();
builder.Services.AddTransient<IProductItemRepo, ProductItemRepo>();
builder.Services.AddTransient<IOrderRepo, OrderRepo>();
builder.Services.AddTransient<ISizeRepo, SizeRepo>();
builder.Services.AddTransient<ISubProductRepo, SubProductRepo>();


//Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "GreenGarden.API",
        Description = "APIs for GreenGarden"
    });

    var securityScheme = new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. " +
                        "\n\nEnter 'Bearer' [space] and then your token in the text input below. " +
                          "\n\nExample: 'Bearer 12345abcde'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference()
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        securityScheme,
                        new string[]{ }
                    }
                });

});

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("greengarden-api-version"),
                                                    new MediaTypeApiVersionReader("greengarden-api-version"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(SecretService.GetTokenSecret())),
            ValidateIssuer = false,
            ValidateAudience = false,

        };
    });


// Add services to the container.
builder.Services.AddControllers();
//Cors
builder.Services.AddCors(p => p.AddPolicy("AllowOrigin", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseCors("AllowOrigin");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
