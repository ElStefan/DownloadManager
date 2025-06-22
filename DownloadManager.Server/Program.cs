using DownloadManager.Server.Services;
using DownloadService = DownloadManager.Library.DownloadManagerService;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console());
builder.Services.AddSingleton<DownloadService>();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DownloadManagerServiceGrpc>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();
