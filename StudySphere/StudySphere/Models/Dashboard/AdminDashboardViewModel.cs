namespace StudySphere.Models.Dashboard
{
    public class AdminDashboardViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
