using ChallengeBackendCSharp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();//.AddJsonOptions(opt => { opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve; });
builder.Services.AddDbContext<DatabaseConnector>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Implementación del servicio Identity.
builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedEmail =  false)
                .AddEntityFrameworkStores<DatabaseConnector>()
                .AddDefaultTokenProviders();

// Agregar limitaciones al momento de elegir el nombre de usuario y contraseña.
builder.Services.Configure<IdentityOptions>(options =>
{
    // Configuración de la contraseña.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Configuración del nombre de usuario.
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

// Implementación de JSON Web Token.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.SaveToken = true;
        /*options.RequireHttpsMetadata = false;*/
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JSONWebToken:Issuer"],
            ValidAudience = builder.Configuration["JSONWebToken:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JSONWebToken:Key"])),
            RequireExpirationTime = true
        };
    });

builder.Services.AddAuthorization();

// Implementación de AutoMapper, listo para ser inyectado en cualquier controlador.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Implementación de SendGrid para enviar los mensajes de bienvenida.
builder.Services.AddScoped<EmailSender>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configuración para que Swagger sea compatible con la autenticación JWT.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("jwt_auth", new OpenApiSecurityScheme
    {
        Name = "Bearer",
        Scheme = "bearer",
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        Description = "Ingrese el token que recibe al loguearse a continuación:"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Id = "jwt_auth",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
