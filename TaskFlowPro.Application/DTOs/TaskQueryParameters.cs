namespace TaskFlowPro.Application.DTOs // Define namespace for application data transfer objects
{
    public class TaskQueryParameters // Define class holding query parameters for filtering and paging tasks
    {
        private int _pageNumber = 1; // Private backing field for page number initialized to default value 1
        public int PageNumber // Public property representing the current requested page index
        {
            get => _pageNumber; // Return backing field value
            set => _pageNumber = value < 1 ? 1 : value; // Ensure page number cannot be less than 1
        }

        private int _pageSize = 10; // Private backing field for page size initialized to default value 10 items per page
        public int PageSize // Public property representing the number of items per page
        {
            get => _pageSize; // Return backing field value
            set => _pageSize = value > 50 ? 50 : (value < 1 ? 1 : value); // Clamp page size between 1 and maximum 50 items
        }

        public int? ProjectId { get; set; } // Optional filter property to filter tasks by a specific project ID

        public int? AssignedUserId { get; set; } // Optional filter property to filter tasks by an assigned user ID

        public bool? IsCompleted { get; set; } // Optional filter property to filter tasks by completion status boolean

        public string? SearchTerm { get; set; } // Optional search string property to filter tasks by title matching
    }
}