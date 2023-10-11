using api.Data;
using api.Models;
using api.Repositories;
using api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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
services.AddHostedService<ClientBackgroundService>();
services.AddSingleton<IEmailRepository, EmailRepository>();
services.AddSingleton<IDocumentRepository, DocumentRepository>();
services.AddScoped<IClientService, ClientService>();


var app = builder.Build();


var backgroundService = new ClientBackgroundService(app.Services,
    app.Services.GetRequiredService<IEmailRepository>(),
    app.Services.GetRequiredService<IDocumentRepository>());

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
    try
    {
        await _clientService.Create(client);

        await backgroundService.TriggerBackgroundTasksAsync();


        return Results.Ok("Client added.");
    }
    catch (Exception ex)
    {
        return Results.Ok("An error occurred while adding client. " + ex.Message);
    }
});


app.MapPut("/clients/{id}", async (IClientService _clientService, string id, Client client) =>
{
    try
    {
        var existingClient = await _clientService.GetById(client.Id);
        await _clientService.Update(client);

        if (existingClient != null)
        {
            if (existingClient.Email != client.Email)
            {
                // Trigger the background service to run tasks (e.g., SendEmail() and UpdateDocuments())
                await backgroundService.TriggerBackgroundTasksAsync();
            }
        }

        return Results.Ok(client);
    }
    catch (Exception ex)
    {
        return Results.Ok("An error occurred while updating client. " + ex.Message);
    }
});

app.MapGet("/clients/{id}", async (int id, IClientService _clientService) =>
{
    var client = await _clientService.Get();
    if (client != null)
    {
        return Results.Ok(client);
    }
    else
    {
        return Results.NotFound();
    }
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