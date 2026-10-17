using Microsoft.AspNetCore.Components;
using VehicleCategoryManager.Client.Components;

namespace VehicleCategoryManager.Client.Pages
{
    public class BasePage : ComponentBase
    {
        protected DeleteConfirmationModal? deleteModal { get; set; }

        protected string SortColumn = string.Empty;
        protected bool SortAscending = true;

        protected string? errorMessage;
        protected string? loadErrorMessage;
        protected bool isModalOpen;
        protected bool isDeletionModalOpen;

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

        protected virtual async Task PromptDelete(int recordId)
        {
            errorMessage = null;

            if (deleteModal != null)
            {
                deleteModal.RecordId = recordId;
                isDeletionModalOpen = true;
            }
            else
            {
                errorMessage = "Unable to open deletion confirmation form, please try refreshing the page.";
            }
        }
    }
}
