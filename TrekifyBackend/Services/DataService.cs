using OfficeOpenXml;
using TrekifyBackend.Models;

namespace TrekifyBackend.Services
{
    public interface IDataService
    {
        Task<List<Trek>> GetAllTreksAsync();
        Task<List<Trek>> GetTreksByStateAsync(string state);
        Task<Trek?> GetTrekByIdAsync(int id);
    }

    public class DataService : IDataService
    {
        private readonly ILogger<DataService> _logger;

        public DataService(ILogger<DataService> logger)
        {
            _logger = logger;
        }

        public async Task<List<Trek>> GetAllTreksAsync()
        {
            return await Task.FromResult(LoadTreksFromExcel());
        }

        public async Task<List<Trek>> GetTreksByStateAsync(string state)
        {
            var allTreks = LoadTreksFromExcel();
            var filtered = allTreks.Where(t => 
                t.State.Contains(state, StringComparison.OrdinalIgnoreCase)).ToList();
            return await Task.FromResult(filtered);
        }

        public async Task<Trek?> GetTrekByIdAsync(int id)
        {
            var allTreks = LoadTreksFromExcel();
            var trek = allTreks.FirstOrDefault(t => t.SerialNumber == id);
            return await Task.FromResult(trek);
        }

        private List<Trek> LoadTreksFromExcel()
        {
            var treks = new List<Trek>();
            
            try
            {
                // Set the EPPlus license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var excelPath = Environment.GetEnvironmentVariable("EXCEL_DATA_PATH") 
                               ?? "data/Flutter Data Set.xlsx";
                
                // For production (Docker), use relative path from app directory
                // For development, use path relative to current directory
                string filePath;
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), excelPath);
                }
                else
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", excelPath);
                }
                
                _logger.LogInformation($"Looking for Excel file at: {filePath}");
                
                if (!File.Exists(filePath))
                {
                    _logger.LogError($"Excel file not found at: {filePath}");
                    _logger.LogInformation($"Current directory: {Directory.GetCurrentDirectory()}");
                    
                    // Try alternative paths
                    var alternativePaths = new[]
                    {
                        Path.Combine(Directory.GetCurrentDirectory(), "data", "Flutter Data Set.xlsx"),
                        Path.Combine(Directory.GetCurrentDirectory(), "..", "data", "Flutter Data Set.xlsx"),
                        Path.Combine("/app", "data", "Flutter Data Set.xlsx")
                    };
                    
                    foreach (var altPath in alternativePaths)
                    {
                        _logger.LogInformation($"Trying alternative path: {altPath}");
                        if (File.Exists(altPath))
                        {
                            filePath = altPath;
                            _logger.LogInformation($"Found Excel file at alternative path: {altPath}");
                            break;
                        }
                    }
                    
                    if (!File.Exists(filePath))
                    {
                        _logger.LogError("Excel file not found in any expected location");
                        return treks;
                    }
                }

                using var package = new ExcelPackage(new FileInfo(filePath));
                var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                
                var rowCount = worksheet.Dimension?.Rows ?? 0;
                _logger.LogInformation($"Found {rowCount} rows in Excel file");
                
                // Skip header row (start from row 2)
                for (int row = 2; row <= rowCount; row++)
                {
                    // The columns appear to be swapped based on the data we're seeing
                    var state = worksheet.Cells[row, 3].Value?.ToString() ?? "";  // Column 3 has state names
                    var trekName = worksheet.Cells[row, 2].Value?.ToString() ?? "";  // Column 2 has trek names
                    
                    // Look for actual Cloudinary URLs in any column (most likely in columns 11-15)
                    string imageUrl = "";
                    for (int col = 1; col <= 15; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(cellValue) && 
                            (cellValue.Contains("cloudinary.com") || 
                             cellValue.Contains("res.cloudinary") ||
                             cellValue.StartsWith("https://res.cloudinary")))
                        {
                            imageUrl = cellValue;
                            if (row <= 10) _logger.LogInformation($"Found Cloudinary URL in column {col} for '{trekName}': {imageUrl}");
                            break;
                        }
                    }
                    
                    // Debug logging for first few rows to see structure
                    if (row <= 5)
                    {
                        _logger.LogInformation($"Row {row}: State='{state}', TrekName='{trekName}', ImageURL='{imageUrl}'");
                    }
                    
                    // Only use exact URLs from Excel - no fallback URLs
                    
                    var trek = new Trek
                    {
                        SerialNumber = int.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out int srNo) ? srNo : 0,
                        State = state,
                        TrekName = trekName,
                        TrekType = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        DifficultyLevel = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                        Season = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                        Duration = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                        Distance = worksheet.Cells[row, 8].Value?.ToString() ?? "",
                        MaxAltitude = worksheet.Cells[row, 9].Value?.ToString() ?? "",
                        TrekDescription = worksheet.Cells[row, 10].Value?.ToString() ?? "",
                        Image = imageUrl
                    };
                    
                    treks.Add(trek);
                }
                
                _logger.LogInformation($"Successfully loaded {treks.Count} treks from Excel file");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading Excel file: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
            }
            
            return treks;
        }

        private bool IsImageFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || 
                   extension == ".gif" || extension == ".bmp" || extension == ".webp" || 
                   extension == ".svg" || extension == ".avif";
        }
    }
}
