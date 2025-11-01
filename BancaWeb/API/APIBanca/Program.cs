using APIBanca.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<AuthUsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddControllers();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddHttpClient<UserRepository>();
builder.Services.AddScoped<CreateUserService>();


builder.Services.AddSingleton<InfoUserRepository>(); //

builder.Services.AddSingleton<GetInfoUserService>();
builder.Services.AddHttpClient<UserRolIDService>();


builder.Services.AddSingleton<ApiKeyGeneratorService>();
builder.Services.AddHttpClient<ApiKeyRepository>();
builder.Services.AddScoped<ApiKeyRepository>();

builder.Services.AddHttpClient<UpdateUserRepository>();
builder.Services.AddScoped<UpdateUserRepository>();
builder.Services.AddScoped<UpdateUserService>();


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),

            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role
        };
    });




// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();