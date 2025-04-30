using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Events;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.UnitOfWork
{
    /// <summary>
    /// File system implementation of the Unit of Work pattern
    /// </summary>
    public class FileSystemUnitOfWork : BaseUnitOfWork
    {
        private readonly string _backupDirectory;
        private readonly List<string> _modifiedFiles = new();

        public FileSystemUnitOfWork(
            IContentRepository contentRepository,
            IDirectoryRepository directoryRepository,
            IDomainEventDispatcher eventDispatcher,
            ILogger<FileSystemUnitOfWork> logger,
            string backupDirectory)
            : base(contentRepository, directoryRepository, "filesystem", logger, eventDispatcher)
        {
            _backupDirectory = backupDirectory ?? throw new ArgumentNullException(nameof(backupDirectory));

            // Ensure backup directory exists
            System.IO.Directory.CreateDirectory(_backupDirectory);
        }

        protected override Task OnBeginTransactionAsync(CancellationToken cancellationToken)
        {
            _modifiedFiles.Clear();
            return Task.CompletedTask;
        }

        protected override Task OnCommitTransactionAsync(CancellationToken cancellationToken)
        {
            // For file system, committing just means keeping the changes
            // Clean up any backups made
            foreach (var file in _modifiedFiles)
            {
                var backupPath = GetBackupPath(file);
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
            }

            _modifiedFiles.Clear();
            return Task.CompletedTask;
        }

        protected override Task OnRollbackTransactionAsync(CancellationToken cancellationToken)
        {
            // Restore backups of modified files
            foreach (var file in _modifiedFiles)
            {
                var backupPath = GetBackupPath(file);
                if (File.Exists(backupPath))
                {
                    File.Copy(backupPath, file, true);
                    File.Delete(backupPath);
                }
            }

            _modifiedFiles.Clear();
            return Task.CompletedTask;
        }

        protected override Task OnCreateSavePointAsync(string savePointName, CancellationToken cancellationToken)
        {
            // For savepoints, we just store the current list of modified files
            return Task.CompletedTask;
        }

        protected override Task OnRollbackToSavePointAsync(string savePointName, CancellationToken cancellationToken)
        {
            // Get the files modified after the savepoint
            var savePointState = _savePoints[savePointName];
            var savePointTimestamp = long.Parse(savePointState);

            // Find files backed up after the savepoint
            var backupsToRestore = System.IO.Directory.GetFiles(_backupDirectory, "*.bak")
                .Where(f => new FileInfo(f).CreationTimeUtc.Ticks > savePointTimestamp)
                .ToList();

            // Restore these files from backup
            foreach (var backupFile in backupsToRestore)
            {
                var originalFile = backupFile.Substring(0, backupFile.Length - 4); // Remove .bak
                if (File.Exists(backupFile))
                {
                    File.Copy(backupFile, originalFile, true);
                    File.Delete(backupFile);
                }

                // Remove from modified files list
                _modifiedFiles.Remove(originalFile);
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
                var backupPath = GetBackupPath(filePath);
                if (!File.Exists(backupPath) && File.Exists(filePath))
                {
                    File.Copy(filePath, backupPath, true);
                }

                _modifiedFiles.Add(filePath);
            }
        }

        private string GetBackupPath(string filePath)
        {
            return Path.Combine(_backupDirectory, Path.GetFileName(filePath) + ".bak");
        }
    }
}