using arts_core.Data;
using arts_core.Hubs;
using arts_core.Interfaces;
using arts_core.Service;
using arts_core.Setting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"), sqlServerOptions =>
    {
        sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var allowOrigin = builder.Configuration.GetSection("AllowOrigin").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("myAppCors", policy =>
    {
        policy.WithOrigins(allowOrigin).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["JwtSettings:Key"]!
            )),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddDbContext<DataContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"));

});
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ITypeRepository, TypeRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IAddressRepository, AddressRepository>();
builder.Services.AddTransient<IReviewRepository, ReviewRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
builder.Services.AddTransient<IRefundRepository, RefundRepository>();
builder.Services.AddTransient<IExchangeRepository, ExchangeRepository>();
builder.Services.AddTransient<ISeed, Seed>();
builder.Services.AddTransient<IFileService, FileService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("myAppCors");

app.MapHub<NotificationHub>("/notification");

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
