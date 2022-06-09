using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using PortfolioTracker.Api.Infrastructure;
using PortfolioTracker.DataAccess;
using PortfolioTracker.DataAccess.Repositories;
using PortfolioTracker.Events;
using PortfolioTracker.EventStore;
using PortfolioTracker.EventStore.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AssetRepository>();
builder.Services.AddScoped<TransactionRepository>();

builder.Services.AddCosmosClient(builder.Configuration);

builder.Services.AddSingleton<IEventStore, AssetEventStore>(sp =>
{
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    var settings = sp.GetRequiredService<IOptions<CosmosDbSettings>>().Value;
    var database = cosmosClient.CreateDatabaseIfNotExistsAsync(settings.DatabaseId).GetAwaiter().GetResult().Database;
    var eventStoreSerializer = new JsonStoredEventSerializer(
        typeof(AssetCreated).Assembly);

    var eventStore = new AssetEventStore(
        eventStoreSerializer,
        database);

    eventStore.InitializeAppendEventsStoredProcedure().GetAwaiter().GetResult();

    return eventStore;
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");
app.MapControllers();

app.Run();
