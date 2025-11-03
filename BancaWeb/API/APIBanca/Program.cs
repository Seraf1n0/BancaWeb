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
builder.Services.AddSingleton<EncryptionProtect>();

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

builder.Services.AddHttpClient<DeleteUserRepository>();
builder.Services.AddScoped<DeleteUserRepository>();
builder.Services.AddScoped<DeleteUserService>();


builder.Services.AddHttpClient<CreateCardRepository>();
builder.Services.AddScoped<CreateCardService>();
builder.Services.AddScoped<CreateCardRepository>();


builder.Services.AddHttpClient<GetCardRepository>();
builder.Services.AddScoped<GetCardService>();
builder.Services.AddScoped<GetCardRepository>();


builder.Services.AddHttpClient<CardMovementRepository>();
builder.Services.AddScoped<CardMovementService>();
builder.Services.AddScoped<CardMovementRepository>();

builder.Services.AddHttpClient<GetCardMovementRepository>();
builder.Services.AddScoped<GetCardMovementService>();
builder.Services.AddScoped<GetCardMovementRepository>();


builder.Services.AddHttpClient<OtpCreateRepository>();
builder.Services.AddScoped<OtpCreateService>();
builder.Services.AddScoped<OtpCreateRepository>();


builder.Services.AddHttpClient<OtpConsumeRepository>();
builder.Services.AddScoped<OtpConsumeService>();
builder.Services.AddScoped<OtpConsumeRepository>();

builder.Services.AddHttpClient<BankValidateRepository>();
builder.Services.AddScoped<BankValidateService>();
builder.Services.AddScoped<BankValidateRepository>();

builder.Services.AddHttpClient<ForgotPasswordRepository>();
builder.Services.AddScoped<ForgotPasswordService>();
builder.Services.AddScoped<ForgotPasswordRepository>();


builder.Services.AddHttpClient<VerifyOtpRepository>();
builder.Services.AddScoped<VerifyOtpService>();
builder.Services.AddScoped<VerifyOtpRepository>();


builder.Services.AddHttpClient<ResetPasswordRepository>();
builder.Services.AddScoped<ResetPasswordService>();
builder.Services.AddScoped<ResetPasswordRepository>();

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