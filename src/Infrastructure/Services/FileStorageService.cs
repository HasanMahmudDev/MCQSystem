using MCQSystem.Application.Interfaces;

namespace MCQSystem.Infrastructure.Services;

public sealed class FileStorageService : IFileStorageService
{
    public async Task<string> SaveAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var safeName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var root = Path.Combine(AppContext.BaseDirectory, "uploads");
        Directory.CreateDirectory(root);

        var path = Path.Combine(root, safeName);
        await using var fileStream = File.Create(path);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return path;
    }
}
