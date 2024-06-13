using CsvHelper;
using CsvHelper.Configuration;
using SalesSummary.Models;
using System.Globalization;

namespace SalesSummary.Services
{
    public class CsvService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CsvService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<SalesRecord>> ReadCsvAsync(string filePath)
        {
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);
            using var reader = new StreamReader(fullPath);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Trim().Replace(" ", "")
            };
            using var csv = new CsvReader(reader, config);
            var records = new List<SalesRecord>();
            await foreach (var record in csv.GetRecordsAsync<SalesRecord>())
            {
                records.Add(record);
            }
            return records;
        }
    }
}
