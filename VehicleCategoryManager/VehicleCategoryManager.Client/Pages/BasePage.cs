using Microsoft.AspNetCore.Components;

namespace VehicleCategoryManager.Client.Pages
{
    public class BasePage : ComponentBase
    {

        protected string SortColumn;
        protected bool SortAscending = true;

        protected string? errorMessage;
        protected bool isModalOpen;

        protected void OpenModal() => isModalOpen = true;

        protected void SortBy(string column)
        {
            if (SortColumn == column)
            {
                SortAscending = !SortAscending;
            }
            else
            {
                SortColumn = column;
                SortAscending = true;
            }
        }

        protected string SortIcon(string column)
        {
            if (SortColumn != column)
                return "";

            return SortAscending ? "▲" : "▼";
        }
    }
}
