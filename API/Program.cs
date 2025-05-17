using API.Middlewares;
using AutoMapper;
using BusinessLogicLayer.Generic;
using BusinessLogicLayer.IService;
using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Mappings;
using BusinessLogicLayer.Models.General;
using BusinessLogicLayer.Service;
using BusinessLogicLayer.Services;
using Data.Entity;
using DataAccessLayer;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

// Add services to the container.

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var connectionString = Environment.GetEnvironmentVariable("DB_URL");

builder.Services.AddControllers();
builder.Services.AddHostedService<WorkerService>();
builder.Services.AddHostedService<BatchCheckService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<WaseEaseDbContext>(options =>
    options.UseSqlServer(connectionString));

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
    option.MapType<TimeOnly>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "time",
        Example = new Microsoft.OpenApi.Any.OpenApiString("00:00")
    });
    option.MapType<DateOnly>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });
});

var mapper = new MapperConfiguration(mc =>
{
    mc.AddProfile<MappingProfile>();
});
builder.Services.AddSingleton(mapper.CreateMapper());

#region Generic
builder.Services.AddScoped<IGenericRepository<Warehouse>, GenericRepository<Warehouse>>();
builder.Services.AddScoped<IGenericRepository<Partner>, GenericRepository<Partner>>();
builder.Services.AddScoped<IGenericRepository<Account>, GenericRepository<Account>>();
builder.Services.AddScoped<IGenericRepository<Group>, GenericRepository<Group>>();
builder.Services.AddScoped<IGenericRepository<Permission>, GenericRepository<Permission>>();
builder.Services.AddScoped<IGenericRepository<Category>, GenericRepository<Category>>();
builder.Services.AddScoped<IGenericRepository<ErrorTicket>, GenericRepository<ErrorTicket>>();
builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
builder.Services.AddScoped<IGenericRepository<Schedule>, GenericRepository<Schedule>>();
builder.Services.AddScoped<IGenericRepository<Batch>, GenericRepository<Batch>>();
builder.Services.AddScoped<IGenericRepository<AccountGroup>, GenericRepository<AccountGroup>>();
builder.Services.AddScoped<IGenericRepository<InventoryCount>, GenericRepository<InventoryCount>>();
builder.Services.AddScoped<IGenericRepository<InventoryCountDetail>, GenericRepository<InventoryCountDetail>>();
builder.Services.AddScoped<IGenericRepository<ProductType>, GenericRepository<ProductType>>();
builder.Services.AddScoped<IGenericRepository<Brand>, GenericRepository<Brand>>();
builder.Services.AddScoped<IGenericRepository<Unit>, GenericRepository<Unit>>();
builder.Services.AddScoped<IGenericRepository<InventoryAdjustment>, GenericRepository<InventoryAdjustment>>();
builder.Services.AddScoped<IGenericRepository<InventoryAdjustmentDetail>, GenericRepository<InventoryAdjustmentDetail>>();
builder.Services.AddScoped<IGenericRepository<LocationLog>, GenericRepository<LocationLog>>();
builder.Services.AddScoped<IGenericRepository<InventoryLocation>, GenericRepository<InventoryLocation>>();
//builder.Services.AddScoped<IGenericRepository<Supplier>, GenericRepository<Supplier>>();
builder.Services.AddScoped<IGenericRepository<Data.Entity.Route>, GenericRepository<Data.Entity.Route>>();
builder.Services.AddScoped<IGenericRepository<Location>, GenericRepository<Location>>();
builder.Services.AddScoped<IGenericRepository<GoodRequest>, GenericRepository<GoodRequest>>();
builder.Services.AddScoped<IGenericRepository<GoodRequestDetail>, GenericRepository<GoodRequestDetail>>();
builder.Services.AddScoped<IGenericRepository<GoodNote>, GenericRepository<GoodNote>>();
builder.Services.AddScoped<IGenericRepository<GoodNoteDetail>, GenericRepository<GoodNoteDetail>>();
builder.Services.AddScoped<IGenericRepository<Inventory>, GenericRepository<Inventory>>();
builder.Services.AddScoped<IGenericRepository<LocationLog>, GenericRepository<LocationLog>>();
#endregion Generic

#region Services
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBatchService, BatchService>();
builder.Services.AddScoped<IProductTypesService, ProductTypesService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IErrorTicketService, ErrorTicketService>();
builder.Services.AddScoped<IInventoryCountService, InventoryCountService>();
builder.Services.AddScoped<IInventoryAdjustmentService, InventoryAdjustmentService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IGenericPaginationService, GenericPaginationService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IGoodRequestService, GoodRequestService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IGoodNoteService, GoodNoteService>();
builder.Services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
#endregion Services

#region Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IReceivingNoteRepository, ReceivingNoteRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IReceivingDetailRepository, ReceivingDetailRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IGoodRequestRepository, GoodRequestRepository>();
builder.Services.AddScoped<IGoodRequestDetailRepository, GoodRequestDetailRepository>();
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IGoodNoteRepository, GoodNoteRepository>();
builder.Services.AddScoped<IGoodNoteDetailRepository, GoodNoteDetailRepository>();
builder.Services.AddScoped<IBatchRepository, BatchRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryLocationRepository, InventoryLocationRepository>();
builder.Services.AddScoped<ILocationLogRepository, LocationLogRepository>();
builder.Services.AddScoped<IGroupPermissionRepository, GroupPermissionRepository>();
builder.Services.AddScoped<IAccountPermissionRepository, AccountPermissionRepository>();
builder.Services.AddScoped<IAccountGroupRepository, AccountGroupRepository>();
builder.Services.AddScoped<IInventoryAdjustmentRepository, InventoryAdjustmentRepository>();
builder.Services.AddScoped<IInventoryCountRepository, InventoryCountRepository>();
builder.Services.AddScoped<IInventoryCountDetailRepository, InventoryCountDetailRepository>();
#endregion Repositories

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE"),
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY"))),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Request.Cookies.TryGetValue("accessToken", out var token);
            context.Token = token;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddMemoryCache();

builder.Services.AddCors(p => p.AddPolicy("Cors", policy =>
{
    policy.WithOrigins("https://wareease.site", "http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
}));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// Global exception handler
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var exception = context.HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        if (exception != null)
        {
            context.ProblemDetails.Extensions["exceptionMessage"] = exception.Message;
        }
    };
});

var app = builder.Build();

app.UseCors("Cors");

// Config Middleware
//app.UseMiddleware<JwtMiddleware>();
//app.UseMiddleware<AccountStatusMiddleware>();
//app.UseMiddleware<TokenValidationMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler();  

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<PermissionMiddleware>();


app.MapControllers();

app.Run();

