using Microsoft.AspNetCore.Components;
using SalesSummary.Models;
using SalesSummary.Services;


namespace SalesSummary.Components.Pages
{
    public partial class ProductSummary
    {

        [Inject]
        public CsvService CsvService { get; set; }
        private int ProductCount;
        private List<string> UniqueProducts = new List<string>(); // Initialize as empty list
        private decimal TotalUnitsSold; // Changed to decimal
        private List<SalesRecord> allRecords;

        protected override async Task OnInitializedAsync()
        {
            allRecords = await CsvService.ReadCsvAsync("AppData/Data.csv");
            var productGroups = allRecords.GroupBy(r => r.Product);

            ProductCount = productGroups.Count();
            UniqueProducts = productGroups.Select(g => g.Key).ToList();

            TotalUnitsSold = productGroups.Sum(g => g.Sum(r => decimal.TryParse(r.UnitsSold, out var units) ? units : 0));
        }
    }
}