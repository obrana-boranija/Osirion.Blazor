using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.UnitOfWork;
using Shouldly;
using System.Reflection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.UnitOfWork;

/// <summary>
/// The unit of work copies real files, so every test runs in its own temporary sandbox directory.
/// </summary>
public sealed class FileSystemUnitOfWorkTests : IDisposable
{
    private readonly string _backupDirectory =
        Path.Combine(Path.GetTempPath(), "osirion-fs-uow-" + Guid.NewGuid().ToString("N"));

    private readonly FileSystemUnitOfWork _unitOfWork;

    public FileSystemUnitOfWorkTests()
    {
        _unitOfWork = new FileSystemUnitOfWork(
            Substitute.For<IContentRepository>(),
            Substitute.For<IDirectoryRepository>(),
            Substitute.For<IDomainEventDispatcher>(),
            NullLogger<FileSystemUnitOfWork>.Instance,
            _backupDirectory);
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
        if (System.IO.Directory.Exists(_backupDirectory))
        {
            System.IO.Directory.Delete(_backupDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task BeginTransactionAsync_ClearsModifiedFiles()
    {
        // Arrange
        var modifiedFiles = GetModifiedFiles();
        modifiedFiles.Add("test-file.txt");
        modifiedFiles.Count.ShouldBe(1);

        // Act
        await _unitOfWork.BeginTransactionAsync();

        // Assert
        modifiedFiles.Count.ShouldBe(0, "Modified files list should be cleared");
    }

    [Fact]
    public void TrackModifiedFile_WithNoTransaction_ThrowsInvalidOperationException()
    {
        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
            _unitOfWork.TrackModifiedFile("test-file.txt"));
    }

    [Fact]
    public async Task TrackModifiedFile_WithTransaction_AddsToModifiedFiles()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var testFilePath = WriteFile("test-file.txt", "original content");

        // Act
        _unitOfWork.TrackModifiedFile(testFilePath);

        // Assert
        GetModifiedFiles().ShouldContain(testFilePath);
        File.ReadAllText(BackupPath("test-file.txt")).ShouldBe("original content");
    }

    [Fact]
    public async Task CommitAsync_WithTransaction_DeletesBackupFiles()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var testFilePath = WriteFile("test-file.txt", "original content");
        _unitOfWork.TrackModifiedFile(testFilePath);
        File.Exists(BackupPath("test-file.txt")).ShouldBeTrue();

        // Act
        await _unitOfWork.CommitAsync();

        // Assert
        File.Exists(BackupPath("test-file.txt")).ShouldBeFalse("Backup file should be deleted after commit");
        File.ReadAllText(testFilePath).ShouldBe("original content");
        GetModifiedFiles().Count.ShouldBe(0, "Modified files list should be cleared after commit");
    }

    [Fact]
    public async Task RollbackAsync_WithTransaction_RestoresBackupFiles()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var testFilePath = WriteFile("test-file.txt", "original content");
        _unitOfWork.TrackModifiedFile(testFilePath);
        File.WriteAllText(testFilePath, "modified content");

        // Act
        await _unitOfWork.RollbackAsync();

        // Assert
        File.ReadAllText(testFilePath).ShouldBe("original content", "File should be restored from backup");
        File.Exists(BackupPath("test-file.txt")).ShouldBeFalse("Backup file should be deleted after rollback");
        GetModifiedFiles().Count.ShouldBe(0, "Modified files list should be cleared after rollback");
    }

    [Fact]
    public async Task SavePointAsync_StoresCurrentModifiedFilesState()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();
        var testFilePath = WriteFile("test-file.txt", "original content");
        _unitOfWork.TrackModifiedFile(testFilePath);

        // Act
        await _unitOfWork.SavePointAsync("test-savepoint");

        // Assert
        GetSavePoints().ShouldContainKey("test-savepoint");
    }

    [Fact]
    public async Task RollbackToSavePointAsync_RestoresFilesModifiedAfterSavePoint()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // The first file is backed up before the savepoint.
        var file1Path = WriteFile("file1.txt", "file1 original");
        _unitOfWork.TrackModifiedFile(file1Path);
        File.WriteAllText(file1Path, "file1 modified");

        await _unitOfWork.SavePointAsync("test-savepoint");

        // File times on disk are coarser than the clock behind the savepoint, so the savepoint is pinned to a known
        // instant: the first backup is dated before it and every backup made from now on is dated after it.
        var savePointTime = DateTime.UtcNow.AddHours(-1);
        File.SetCreationTimeUtc(BackupPath("file1.txt"), savePointTime.AddHours(-1));
        GetSavePoints()["test-savepoint"] = savePointTime.Ticks.ToString();

        // The second file is backed up after the savepoint.
        var file2Path = WriteFile("file2.txt", "file2 original");
        _unitOfWork.TrackModifiedFile(file2Path);
        File.WriteAllText(file2Path, "file2 modified");

        // Act
        await _unitOfWork.RollbackToSavePointAsync("test-savepoint");

        // Assert
        File.ReadAllText(file1Path).ShouldBe("file1 modified", "A file backed up before the savepoint keeps its changes");
        File.ReadAllText(file2Path).ShouldBe("file2 original", "A file backed up after the savepoint is restored");
        GetModifiedFiles().ShouldNotContain(file2Path);
    }

    private string WriteFile(string name, string content)
    {
        var path = Path.Combine(_backupDirectory, name);
        File.WriteAllText(path, content);
        return path;
    }

    private string BackupPath(string name) => Path.Combine(_backupDirectory, name + ".bak");

    private List<string> GetModifiedFiles() =>
        ReadField<List<string>>(typeof(FileSystemUnitOfWork), "_modifiedFiles");

    private Dictionary<string, string> GetSavePoints() =>
        ReadField<Dictionary<string, string>>(typeof(BaseUnitOfWork), "_savePoints");

    private T ReadField<T>(Type declaringType, string fieldName) where T : class
    {
        var field = declaringType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"The field {fieldName} was not found.");
        return field.GetValue(_unitOfWork) as T
            ?? throw new InvalidOperationException($"The field {fieldName} is not a {typeof(T).Name}.");
    }
}
