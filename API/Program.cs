using API.Payloads.Responses;
using AutoMapper;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.Mappings;
using BusinessLogicLayer.Service;
using BusinessLogicLayer.Services;
using DataAccessLayer;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<WaseEaseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//Add kebab
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "WareEase API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    }); option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Add Mapping profiles
var mapper = new MapperConfiguration(mc =>
{
    mc.AddProfile<MappingProfile>();
});
builder.Services.AddSingleton(mapper.CreateMapper());

// Register repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IPurchaseReceiptRepository, PurchaseReceiptRepository>();
builder.Services.AddScoped<IReceivingNoteRepository, ReceivingNoteRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
builder.Services.AddScoped<IPurchaseDetailRepository, PurchaseDetailRepository>();
builder.Services.AddScoped<IReceivingDetailRepository, ReceivingDetailRepository>();
builder.Services.AddScoped<ITypeDetailRepository, TypeDetailRepository>();
builder.Services.AddScoped<IProductTypeTypeDetailRepository, ProductTypeTypeDetailRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IAppActionRepository, AppActionRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IAccountGroupRepository, AccountGroupRepository>();
builder.Services.AddScoped<IAccountPermissionRepository, AccountPermissionRepository>();

// Register servicies
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IPurchaseReceiptService, PurchaseReceiptService>();
builder.Services.AddScoped<IReceivingNoteService, ReceivingNoteService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IPurchaseDetailService, PurchaseDetailService>();
builder.Services.AddScoped<IReceivingDetailService, ReceivingDetailService>();
builder.Services.AddScoped<ITypeDetailService, TypeDetailService>();
builder.Services.AddScoped<IProductTypeTypeDetailService, ProductTypeTypeDetailService>();
builder.Services.AddScoped<IGenericPaginationService, GenericPaginationService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IAppActionService, AppActionService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IAccountGroupService, AccountGroupService>();
builder.Services.AddScoped<IAccountPermissionService, AccountPermissionService>();

////Config Jwt Token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
        ClockSkew = TimeSpan.Zero
    };
});

// Add CORS
builder.Services.AddCors(p => p.AddPolicy("Cors", policy =>
{
    policy.WithOrigins("*")
          .AllowAnyHeader()
          .AllowAnyMethod();
}));
// add  json option để tránh vòng lặp tại json khi trả về
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Đặt múi giờ VN cho toàn bộ ứng dụng
TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("vi-VN");

var app = builder.Build();

app.UseCors("Cors");

// Config Middleware
//app.UseMiddleware<AccountStatusMiddleware>();
//app.UseMiddleware<TokenValidationMiddleware>();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
//app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();


app.MapControllers();

app.Run();

