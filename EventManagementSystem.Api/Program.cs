using EventManagementSystem.Api.Common.AppSettings;
using EventManagementSystem.Api.Data;
using EventManagementSystem.Api.Data.Models;
using EventManagementSystem.Api.Services;
using EventManagementSystem.Api.Services.Implementations;
using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(50));
});

#endregion

#region Configure Identity
builder.Services.AddIdentity<User, Role>(opt => {
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.User.RequireUniqueEmail = true;
})
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
#endregion

#region Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
           .AddJwtBearer(option =>
           {
               option.SaveToken = true;
               option.RequireHttpsMetadata = false;
               option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,

                   ValidAudience = builder.Configuration["JwtDetails:Audience"],
                   ValidIssuer = builder.Configuration["JwtDetails:Issuer"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtDetails:SecretKey"]))
               };
           });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
                        policy.RequireClaim("Role", "Admin"));
    options.AddPolicy("OrganizerPolicy", policy =>
                        policy.RequireClaim("Role", "Organizer"));
    options.AddPolicy("ParticipantPolicy", policy =>
                        policy.RequireClaim("Role", "Participant"));
});
#endregion

#region Configure Service
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.Configure<JwtDetails>(builder.Configuration.GetSection("JwtDetails"));
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}");

app.Run();
