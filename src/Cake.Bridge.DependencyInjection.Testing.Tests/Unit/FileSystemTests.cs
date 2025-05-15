namespace Cake.Bridge.DependencyInjection.Testing.Tests.Unit;

public class FileSystemTests
{
    [Theory]
    [InlineData("/test/file.txt", "File content")]
    [InlineData("/path/to/document.md", "# Markdown Document")]
    public async Task CreateFile(FilePath path, string content)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<IFileSystem, FakeFileSystem>();

        // When
        fake.CreateFile(path).SetContent(content);
        
        // Then
        await Verify(
            service.FromDirectoryPath(path.GetDirectory())
            );
    }

    [Theory]
    [InlineData("/test/directory")]
    [InlineData("/path/to/folder")]
    public async Task CreateDirectory(DirectoryPath path)
    {
        // Given
        var (service, fake) = ServiceProviderFixture.GetRequiredService<IFileSystem, FakeFileSystem>();

        // When
        fake.CreateDirectory(path);

        // Then
        await Verify(service.FromFileSystem());
    }
} 