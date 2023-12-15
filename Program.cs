using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine($"--> Command Service Endpoint {configuration["CommandService"]}");

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(     
        opt => opt.UseSqlServer(configuration.GetConnectionString("PlatformsConn")));
    Console.WriteLine("--> Using Sql Server Db");
}
else
{
    Console.WriteLine("--> Using In-Memory Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsDevelopment());

app.Run();
