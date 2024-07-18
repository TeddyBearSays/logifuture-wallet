using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WalletSytem.BusinessLayer;
using WalletSytem.DataAccess;
using WalletSytem.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddScoped<IWalletRepository, WalletRepository>();
services.AddScoped<IWalletService, WalletService>();
services.AddSingleton<ICorrelationIdProvider, CorrelationIdProvider>();

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<WalletDbContext>(options =>
    options.UseInMemoryDatabase(databaseName: "DB"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.CorrelationIdMiddleware();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

