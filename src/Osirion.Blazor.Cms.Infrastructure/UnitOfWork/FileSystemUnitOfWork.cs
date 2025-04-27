using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.UnitOfWork;

/// <summary>
/// File system implementation of the Unit of Work pattern
/// </summary>
public class FileSystemUnitOfWork : IUnitOfWork
{
    private readonly IContentRepository _contentRepository;
    private readonly IDirectoryRepository _directoryRepository;
    private readonly ILogger<FileSystemUnitOfWork> _logger;
    private bool _transactionStarted = false;
    private readonly string _backupDirectory;
    private readonly List<string> _modifiedFiles = new();
    private readonly Dictionary<string, string> _savePoints = new();

    public FileSystemUnitOfWork(
        IContentRepository contentRepository,
        IDirectoryRepository directoryRepository,
        ILogger<FileSystemUnitOfWork> logger,
        string backupDirectory)
    {
        _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        _directoryRepository = directoryRepository ?? throw new ArgumentNullException(nameof(directoryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _backupDirectory = backupDirectory ?? throw new ArgumentNullException(nameof(backupDirectory));

        // Ensure backup directory exists
        Directory.CreateDirectory(_backupDirectory);
    }

    public IContentRepository ContentRepository => _contentRepository;

    public IDirectoryRepository DirectoryRepository => _directoryRepository;

    public string ProviderId => "filesystem";

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transactionStarted)
            throw new InvalidOperationException("Transaction already started");

        _transactionStarted = true;
        _modifiedFiles.Clear();
        _savePoints.Clear();

        _logger.LogInformation("Started file system transaction");
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction in progress");

        try
        {
            // For file system, committing just means keeping the changes
            // Clean up any backups made
            foreach (var file in _modifiedFiles)
            {
                var backupPath = Path.Combine(_backupDirectory, Path.GetFileName(file) + ".bak");
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
            }

            _transactionStarted = false;
            _modifiedFiles.Clear();
            _savePoints.Clear();

            _logger.LogInformation("Committed file system transaction");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing file system transaction");
            throw;
        }

        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction in progress");

        try
        {
            // Restore backups of modified files
            foreach (var file in _modifiedFiles)
            {
                var backupPath = Path.Combine(_backupDirectory, Path.GetFileName(file) + ".bak");
                if (File.Exists(backupPath))
                {
                    File.Copy(backupPath, file, true);
                    File.Delete(backupPath);
                }
            }

            _transactionStarted = false;
            _modifiedFiles.Clear();
            _savePoints.Clear();

            _logger.LogInformation("Rolled back file system transaction");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back file system transaction");
            throw;
        }

        return Task.CompletedTask;
    }

    public Task SavePointAsync(string savePointName, CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction in progress");

        if (string.IsNullOrEmpty(savePointName))
            throw new ArgumentException("Savepoint name cannot be empty", nameof(savePointName));

        if (_savePoints.ContainsKey(savePointName))
            throw new ArgumentException($"Savepoint '{savePointName}' already exists");

        // Store current savepoint state as a list of modified files
        _savePoints[savePointName] = string.Join("|", _modifiedFiles);

        _logger.LogInformation("Created savepoint {SavePoint} with {FileCount} modified files",
            savePointName, _modifiedFiles.Count);

        return Task.CompletedTask;
    }

    public Task RollbackToSavePointAsync(string savePointName, CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction in progress");

        if (!_savePoints.TryGetValue(savePointName, out var savePointState))
            throw new ArgumentException($"Savepoint '{savePointName}' does not exist");

        try
        {
            var savePointFiles = savePointState.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Find files modified after the savepoint
            var filesToRestore = _modifiedFiles
                .Except(savePointFiles)
                .ToList();

            // Restore these files from backup
            foreach (var file in filesToRestore)
            {
                var backupPath = Path.Combine(_backupDirectory, Path.GetFileName(file) + ".bak");
                if (File.Exists(backupPath))
                {
                    File.Copy(backupPath, file, true);
                }

                // Remove from modified files list
                _modifiedFiles.Remove(file);
            }

            // Remove savepoints that came after this one
            var savePointsToRemove = new List<string>();
            bool foundCurrentSavepoint = false;

            foreach (var kvp in _savePoints)
            {
                if (kvp.Key == savePointName)
                {
                    foundCurrentSavepoint = true;
                    continue;
                }

                if (foundCurrentSavepoint)
                {
                    savePointsToRemove.Add(kvp.Key);
                }
            }

            foreach (var sp in savePointsToRemove)
            {
                _savePoints.Remove(sp);
            }

            _logger.LogInformation("Rolled back to savepoint {SavePoint}, restored {FileCount} files",
                savePointName, filesToRestore.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back to savepoint {SavePoint}", savePointName);
            throw;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Tracks a modified file in the current transaction
    /// </summary>
    public void TrackModifiedFile(string filePath)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction in progress");

        if (!_modifiedFiles.Contains(filePath))
        {
            // Create backup if it doesn't exist
            var backupPath = Path.Combine(_backupDirectory, Path.GetFileName(filePath) + ".bak");
            if (!File.Exists(backupPath) && File.Exists(filePath))
            {
                File.Copy(filePath, backupPath, true);
                _logger.LogDebug("Created backup for file {FilePath}", filePath);
            }

            _modifiedFiles.Add(filePath);
        }
    }

    public void Dispose()
    {
        // If transaction is still active, roll it back
        if (_transactionStarted)
        {
            try
            {
                _logger.LogWarning("Transaction still active during disposal, rolling back");
                RollbackAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back transaction during disposal");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        // If transaction is still active, roll it back
        if (_transactionStarted)
        {
            try
            {
                _logger.LogWarning("Transaction still active during async disposal, rolling back");
                await RollbackAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rolling back transaction during async disposal");
            }
        }
    }
}