namespace StudySphere.Models.Dashboard
{
    public class StudentDashboardViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
