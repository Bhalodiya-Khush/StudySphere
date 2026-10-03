namespace StudySphere.Models.ViewModels
{
    public class NotificationViewModel
    {
        public List<NotificationItemViewModel> Items { get; set; } = new();
    }

    public class NotificationItemViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string Url { get; set; } = string.Empty;
    }
}
