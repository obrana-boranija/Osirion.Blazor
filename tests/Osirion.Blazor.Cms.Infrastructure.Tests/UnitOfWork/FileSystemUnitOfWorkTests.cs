using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Infrastructure.UnitOfWork;
using Shouldly;
using System.Reflection;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.UnitOfWork;

public class FileSystemUnitOfWorkTests
{
    private readonly IContentRepository _contentRepository;
    private readonly IDirectoryRepository _directoryRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<FileSystemUnitOfWork> _logger;
    private readonly string _backupDirectory;
    private readonly FileSystemUnitOfWork _unitOfWork;
    private readonly MockFileSystem _mockFileSystem;

    public FileSystemUnitOfWorkTests()
    {
        _contentRepository = Substitute.For<IContentRepository>();
        _directoryRepository = Substitute.For<IDirectoryRepository>();
        _eventDispatcher = Substitute.For<IDomainEventDispatcher>();
        _logger = Substitute.For<ILogger<FileSystemUnitOfWork>>();
        _backupDirectory = Path.Combine(Path.GetTempPath(), "fs-unit-of-work-test");

        // Create a mock file system for testing
        _mockFileSystem = new MockFileSystem();

        _unitOfWork = new TestFileSystemUnitOfWork(
            _contentRepository,
            _directoryRepository,
            _eventDispatcher,
            _logger,
            _backupDirectory,
            _mockFileSystem);
    }

    [Fact]
    public async Task BeginTransactionAsync_ClearsModifiedFiles()
    {
        // Arrange
        var modifiedFilesField = typeof(FileSystemUnitOfWork)
            .GetField("_modifiedFiles", BindingFlags.NonPublic | BindingFlags.Instance);
        modifiedFilesField.ShouldNotBeNull();

        var modifiedFiles = (List<string>)modifiedFilesField.GetValue(_unitOfWork);
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

        // Setup test file
        var testFilePath = Path.Combine(_backupDirectory, "test-file.txt");
        _mockFileSystem.AddFile(testFilePath, "original content");

        // Act
        _unitOfWork.TrackModifiedFile(testFilePath);

        // Assert
        var modifiedFilesField = typeof(FileSystemUnitOfWork)
            .GetField("_modifiedFiles", BindingFlags.NonPublic | BindingFlags.Instance);
        modifiedFilesField.ShouldNotBeNull();

        var modifiedFiles = (List<string>)modifiedFilesField.GetValue(_unitOfWork);
        modifiedFiles.ShouldContain(testFilePath);

        // Verify backup was created
        var backupPath = Path.Combine(_backupDirectory, "test-file.txt.bak");
        _mockFileSystem.FileExists(backupPath).ShouldBeTrue("Backup file should be created");
    }

    [Fact]
    public async Task CommitAsync_WithTransaction_DeletesBackupFiles()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Setup test file and backup
        var testFilePath = Path.Combine(_backupDirectory, "test-file.txt");
        var backupPath = Path.Combine(_backupDirectory, "test-file.txt.bak");

        _mockFileSystem.AddFile(testFilePath, "original content");
        _mockFileSystem.AddFile(backupPath, "backup content");

        // Track the file
        _unitOfWork.TrackModifiedFile(testFilePath);

        // Act
        await _unitOfWork.CommitAsync();

        // Assert
        _mockFileSystem.FileExists(backupPath).ShouldBeFalse("Backup file should be deleted after commit");

        // Verify modified files list is cleared
        var modifiedFilesField = typeof(FileSystemUnitOfWork)
            .GetField("_modifiedFiles", BindingFlags.NonPublic | BindingFlags.Instance);
        var modifiedFiles = (List<string>)modifiedFilesField.GetValue(_unitOfWork);
        modifiedFiles.Count.ShouldBe(0, "Modified files list should be cleared after commit");
    }

    [Fact]
    public async Task RollbackAsync_WithTransaction_RestoresBackupFiles()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Setup test file and backup
        var testFilePath = Path.Combine(_backupDirectory, "test-file.txt");
        var backupPath = Path.Combine(_backupDirectory, "test-file.txt.bak");

        _mockFileSystem.AddFile(testFilePath, "modified content");
        _mockFileSystem.AddFile(backupPath, "original content");

        // Track the file
        _unitOfWork.TrackModifiedFile(testFilePath);

        // Act
        await _unitOfWork.RollbackAsync();

        // Assert
        var fileContent = _mockFileSystem.GetFileContent(testFilePath);
        fileContent.ShouldBe("original content", "File should be restored from backup");

        _mockFileSystem.FileExists(backupPath).ShouldBeFalse("Backup file should be deleted after rollback");

        // Verify modified files list is cleared
        var modifiedFilesField = typeof(FileSystemUnitOfWork)
            .GetField("_modifiedFiles", BindingFlags.NonPublic | BindingFlags.Instance);
        var modifiedFiles = (List<string>)modifiedFilesField.GetValue(_unitOfWork);
        modifiedFiles.Count.ShouldBe(0, "Modified files list should be cleared after rollback");
    }

    [Fact]
    public async Task SavePointAsync_StoresCurrentModifiedFilesState()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Setup test file
        var testFilePath = Path.Combine(_backupDirectory, "test-file.txt");
        _mockFileSystem.AddFile(testFilePath, "original content");

        // Track the file
        _unitOfWork.TrackModifiedFile(testFilePath);

        // Act
        await _unitOfWork.SavePointAsync("test-savepoint");

        // Assert
        var savePointsField = typeof(BaseUnitOfWork)
            .GetField("_savePoints", BindingFlags.NonPublic | BindingFlags.Instance);
        savePointsField.ShouldNotBeNull();

        var savePoints = (Dictionary<string, string>)savePointsField.GetValue(_unitOfWork);
        savePoints.ShouldContainKey("test-savepoint");
    }

    [Fact]
    public async Task RollbackToSavePointAsync_RestoresFilesModifiedAfterSavePoint()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Setup first file (before savepoint)
        var file1Path = Path.Combine(_backupDirectory, "file1.txt");
        var file1BackupPath = Path.Combine(_backupDirectory, "file1.txt.bak");
        _mockFileSystem.AddFile(file1Path, "file1 modified");
        _mockFileSystem.AddFile(file1BackupPath, "file1 original");
        _unitOfWork.TrackModifiedFile(file1Path);

        // Create savepoint
        await _unitOfWork.SavePointAsync("test-savepoint");

        // Get the savepoint timestamp
        var savePointsField = typeof(BaseUnitOfWork)
            .GetField("_savePoints", BindingFlags.NonPublic | BindingFlags.Instance);
        var savePoints = (Dictionary<string, string>)savePointsField.GetValue(_unitOfWork);
        var savePointTimestamp = long.Parse(savePoints["test-savepoint"]);

        // Setup second file (after savepoint) with a backup creation time after the savepoint
        var file2Path = Path.Combine(_backupDirectory, "file2.txt");
        var file2BackupPath = Path.Combine(_backupDirectory, "file2.txt.bak");
        _mockFileSystem.AddFile(file2Path, "file2 modified");
        _unitOfWork.TrackModifiedFile(file2Path);

        // Act
        await _unitOfWork.RollbackToSavePointAsync("test-savepoint");

        // Assert
        // File modified before savepoint should still be modified
        _mockFileSystem.GetFileContent(file1Path).ShouldBe("file1 modified");
        // File modified after savepoint should be restored
        _mockFileSystem.GetFileContent(file2Path).ShouldBe("file2 original");
    }

    // Test-specific mock file system class
    public class MockFileSystem
    {
        private Dictionary<string, MockFile> _files = new Dictionary<string, MockFile>();

        public void AddFile(string path, string content, DateTime? creationTime = null)
        {
            _files[path] = new MockFile
            {
                Content = content,
                CreationTime = creationTime ?? DateTime.UtcNow
            };
        }

        public bool FileExists(string path)
        {
            return _files.ContainsKey(path);
        }

        public string GetFileContent(string path)
        {
            if (!_files.ContainsKey(path))
                throw new FileNotFoundException($"File not found: {path}");

            return _files[path].Content;
        }

        public void CopyFile(string sourcePath, string destinationPath, bool overwrite)
        {
            if (!_files.ContainsKey(sourcePath))
                throw new FileNotFoundException($"Source file not found: {sourcePath}");

            _files[destinationPath] = new MockFile
            {
                Content = _files[sourcePath].Content,
                CreationTime = DateTime.UtcNow
            };
        }

        public void DeleteFile(string path)
        {
            _files.Remove(path);
        }
    }

    public class MockFile
    {
        public string Content { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    }

    // Test-specific FileSystemUnitOfWork that works with our mock file system
    public class TestFileSystemUnitOfWork : FileSystemUnitOfWork
    {
        private readonly MockFileSystem _mockFileSystem;

        public TestFileSystemUnitOfWork(
            IContentRepository contentRepository,
            IDirectoryRepository directoryRepository,
            IDomainEventDispatcher eventDispatcher,
            ILogger<FileSystemUnitOfWork> logger,
            string backupDirectory,
            MockFileSystem mockFileSystem)
            : base(contentRepository, directoryRepository, eventDispatcher, logger, backupDirectory)
        {
            _mockFileSystem = mockFileSystem;
        }

        // Override the file operations methods to use our mock file system
        public void TrackModifiedFile(string filePath)
        {
            if (!_transactionStarted)
                throw new InvalidOperationException("No transaction in progress");

            var modifiedFilesField = typeof(FileSystemUnitOfWork)
                .GetField("_modifiedFiles", BindingFlags.NonPublic | BindingFlags.Instance);
            var modifiedFiles = (List<string>)modifiedFilesField.GetValue(this);

            if (!modifiedFiles.Contains(filePath))
            {
                // Create backup if it doesn't exist
                var backupPath = GetBackupPath(filePath);
                if (!_mockFileSystem.FileExists(backupPath) && _mockFileSystem.FileExists(filePath))
                {
                    _mockFileSystem.CopyFile(filePath, backupPath, true);
                }

                modifiedFiles.Add(filePath);
            }
        }

        private string GetBackupPath(string filePath)
        {
            var getBackupPathMethod = typeof(FileSystemUnitOfWork)
                .GetMethod("GetBackupPath", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)getBackupPathMethod.Invoke(this, new object[] { filePath });
        }
    }
}