using System.Text;

namespace Cake.Bridge.DependencyInjection.Testing;

/// <summary>
/// Provides extension methods for file system operations.
/// </summary>
public static class FileSystemExtensions
{
    /// <summary>
    /// Creates a <see cref="DirectoryResult"/> from a <see cref="DirectoryPath"/>.
    /// </summary>
    /// <param name="fileSystem">The file system to operate on.</param>
    /// <returns>A <see cref="DirectoryResult"/> representing the directory.</returns>
    public static DirectoryResult FromFileSystem(this IFileSystem fileSystem)
        => DirectoryResult.FromDirectoryPath(fileSystem, fileSystem.GetDirectory("/").Path);

    /// <summary>
    /// Creates a <see cref="DirectoryResult"/> from a <see cref="DirectoryPath"/>.
    /// </summary>
    /// <param name="fileSystem">The file system to operate on.</param>
    /// <param name="directoryPath">The path of the directory.</param>
    /// <returns>A <see cref="DirectoryResult"/> representing the directory.</returns>
    public static DirectoryResult FromDirectoryPath(this IFileSystem fileSystem, DirectoryPath directoryPath)
        => DirectoryResult.FromDirectoryPath(fileSystem, directoryPath);

    /// <summary>
    /// Represents the result of a directory operation.
    /// </summary>
    /// <param name="Path">The path of the directory.</param>
    /// <param name="Exists">Indicates whether the directory exists.</param>
    /// <param name="Hidden">Indicates whether the directory is hidden.</param>
    /// <param name="Directories">The subdirectories within the directory.</param>
    /// <param name="Files">The files within the directory.</param>
    public record struct DirectoryResult(
        DirectoryPath Path,
        bool Exists,
        bool Hidden,
        DirectoryResult[] Directories,
        FileResult[] Files
        )
    {
        /// <summary>
        /// Creates a <see cref="DirectoryResult"/> from a <see cref="DirectoryPath"/>.
        /// </summary>
        /// <param name="fileSystem">The file system to operate on.</param>
        /// <param name="directoryPath">The path of the directory.</param>
        /// <returns>A <see cref="DirectoryResult"/> representing the directory.</returns>
        public static DirectoryResult FromDirectoryPath(IFileSystem fileSystem, DirectoryPath directoryPath)
                => FromDirectory(fileSystem.GetDirectory(directoryPath));

        /// <summary>
        /// Creates a <see cref="DirectoryResult"/> from an <see cref="IDirectory"/>.
        /// </summary>
        /// <param name="directory">The directory to convert.</param>
        /// <returns>A <see cref="DirectoryResult"/> representing the directory.</returns>
        public static DirectoryResult FromDirectory(IDirectory directory)
                 => new(
                     directory.Path,
                     directory.Exists,
                     directory.Hidden,
                     [.. directory
                         .GetDirectories(string.Empty, SearchScope.Current)
                         .OrderBy(directories => directories.Path.FullPath, StringComparer.OrdinalIgnoreCase)
                         .Select(FromDirectory)],
                     [.. directory
                         .GetFiles(string.Empty, SearchScope.Current)
                         .OrderBy(file => file.Path.FullPath, StringComparer.OrdinalIgnoreCase)
                         .Select(FileResult.FromFile)]
                 );
    }

    /// <summary>
    /// Represents the result of a file operation.
    /// </summary>
    /// <param name="Path">The path of the file.</param>
    /// <param name="Exists">Indicates whether the file exists.</param>
    /// <param name="Hidden">Indicates whether the file is hidden.</param>
    /// <param name="Length">The length of the file in characters.</param>
    public record struct FileResult(
        FilePath Path,
        bool Exists,
        bool Hidden,
        long Length
        )
    {
        /// <summary>
        /// Creates a <see cref="FileResult"/> from an <see cref="IFile"/>.
        /// </summary>
        /// <param name="file">The file to convert.</param>
        /// <returns>A <see cref="FileResult"/> representing the file.</returns>
        public static FileResult FromFile(IFile file)
              => new(
                  file.Path,
                  file.Exists,
                  file.Hidden,
                  file
                    .ReadLines(Encoding.UTF8)
                    .SelectMany(c => c)
                    .LongCount()
                  );
    }
}