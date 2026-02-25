using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Mappings;
using Application.Services;
using Infrastructure;
using Infrastructure.Hubs;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using LeaveManagementSystem.API.Middlewares; // Middleware klasörünü eklemeyi unutma
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. SERVÝS KAYITLARI (Dependency Injection)
// ==========================================

// Infrastructure katmanýndaki temel servisleri (DbContext vb.) ekle
builder.Services.AddInfrastructureServices(builder.Configuration);

// --- AUTOMAPPER ---
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// --- CONTROLLERS & SWAGGER ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- REPOSITORY KAYITLARI (Veritabaný Eriþim) ---
// Not: GenericRepository genelde InfrastructureServices içinde eklenir ama 
// özel repository'leri burada belirtmek güvenlidir.
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

// Program.cs içinde Service kayýtlarý bölümünde:
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();


builder.Services.AddScoped<IEmployeeService, EmployeeService>();


// --- SERVICE KAYITLARI (Ýþ Mantýðý) ---
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ILeaveReportService, LeaveReportService>(); // Yeni Raporlama Servisi

// --- SIGNALR ---
builder.Services.AddSignalR();

// --- CORS AYARLARI ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
        });
});

// --- JWT AUTHENTICATION ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
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

    // SignalR için Token Ayarý (Query String'den okuma)
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// --- SWAGGER AYARLARI ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Leave Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// --- BÝNA ÝNÞA EDÝLÝYOR ---
var app = builder.Build();

// ==========================================
// 2. HTTP REQUEST PIPELINE (Middleware)
// ==========================================

// 1. Swagger (Geliþtirme Ortamý)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. GLOBAL EXCEPTION MIDDLEWARE (Hata Yakalayýcý)
// Bu satýr, Auth ve Controller'dan ÖNCE gelmeli ki her þeyi yakalayabilsin.
app.UseMiddleware<ExceptionMiddleware>();

// 3. Standart Middlewareler
app.UseCors("AllowAll");
app.UseHttpsRedirection();

// 4. Kimlik Doðrulama ve Yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

// 5. Endpoint Haritalama
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub"); // SignalR Endpoint

// Ana sayfayý Swagger'a yönlendir
app.MapGet("/", async context =>
{
    context.Response.Redirect("/swagger");
    await Task.CompletedTask;
});

// --- VERÝTABANI BAÞLATICI (SEED DATA) ---
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Veritabaný baþlatma hatasý: " + ex.Message);
    }
}

app.Run();