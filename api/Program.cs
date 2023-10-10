using api.Data;
using api.Models;
using api.Repositories;
using api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// cors
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => builder
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .Build());
});

// ioc
services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(databaseName: "Test"));

services.AddScoped<DataSeeder>();
services.AddScoped<IClientRepository, ClientRepository>();
services.AddScoped<IEmailRepository, EmailRepository>();
services.AddScoped<IDocumentRepository, DocumentRepository>();
services.AddScoped<IClientService, ClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/clients", async (IClientService _clientService) =>
{
    return await _clientService.Get();
})
.WithName("get clients");

app.MapGet("/api/clients/{searchParam}", async (IClientService _clientService, string searchParam) =>
{
    var clients = await _clientService.Search(searchParam);
    if (clients == null)
    {
        return Results.NotFound($"Clients not found.");
    }

    return Results.Ok(clients);
});

app.MapPost("/clients", async (IClientService _clientService, Client client) =>
{
    await _clientService.Create(client);
});

app.MapPut("/clients/{id}", async (IClientService _clientService, string id, Client client) =>
{
    await _clientService.Update(client);
});


app.UseCors();

// seed data
using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();

    dataSeeder.Seed();
}

// run app
app.Run();