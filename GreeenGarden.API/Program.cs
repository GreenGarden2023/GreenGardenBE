using GreeenGarden.Business.Service.CartService;
using GreeenGarden.Business.Service.CategogyService;
using GreeenGarden.Business.Service.EMailService;
using GreeenGarden.Business.Service.FeedbackService;
using GreeenGarden.Business.Service.ImageService;
using GreeenGarden.Business.Service.OrderService;
using GreeenGarden.Business.Service.PaymentService;
using GreeenGarden.Business.Service.ProductItemService;
using GreeenGarden.Business.Service.ProductService;
using GreeenGarden.Business.Service.SecretService;
using GreeenGarden.Business.Service.ServiceCalendarService;
using GreeenGarden.Business.Service.ShippingFeeService;
using GreeenGarden.Business.Service.SizeService;
using GreeenGarden.Business.Service.TakecareComboService;
using GreeenGarden.Business.Service.TakecareComboServiceServ;
using GreeenGarden.Business.Service.TakecareService;
using GreeenGarden.Business.Service.TransactionService;
using GreeenGarden.Business.Service.UserService;
using GreeenGarden.Business.Service.UserTreeService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.CategoryRepo;
using GreeenGarden.Data.Repositories.DistrictRepo;
using GreeenGarden.Data.Repositories.EmailOTPCodeRepo;
using GreeenGarden.Data.Repositories.FeedbackRepo;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.ProductItemRepo;
using GreeenGarden.Data.Repositories.ProductRepo;
using GreeenGarden.Data.Repositories.RentOrderDetailRepo;
using GreeenGarden.Data.Repositories.RentOrderGroupRepo;
using GreeenGarden.Data.Repositories.RentOrderRepo;
using GreeenGarden.Data.Repositories.RewardRepo;
using GreeenGarden.Data.Repositories.SaleOrderDetailRepo;
using GreeenGarden.Data.Repositories.SaleOrderRepo;
using GreeenGarden.Data.Repositories.ServiceCalendarRepo;
using GreeenGarden.Data.Repositories.ServiceDetailRepo;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using GreeenGarden.Data.Repositories.ShippingFeeRepo;
using GreeenGarden.Data.Repositories.SizeProductItemRepo;
using GreeenGarden.Data.Repositories.SizeRepo;
using GreeenGarden.Data.Repositories.TakecareComboRepo;
using GreeenGarden.Data.Repositories.TakecareComboServiceRepo;
using GreeenGarden.Data.Repositories.TransactionRepo;
using GreeenGarden.Data.Repositories.UserRepo;
using GreeenGarden.Data.Repositories.UserTreeRepo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// Database
builder.Services.AddDbContext<GreenGardenDbContext>(option => option.UseSqlServer(SecretService.GetConnectionString()));


// Add services to interface


builder.Services.AddScoped<ICategogyService, CategogyService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductItemService, ProductItemService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<IEMailService, EMailService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IMoMoService, MoMoServices>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IShippingFeeService, ShippingFeeService>();
builder.Services.AddScoped<IUserTreeService, UserTreeService>();
builder.Services.AddScoped<ITakecareService, TakecareService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IServiceCalendarService, ServiceCalendarService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ITakecareComboService, TakecareComboService>();
builder.Services.AddScoped<ITakecareComboServiceServ, TakecareComboServiceServ>();
//
builder.Services.AddTransient<IUserTreeRepo, UserTreeRepo>();
builder.Services.AddTransient<IServiceCalendarRepo, ServiceCalendarRepo>();
builder.Services.AddTransient<IServiceOrderRepo, ServiceOrderRepo>();
builder.Services.AddTransient<IUserRepo, UserRepo>();
builder.Services.AddTransient<IDistrictRepo, DistrictRepo>();
builder.Services.AddTransient<IShippingFeeRepo, ShippingFeeRepo>();
builder.Services.AddTransient<IRentOrderRepo, RentOrderRepo>();
builder.Services.AddTransient<IRentOrderGroupRepo, RentOrderGroupRepo>();
builder.Services.AddTransient<IRentOrderDetailRepo, RentOrderDetailRepo>();
builder.Services.AddTransient<ISaleOrderRepo, SaleOrderRepo>();
builder.Services.AddTransient<ISaleOrderDetailRepo, SaleOrderDetailRepo>();
builder.Services.AddTransient<IRewardRepo, RewardRepo>();
builder.Services.AddTransient<IEmailOTPCodeRepo, EmailOTPCodeRepo>();
builder.Services.AddTransient<ICategoryRepo, CategoryRepo>();
builder.Services.AddTransient<IProductRepo, ProductRepo>();
builder.Services.AddTransient<IProductItemRepo, ProductItemRepo>();
builder.Services.AddTransient<ISizeRepo, SizeRepo>();
builder.Services.AddTransient<IProductItemDetailRepo, ProductItemDetailRepo>();
builder.Services.AddTransient<IImageRepo, ImageRepo>();
builder.Services.AddTransient<ITransactionRepo, TransactionRepo>();
builder.Services.AddTransient<ICartRepo, CartRepo>();
builder.Services.AddTransient<IUserTreeRepo, UserTreeRepo>();
builder.Services.AddTransient<IServiceRepo, ServiceRepo>();
builder.Services.AddTransient<IServiceDetailRepo, ServiceDetailRepo>();
builder.Services.AddTransient<IFeedbackRepo, FeedbackRepo>();
builder.Services.AddTransient<ITakecareComboRepo, TakecareComboRepo>();
builder.Services.AddTransient<ITakecareComboServiceRepo, TakecareComboServiceRepo>();
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

    OpenApiSecurityScheme securityScheme = new()
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


builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Add services to the container.
builder.Services.AddControllers();
//Cors
builder.Services.AddCors(p => p.AddPolicy("AllowOrigin", builder =>
{
    _ = builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

WebApplication app = builder.Build();

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
