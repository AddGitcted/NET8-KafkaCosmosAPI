using Confluent.Kafka;
using CosmosDB_Simple_API.Repositories;
using CosmosDB_Simple_API.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
string cosmosEndpoint = cosmosDbConfig["Endpoint"];
string cosmosKey = cosmosDbConfig["Key"];
string databaseName = cosmosDbConfig["DatabaseName"];
string containerName = cosmosDbConfig["ContainerName"];

Console.WriteLine($"CosmosDB Endpoint: {cosmosEndpoint}");

CosmosClientOptions options = new()
{
    HttpClientFactory = () => new HttpClient(new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }),
    ConnectionMode = ConnectionMode.Gateway,
};

CosmosClient cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey, options);

Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
await database.CreateContainerIfNotExistsAsync(containerName, "/id");

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers"),
    GroupId = builder.Configuration.GetValue<string>("Kafka:GroupId"),
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = true
};


builder.Services.AddSingleton(consumerConfig);
builder.Services.AddSingleton<ITaskRepository>(new TaskRepository(cosmosClient, databaseName, containerName));
//builder.Services.AddSingleton<ITaskRepository, MockTaskRepository>();
builder.Services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();
builder.Services.AddHostedService<KafkaConsumerHostedService>();
builder.Services.AddControllers();
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
app.MapControllers();
app.Run();
