using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Asana.API.Database
{
    /// <summary>
    /// A simple file-based database that stores data as JSON files in the local file system
    /// </summary>
    public class Filebase
    {
        private readonly string? _rootDirectory;
        private readonly JsonSerializerOptions _jsonOptions;

        public Filebase(string? rootDirectory = null)
        {
            // Use provided directory or default to platform-appropriate temp directory
            if (string.IsNullOrEmpty(rootDirectory))
            {
                // For Windows, use C:\temp as specified in requirements
                // For other platforms, use system temp directory + Filebase folder
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    _rootDirectory = @"C:\temp";
                }
                else
                {
                    _rootDirectory = Path.Combine(Path.GetTempPath(), "Filebase");
                }
            }
            else
            {
                _rootDirectory = rootDirectory;
            }

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            // Ensure the root directory exists
            if (!string.IsNullOrEmpty(_rootDirectory))
            {
                EnsureDirectoryExists(_rootDirectory);
            }
        }

        /// <summary>
        /// Save data to a JSON file
        /// </summary>
        /// <typeparam name="T">Type of data to save</typeparam>
        /// <param name="tableName">Name of the table (file)</param>
        /// <param name="data">Data to save</param>
        public async Task SaveAsync<T>(string tableName, T data)
        {
            try
            {
                var filePath = GetFilePath(tableName);
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                await Task.Run(() => File.WriteAllText(filePath, json));
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to save data to table '{tableName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Save data to a JSON file synchronously
        /// </summary>
        /// <typeparam name="T">Type of data to save</typeparam>
        /// <param name="tableName">Name of the table (file)</param>
        /// <param name="data">Data to save</param>
        public void Save<T>(string tableName, T data)
        {
            try
            {
                var filePath = GetFilePath(tableName);
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to save data to table '{tableName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Load data from a JSON file
        /// </summary>
        /// <typeparam name="T">Type of data to load</typeparam>
        /// <param name="tableName">Name of the table (file)</param>
        /// <returns>Loaded data or default value if file doesn't exist</returns>
        public async Task<T?> LoadAsync<T>(string tableName) where T : class
        {
            try
            {
                var filePath = GetFilePath(tableName);
                
                if (!File.Exists(filePath))
                {
                    return null;
                }

                var json = await Task.Run(() => File.ReadAllText(filePath));
                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to load data from table '{tableName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Load data from a JSON file synchronously
        /// </summary>
        /// <typeparam name="T">Type of data to load</typeparam>
        /// <param name="tableName">Name of the table (file)</param>
        /// <returns>Loaded data or default value if file doesn't exist</returns>
        public T? Load<T>(string tableName) where T : class
        {
            try
            {
                var filePath = GetFilePath(tableName);
                
                if (!File.Exists(filePath))
                {
                    return null;
                }

                var json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to load data from table '{tableName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete a table (file)
        /// </summary>
        /// <param name="tableName">Name of the table to delete</param>
        public void DeleteTable(string tableName)
        {
            try
            {
                var filePath = GetFilePath(tableName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to delete table '{tableName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if a table (file) exists
        /// </summary>
        /// <param name="tableName">Name of the table to check</param>
        /// <returns>True if table exists, false otherwise</returns>
        public bool TableExists(string tableName)
        {
            var filePath = GetFilePath(tableName);
            return File.Exists(filePath);
        }

        /// <summary>
        /// Get all table names (file names without extension)
        /// </summary>
        /// <returns>List of table names</returns>
        public List<string> GetTableNames()
        {
            try
            {
                var tableNames = new List<string>();
                if (Directory.Exists(_rootDirectory))
                {
                    var files = Directory.GetFiles(_rootDirectory, "*.json");
                    foreach (var file in files)
                    {
                        tableNames.Add(Path.GetFileNameWithoutExtension(file));
                    }
                }
                return tableNames;
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to get table names: {ex.Message}", ex);
            }
        }

        private string GetFilePath(string tableName)
        {
            if (string.IsNullOrEmpty(_rootDirectory))
            {
                throw new FilebaseException("Root directory is not configured.");
            }
            return Path.Combine(_rootDirectory, $"{tableName}.json");
        }

        private void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }

    /// <summary>
    /// Exception thrown by Filebase operations
    /// </summary>
    public class FilebaseException : Exception
    {
        public FilebaseException(string message) : base(message) { }
        public FilebaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
