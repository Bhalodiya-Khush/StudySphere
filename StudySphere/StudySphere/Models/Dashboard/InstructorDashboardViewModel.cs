namespace StudySphere.Models.Dashboard
{
    public class InstructorDashboardViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
