using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using TrekifyBackend.Data;
using TrekifyBackend.Models;

namespace TrekifyBackend.Services
{
    public interface IDataService
    {
        Task<List<Trek>> GetAllTreksAsync();
        Task<List<Trek>> GetTreksByStateAsync(string state);
        Task<Trek?> GetTrekByIdAsync(int id);
        Task SeedDataFromExcelAsync();
    }

    public class DataService : IDataService
    {
        private readonly TrekifyDbContext _context;

        public DataService(TrekifyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Trek>> GetAllTreksAsync()
        {
            // If no data in database, seed from Excel
            if (!await _context.Treks.AnyAsync())
            {
                await SeedDataFromExcelAsync();
            }
            
            return await _context.Treks.ToListAsync();
        }

        public async Task<List<Trek>> GetTreksByStateAsync(string state)
        {
            // If no data in database, seed from Excel
            if (!await _context.Treks.AnyAsync())
            {
                await SeedDataFromExcelAsync();
            }
            
            return await _context.Treks
                .Where(t => t.State.Contains(state))
                .ToListAsync();
        }

        public async Task<Trek?> GetTrekByIdAsync(int id)
        {
            // If no data in database, seed from Excel
            if (!await _context.Treks.AnyAsync())
            {
                await SeedDataFromExcelAsync();
            }
            
            return await _context.Treks.FirstOrDefaultAsync(t => t.SerialNumber == id);
        }

        public async Task SeedDataFromExcelAsync()
        {
            var treks = LoadTreksFromExcel();
            if (treks.Any())
            {
                await _context.Treks.AddRangeAsync(treks);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Seeded {treks.Count} treks from Excel to database");
            }
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
                
                Console.WriteLine($"Looking for Excel file at: {filePath}");
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Excel file not found at: {filePath}");
                    Console.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
                    Console.WriteLine($"Files in current directory: {string.Join(", ", Directory.GetFiles(Directory.GetCurrentDirectory()))}");
                    
                    // Try alternative paths
                    var alternativePaths = new[]
                    {
                        Path.Combine(Directory.GetCurrentDirectory(), "data", "Flutter Data Set.xlsx"),
                        Path.Combine(Directory.GetCurrentDirectory(), "..", "data", "Flutter Data Set.xlsx"),
                        Path.Combine("/app", "data", "Flutter Data Set.xlsx")
                    };
                    
                    foreach (var altPath in alternativePaths)
                    {
                        Console.WriteLine($"Trying alternative path: {altPath}");
                        if (File.Exists(altPath))
                        {
                            filePath = altPath;
                            Console.WriteLine($"Found Excel file at alternative path: {altPath}");
                            break;
                        }
                    }
                    
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("Excel file not found in any expected location");
                        return treks;
                    }
                }

                using var package = new ExcelPackage(new FileInfo(filePath));
                var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                
                var rowCount = worksheet.Dimension?.Rows ?? 0;
                Console.WriteLine($"Found {rowCount} rows in Excel file");
                
                // Skip header row (start from row 2)
                for (int row = 2; row <= rowCount; row++)
                {
                    var trek = new Trek
                    {
                        SerialNumber = int.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out int srNo) ? srNo : 0,
                        State = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                        TrekName = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        TrekType = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                        DifficultyLevel = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                        Season = worksheet.Cells[row, 6].Value?.ToString() ?? "",
                        Duration = worksheet.Cells[row, 7].Value?.ToString() ?? "",
                        Distance = worksheet.Cells[row, 8].Value?.ToString() ?? "",
                        MaxAltitude = worksheet.Cells[row, 9].Value?.ToString() ?? "",
                        TrekDescription = worksheet.Cells[row, 10].Value?.ToString() ?? "",
                        Image = worksheet.Cells[row, 11].Value?.ToString() ?? ""
                    };
                    
                    treks.Add(trek);
                }
                
                Console.WriteLine($"Loaded {treks.Count} treks from Excel file");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Excel file: {ex.Message}");
            }
            
            return treks;
        }
    }
}
