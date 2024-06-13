using Microsoft.AspNetCore.Components;
using SalesSummary.Models;
using SalesSummary.Services;

namespace SalesSummary.Components.Pages
{
    public partial class DiscountSummary
    {
        [Inject]
        public CsvService CsvService { get; set; }
        private decimal TotalUnitsSold; // Changed to decimal
        private List<SalesRecord> allRecords;

        protected override async Task OnInitializedAsync()
        {
            allRecords = await CsvService.ReadCsvAsync("AppData/Data.csv");
            var discountGroups = allRecords.GroupBy(r => r.DiscountBand);

            TotalUnitsSold = discountGroups.Sum(g => g.Sum(r => decimal.TryParse(r.UnitsSold, out var units) ? units : 0));
        }
    }
}