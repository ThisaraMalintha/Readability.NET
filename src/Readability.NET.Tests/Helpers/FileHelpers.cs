namespace Readability.NET.Tests.Helpers;

internal static class FileHelpers
{
    public static async Task<string> GetFileContent(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
        }

        return await File.ReadAllTextAsync(filePath);
    }
}
