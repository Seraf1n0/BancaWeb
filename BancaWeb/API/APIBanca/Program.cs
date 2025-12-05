using APIBanca.Services;
using APIBanca.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using APIBanca.Handlers;


var builder = WebApplication.CreateBuilder(args);
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

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

builder.Services.AddHttpClient<CreateAccountRepository>();
builder.Services.AddScoped<CreateAccountService>();
builder.Services.AddHttpClient<GetAccountRepository>();
builder.Services.AddScoped<GetAccountService>();
builder.Services.AddHttpClient<UpdateAccountStateRepository>();
builder.Services.AddScoped<UpdateAccountStateService>();
builder.Services.AddHttpClient<AccountMovementRepository>();
builder.Services.AddScoped<AccountMovementService>();
builder.Services.AddHttpClient<GetAccountMovementsRepository>();
builder.Services.AddScoped<GetAccountMovementsService>();


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


builder.Services.AddHttpClient<InterbankTransferRepository>();
builder.Services.AddHttpClient<TransferReserveRepository>();
builder.Services.AddScoped<TransferReserveRepository>();
builder.Services.AddHttpClient<TransferCreditRepository>();
builder.Services.AddScoped<TransferCreditRepository>();
builder.Services.AddSingleton<BankSocketHandler>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<BankSocketHandler>>();
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new BankSocketHandler(logger, scopeFactory);
});


builder.Services.AddHttpClient<TransferDebitRepository>(c =>
{
    c.BaseAddress = new Uri(supabaseUrl + "/rest/v1/");
    c.DefaultRequestHeaders.Add("apikey", supabaseKey);
    c.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");
});



builder.Services.AddScoped<InterbankTransferService>();



builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
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
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("[JWT] Auth failed: " + ctx.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });



// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5174", "http://86.48.22.73:5174", "http://86.48.22.73:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});



var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();


}



//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//Pedimos el objeto y lo dejamos en un segundo plano 
var centralBank = app.Services.GetRequiredService<BankSocketHandler>();
_ = centralBank.ConnectAsync();

app.Run();
