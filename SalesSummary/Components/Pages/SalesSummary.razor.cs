using Microsoft.AspNetCore.Components;
using SalesSummary.Models;
using SalesSummary.Services;
using SalesSummary.Helpers;

namespace SalesSummary.Components.Pages
{
    public partial class SalesSummary : ComponentBase
    {
        [Inject]
        public CsvService CsvService { get; set; }

        protected List<SalesRecord> allRecords = new List<SalesRecord>();
        protected List<SalesRecord> PagedRecords = new List<SalesRecord>();
        private string _searchTerm = "";
        protected int currentPage = 1;
        protected int rowsPerPage = 10; // Ensure initial value is set
        protected int totalPages = 1;
        protected string sortColumn = nameof(SalesRecord.Segment);
        protected bool sortAscending = true;
        protected bool isLoading = true;


        protected string searchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    FilterAndSortRecords();
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            allRecords = await CsvService.ReadCsvAsync("AppData/Data.csv");
            FilterAndSortRecords();
            isLoading = false;
        }

        protected void SortByColumn(string columnName)
        {
            SortByColumnHandler(columnName);
        }

        protected void PreviousPage()
        {
            PreviousPageHandler();
        }

        protected void NextPage()
        {
            NextPageHandler();
        }

        protected async Task RowsPerPageChanged(ChangeEventArgs e)
        {
            rowsPerPage = int.Parse(e.Value.ToString());
            currentPage = 1; 
            FilterAndSortRecords();
            await InvokeAsync(StateHasChanged); 
        }

        protected void SortByColumnHandler(string columnName)
        {
            if (sortColumn == columnName)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                sortColumn = columnName;
                sortAscending = true;
            }
            FilterAndSortRecords();
        }

        protected async Task FilterAndSortRecords()
        {
            var filteredRecords = allRecords
                .Where(r => string.IsNullOrEmpty(searchTerm) ||
                            r.Segment.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            r.Country.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            r.Product.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            r.DiscountBand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .AsQueryable();

            filteredRecords = filteredRecords.OrderByDynamic(sortColumn, sortAscending);

            totalPages = (int)Math.Ceiling((double)filteredRecords.Count() / rowsPerPage);
            PagedRecords = filteredRecords
                .Skip((currentPage - 1) * rowsPerPage)
                .Take(rowsPerPage)
                .ToList();

            await InvokeAsync(StateHasChanged);
        }


        protected void NextPageHandler()
        {
            if (HasNextPage)
            {
                currentPage++;
                FilterAndSortRecords();
            }
        }

        protected void PreviousPageHandler()
        {
            if (HasPreviousPage)
            {
                currentPage--;
                FilterAndSortRecords();
            }
        }

        protected void FirstPage()
        {
            currentPage = 1;
            FilterAndSortRecords();
        }

        protected void LastPage()
        {
            currentPage = totalPages;
            FilterAndSortRecords();
        }

        protected void ResetSearch()
        {
            searchTerm = string.Empty;
            currentPage = 1;
            FilterAndSortRecords();
        }
        protected bool HasPreviousPage => currentPage > 1;
        protected bool HasNextPage => currentPage < totalPages;
    }
}
