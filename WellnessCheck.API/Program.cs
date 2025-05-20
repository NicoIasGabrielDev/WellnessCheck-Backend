using Microsoft.EntityFrameworkCore;
using WellnessCheck.API.Data;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Adding the connection string 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Adding swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WellnessCheck API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Enter your JWT token here in this format: Bearer {your_token}",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

//Adding Jwt
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = ClaimTypes.Role
        };
    });

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Add Cors Policy to allow acess
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Add Migration to enable seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

//Configure Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Adding data to api
DbSeeder.Seed(app);

app.Run();
