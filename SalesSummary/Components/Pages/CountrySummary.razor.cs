using Microsoft.AspNetCore.Components;
using SalesSummary.Models;
using SalesSummary.Services;

namespace SalesSummary.Components.Pages
{
    public partial class CountrySummary
    {
        [Inject]
        public CsvService CsvService { get; set; }

        private int CountryCount;
        private List<string> UniqueCountries = new List<string>(); // Initialize as empty list
        private decimal TotalUnitsSold; // Changed to decimal
        private List<SalesRecord> allRecords;

        protected override async Task OnInitializedAsync()
        {
            allRecords = await CsvService.ReadCsvAsync("AppData/Data.csv");
            var countryGroups = allRecords.GroupBy(r => r.Country);

            CountryCount = countryGroups.Count();
            UniqueCountries = countryGroups.Select(g => g.Key).ToList();

            TotalUnitsSold = countryGroups.Sum(g => g.Sum(r => decimal.TryParse(r.UnitsSold, out var units) ? units : 0));
        }

    }
}