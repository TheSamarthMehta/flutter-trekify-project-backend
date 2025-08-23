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
        private readonly List<Trek> _treks;

        public DataService()
        {
            _treks = LoadTreksFromExcel();
        }

        public async Task<List<Trek>> GetAllTreksAsync()
        {
            return await Task.FromResult(_treks);
        }

        public async Task<List<Trek>> GetTreksByStateAsync(string state)
        {
            var filtered = _treks.Where(t => 
                t.State.Contains(state, StringComparison.OrdinalIgnoreCase)).ToList();
            return await Task.FromResult(filtered);
        }

        public async Task<Trek?> GetTrekByIdAsync(int id)
        {
            var trek = _treks.FirstOrDefault(t => t.SerialNumber == id);
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
                               ?? Path.Combine(Directory.GetCurrentDirectory(), "..", "data", "Flutter Data Set.xlsx");
                
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), excelPath);
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Excel file not found at: {filePath}");
                    return treks;
                }

                using var package = new ExcelPackage(new FileInfo(filePath));
                var worksheet = package.Workbook.Worksheets[0]; // First worksheet
                
                var rowCount = worksheet.Dimension?.Rows ?? 0;
                
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Excel file: {ex.Message}");
            }
            
            return treks;
        }
    }
}
