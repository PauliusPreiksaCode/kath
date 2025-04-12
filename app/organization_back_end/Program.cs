using System.Security.Cryptography.X509Certificates;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using organization_back_end;
using organization_back_end.Auth;
using organization_back_end.Auth.Model;
using organization_back_end.Helpers;
using organization_back_end.Interfaces;
using organization_back_end.Services;
using organization_back_end.Validation.Auth;
using Stripe;
using FileService = organization_back_end.Services.FileService;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddDbContext<SystemContext>(options =>
{
    options.UseSqlServer(builder.Configuration["SqlServer:ConnectionString"]);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Organization API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer <token>"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddLoginRequestValidator>());

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<SystemContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["Jwt:ValidAudience"],
        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        ClockSkew = TimeSpan.Zero 
    };
});

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<JwtService>();
builder.Services.AddTransient<SessionService>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHttpClient<IAIService, GeminiAIService>();
builder.Services.AddScoped<IAIService, GeminiAIService>();
builder.Services.AddSignalR();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<ILicenceService, LicenceService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IOrganizationService, OrganizationService>();
builder.Services.AddTransient<IEntryService, EntryService>();
builder.Services.AddTransient<IGroupService, GroupService>();
builder.Services.AddScoped<AuthSeeder>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policyBuilder => policyBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddMvc();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

var certPath = builder.Configuration["ASPNETCORE_Kestrel__Certificates__Default__Path"];
var certPassword = builder.Configuration["ASPNETCORE_Kestrel__Certificates__Default__Password"];

if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword))
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(8081, listenOptions =>
        {
            listenOptions.UseHttps(new X509Certificate2(certPath, certPassword));
        });

        serverOptions.ListenAnyIP(8080); // HTTP fallback
    });
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UsePathBase(new PathString("/api/v1"));
app.UseRouting();
app.UseStaticFiles();

app.UseCors(options =>
{
    options.SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithExposedHeaders("Content-Disposition");
});

app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.MapHub<EntriesHub>("/entriesHub");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SystemContext>();
    var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthSeeder>();
    await dbSeeder.SeedAsync();
}

app.Run();