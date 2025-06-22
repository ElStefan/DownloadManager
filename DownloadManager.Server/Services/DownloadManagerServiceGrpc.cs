using DownloadService = DownloadManager.Library.DownloadManagerService;
using AppInfoModel = DownloadManager.Library.Models.AppInfo;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;

namespace DownloadManager.Server.Services;

public class DownloadManagerServiceGrpc : DownloadManager.DownloadManagerBase
{
    private readonly DownloadService _service;
    private readonly ILogger<DownloadManagerServiceGrpc> _logger;

    public DownloadManagerServiceGrpc(DownloadService service, ILogger<DownloadManagerServiceGrpc> logger)
    {
        _service = service;
        _logger = logger;
    }

    public override Task<AppList> GetApps(Empty request, ServerCallContext context)
    {
        var result = _service.GetApps();
        var list = new AppList();
        if (result.Success && result.Data != null)
        {
            foreach (var app in result.Data)
            {
                list.Apps.Add(new AppInfo
                {
                    Id = app.Id,
                    Name = app.Name,
                    Link = app.Link,
                    LinkValid = app.LinkValid,
                    WikiLink = app.WikiLink ?? string.Empty,
                    WikiVersion = app.WikiVersion ?? string.Empty,
                    LastUpdate = app.LastUpdate.HasValue ? Timestamp.FromDateTime(app.LastUpdate.Value.ToUniversalTime()) : null
                });
            }
        }
        return Task.FromResult(list);
    }

    public override Task<RequestResult> Add(AppInfo request, ServerCallContext context)
    {
        var result = _service.Add(ToModel(request));
        return Task.FromResult(new RequestResult { Message = result.Message ?? string.Empty, Success = result.Success });
    }

    public override Task<RequestResult> Update(AppInfo request, ServerCallContext context)
    {
        var result = _service.Update(ToModel(request));
        return Task.FromResult(new RequestResult { Message = result.Message ?? string.Empty, Success = result.Success });
    }

    public override Task<RequestResult> Remove(AppId request, ServerCallContext context)
    {
        var result = _service.Remove(request.Id);
        return Task.FromResult(new RequestResult { Message = result.Message ?? string.Empty, Success = result.Success });
    }

    public override Task<BinaryData> GetBinaries(AppId request, ServerCallContext context)
    {
        var result = _service.GetBinaries(request.Id);
        var data = result.Data != null ? ByteString.CopyFrom(result.Data) : ByteString.Empty;
        return Task.FromResult(new BinaryData { Data = data });
    }

    public override Task<RequestResult> RemoveCachedFile(AppId request, ServerCallContext context)
    {
        var result = _service.RemoveCachedFile(request.Id);
        return Task.FromResult(new RequestResult { Message = result.Message ?? string.Empty, Success = result.Success });
    }

    public override Task<RequestResult> ClearAllCachedFiles(Empty request, ServerCallContext context)
    {
        var result = _service.ClearAllCachedFiles();
        return Task.FromResult(new RequestResult { Message = result.Message ?? string.Empty, Success = result.Success });
    }

    public override Task<RequestResult> CreateCachedFile(AppId request, ServerCallContext context)
    {
        var result = _service.CreateCachedFile(request.Id);
        return Task.FromResult(new RequestResult { Message = result.Message ?? string.Empty, Success = result.Success });
    }

    public override Task<RequestResult> CreateCachedFiles(IdList request, ServerCallContext context)
    {
        var ids = request.Ids.ToList();
        var result = _service.CreateCachedFiles(ids);
        return Task.FromResult(new RequestResult { Message = result.Message ?? string.Empty, Success = result.Success });
    }

    private static AppInfoModel ToModel(AppInfo app)
    {
        return new AppInfoModel
        {
            Id = app.Id,
            Name = app.Name,
            Link = app.Link,
            LinkValid = app.LinkValid,
            WikiLink = app.WikiLink,
            WikiVersion = app.WikiVersion,
            LastUpdate = app.LastUpdate != null ? app.LastUpdate.ToDateTime() : null
        };
    }
}
