using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using ChatApp.Data;
using ChatApp.Services;
using ChatApp.Configurations;
using ChatApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json");

// Bind RabbitMQ configuration
builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

// Services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// RabbitMQ Connection
builder.Services.AddSingleton<IConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var rabbitMQConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQConfiguration>();

    var factory = new ConnectionFactory()
    {
        HostName = rabbitMQConfig.HostName,
        UserName = rabbitMQConfig.UserName,
        Password = rabbitMQConfig.Password
    };

    return factory.CreateConnection();
});


builder.Services.AddScoped<IBotService, BotService>();
builder.Services.AddScoped<IChatService, ChatService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
