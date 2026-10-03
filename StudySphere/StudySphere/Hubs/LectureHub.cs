using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Hubs
{
    [Authorize(Roles = "Student,Instructor")]
    public class LectureHub : Hub
    {
        private readonly IStudentDashboardRepository _studentRepository;
        private readonly IInstructorDashboardRepository _instructorRepository;

        public LectureHub(
            IStudentDashboardRepository studentRepository,
            IInstructorDashboardRepository instructorRepository)
        {
            _studentRepository = studentRepository;
            _instructorRepository = instructorRepository;
        }

        public async Task JoinLecture(int lectureId)
        {
            var lecture =
                _instructorRepository.GetLiveLectureById(lectureId);
            var email = Context.User?.Identity?.Name;
            if (lecture is null || lecture.Status != "Live" ||
                string.IsNullOrWhiteSpace(email))
            {
                throw new HubException("This live lecture is unavailable.");
            }

            if (Context.User!.IsInRole("Student"))
            {
                var student = _studentRepository.GetStudentByEmail(email);
                if (student is null ||
                    !_studentRepository.IsEnrolled(
                        student.StudentId,
                        lecture.CourseId))
                {
                    throw new HubException(
                        "You must be enrolled in this course to join.");
                }
            }
            else
            {
                var instructor =
                    _instructorRepository.GetInstructorByEmail(email);
                if (instructor is null ||
                    instructor.InstructorId != lecture.InstructorId)
                {
                    throw new HubException(
                        "Only the course instructor can join as an instructor.");
                }
            }

            string groupName = $"Lecture_{lectureId}";

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                groupName
            );

            // Tell existing users that someone joined
            await Clients.OthersInGroup(groupName)
                .SendAsync(
                    "UserJoined",
                    Context.ConnectionId
                );
        }

        public async Task LeaveLecture(int lectureId)
        {
            string groupName = $"Lecture_{lectureId}";

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                groupName
            );

            await Clients.Group(groupName)
                .SendAsync(
                    "StudentLeft",
                    Context.ConnectionId
                );
        }
        public async Task SendOffer(
            int lectureId,
            string targetConnectionId,
            string offer)
        {
            await Clients.Client(targetConnectionId)
                .SendAsync(
                    "ReceiveOffer",
                    Context.ConnectionId,
                    offer
                );
        }

        public async Task SendAnswer(
            string targetConnectionId,
            string answer)
        {
            await Clients.Client(targetConnectionId)
                .SendAsync(
                    "ReceiveAnswer",
                    Context.ConnectionId,
                    answer
                );
        }

        public async Task SendIceCandidate(
            string targetConnectionId,
            string candidate)
        {
            await Clients.Client(targetConnectionId)
                .SendAsync(
                    "ReceiveIceCandidate",
                    Context.ConnectionId,
                    candidate
                );
        }
    }
}