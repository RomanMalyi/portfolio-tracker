using DbUp;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using PortfolioTracker.Api.Infrastructure;
using PortfolioTracker.DataAccess;
using PortfolioTracker.DataAccess.Repositories;
using PortfolioTracker.Events;
using PortfolioTracker.EventStore;
using PortfolioTracker.EventStore.Core;
using DbUp.Engine;

var builder = WebApplication.CreateBuilder(args);

string sqlConnection = builder.Configuration.GetSection("SqlDB")["ConnectionString"];
RunDbMigrations(sqlConnection);
InitializeCosmosDb(builder.Configuration);

// Add services to the container.
builder.Services.AddTransient(_ => new SqlDatabase(sqlConnection));
builder.Services.AddTransient<AccountRepository>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<AssetRepository>();
builder.Services.AddTransient<AssetArRepository>();
builder.Services.AddTransient<TransactionRepository>();

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

builder.Services.AddCors(options =>
{
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

void InitializeCosmosDb(IConfiguration configuration)
{
    using var cosmos = new CosmosClient(configuration.GetSection("CosmosDb")["ConnectionString"]);
    Database database = cosmos.CreateDatabaseIfNotExistsAsync(configuration.GetSection("CosmosDb")["DatabaseId"]).GetAwaiter().GetResult().Database;
    database.CreateContainerIfNotExistsAsync(AssetEventStore.AssetsContainerId, "/StreamId").GetAwaiter().GetResult();
    database.CreateContainerIfNotExistsAsync(SnapshotRepository.SnapshotsContainerId, "/UserId").GetAwaiter().GetResult();
}

static void RunDbMigrations(string connectionString)
{
    Console.WriteLine("DbUp migration starting...");

    var migrationScriptsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlDbMigrations"));

    EnsureDatabase.For.SqlDatabase(connectionString);
    var upgrader =
        DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScripts(GetMigrationScripts(migrationScriptsPath))
            .WithTransaction()
            .LogToConsole()
            .Build();

    upgrader.PerformUpgrade();

    Console.WriteLine("DbUp migration finished");
}

static IReadOnlyCollection<SqlScript> GetMigrationScripts(
    string path)
{
    return (Directory.GetFiles(path, "*.sql", SearchOption.AllDirectories)).OrderBy<string, string>((Func<string, string>)(fullPath => fullPath)).Select<string, SqlScript>(new Func<string, SqlScript>(SqlScript.FromFile)).ToArray<SqlScript>();
}
