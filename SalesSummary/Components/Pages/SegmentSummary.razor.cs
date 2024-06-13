using Microsoft.AspNetCore.Components;
using SalesSummary.Models;
using SalesSummary.Services;
using System.Collections.Generic;

namespace SalesSummary.Components.Pages
{
    public partial class SegmentSummary : ComponentBase
    {
        [Inject]
        public CsvService CsvService { get; set; }

        private int SegmentCount;
        private List<string> UniqueSegments = new List<string>(); // Initialize as empty list
        private decimal TotalUnitsSold; // Changed to decimal
        private decimal TotalSales; // Changed to decimal
        private List<SalesRecord> allRecords;

        protected override async Task OnInitializedAsync()
        {
            allRecords = await CsvService.ReadCsvAsync("AppData/Data.csv");
            var segmentGroups = allRecords.GroupBy(r => r.Segment);

            SegmentCount = segmentGroups.Count();
            UniqueSegments = segmentGroups.Select(g => g.Key).ToList();

            TotalUnitsSold = segmentGroups.Sum(g => g.Sum(r => decimal.TryParse(r.UnitsSold, out var units) ? units : 0));
            
            TotalSales = segmentGroups.Sum(g => g.Sum(r =>
                decimal.TryParse(r.UnitsSold, out var units) &&
                decimal.TryParse(r.SalePrice, out var price) ? units * price : 0));
        }
    }
}
