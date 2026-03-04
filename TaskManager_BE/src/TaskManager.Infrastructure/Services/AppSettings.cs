using Microsoft.Extensions.Configuration;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Infrastructure.Services;

public sealed class AppSettings(IConfiguration configuration) : IAppSettings
{
    public string AppUrl => configuration["AppUrl"] ?? "http://localhost:5173";
}
