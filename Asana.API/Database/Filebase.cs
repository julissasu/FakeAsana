using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Asana.API.Database
{
    public class Filebase
    {
        private readonly string? _rootDirectory; // Directory where JSON files are stored
        private readonly JsonSerializerOptions _jsonOptions; // Options for JSON serialization

        public Filebase(string? rootDirectory = null)
        {
            if (string.IsNullOrEmpty(rootDirectory))
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    _rootDirectory = @"C:\temp"; // Default for Windows
                }
                else
                {
                    _rootDirectory = Path.Combine(Path.GetTempPath(), "Filebase"); // Default for Unix-like systems
                }
            }
            else
            {
                _rootDirectory = rootDirectory; // Use provided directory
            }

            // Initialize JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            if (!string.IsNullOrEmpty(_rootDirectory))
            {
                EnsureDirectoryExists(_rootDirectory);
            }
        }

        // Save data to a JSON file asynchronously
        public async Task SaveAsync<T>(string tableName, T data)
        {
            try
            {
                var filePath = GetFilePath(tableName); // Get the full file path
                var json = JsonSerializer.Serialize(data, _jsonOptions); // Serialize data to JSON
                await Task.Run(() => File.WriteAllText(filePath, json)); // Write JSON to file
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to save data to table '{tableName}': {ex.Message}", ex);
            }
        }

        // Save data to a JSON file synchronously
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

        // Load data from a JSON file asynchronously
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

                return JsonSerializer.Deserialize<T>(json, _jsonOptions); // Deserialize JSON to object
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to load data from table '{tableName}': {ex.Message}", ex);
            }
        }

        // Load data from a JSON file synchronously
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

                return JsonSerializer.Deserialize<T>(json, _jsonOptions); // Deserialize JSON to object
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to load data from table '{tableName}': {ex.Message}", ex);
            }
        }

        // Delete a JSON file
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

        // Check if a JSON file exists
        public bool TableExists(string tableName)
        {
            var filePath = GetFilePath(tableName);
            return File.Exists(filePath);
        }

        // Get a list of all JSON files in the root directory
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
                        tableNames.Add(Path.GetFileNameWithoutExtension(file)); // Add file name without extension to the list
                    }
                }
                return tableNames;
            }
            catch (Exception ex)
            {
                throw new FilebaseException($"Failed to get table names: {ex.Message}", ex);
            }
        }

        // Get the full file path for a given table name
        private string GetFilePath(string tableName)
        {
            if (string.IsNullOrEmpty(_rootDirectory))
            {
                throw new FilebaseException("Root directory is not configured.");
            }
            return Path.Combine(_rootDirectory, $"{tableName}.json"); // Append .json extension to the table name
        }

        private void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }

    // Custom exception for Filebase operations
    public class FilebaseException : Exception
    {
        // Constructors
        public FilebaseException(string message) : base(message) { }
        public FilebaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
