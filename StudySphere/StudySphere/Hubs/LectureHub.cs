using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StudySphere.Models;
using StudySphere.Repositories.Interfaces;

namespace StudySphere.Hubs
{
    [Authorize(Roles = "Student,Instructor")]
    public class LectureHub : Hub
    {
        private static readonly ConcurrentDictionary<string, int> ActiveLectureByConnection = new();

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
            var lecture = GetLiveLecture(lectureId);
            var email = Context.User?.Identity?.Name;
            if (lecture is null || string.IsNullOrWhiteSpace(email))
            {
                throw new HubException("This live lecture is unavailable.");
            }

            if (Context.User!.IsInRole("Student"))
            {
                var student = _studentRepository.GetStudentByEmail(email);
                if (student is null ||
                    !_studentRepository.IsEnrolled(student.StudentId, lecture.CourseId))
                {
                    throw new HubException(
                        "You must be enrolled in this course to join.");
                }
            }
            else if (!IsOwningInstructor(email, lecture.InstructorId))
            {
                throw new HubException(
                    "Only the course instructor can join as an instructor.");
            }

            if (ActiveLectureByConnection.TryGetValue(
                    Context.ConnectionId,
                    out var activeLectureId) &&
                activeLectureId != lectureId)
            {
                throw new HubException(
                    "Leave your current lecture before joining another.");
            }

            var groupName = GetGroupName(lectureId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            ActiveLectureByConnection[Context.ConnectionId] = lectureId;
            await Clients.OthersInGroup(groupName)
                .SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task LeaveLecture(int lectureId)
        {
            EnsureJoinedLecture(lectureId);
            var groupName = GetGroupName(lectureId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            ActiveLectureByConnection.TryRemove(Context.ConnectionId, out _);
            await Clients.Group(groupName)
                .SendAsync("StudentLeft", Context.ConnectionId);
        }

        public Task SendOffer(
            int lectureId,
            string targetConnectionId,
            string offer)
        {
            EnsurePeerInLecture(lectureId, targetConnectionId);
            return Clients.Client(targetConnectionId)
                .SendAsync("ReceiveOffer", Context.ConnectionId, offer);
        }

        public Task SendAnswer(
            int lectureId,
            string targetConnectionId,
            string answer)
        {
            EnsurePeerInLecture(lectureId, targetConnectionId);
            return Clients.Client(targetConnectionId)
                .SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }

        public Task SendIceCandidate(
            int lectureId,
            string targetConnectionId,
            string candidate)
        {
            EnsurePeerInLecture(lectureId, targetConnectionId);
            return Clients.Client(targetConnectionId)
                .SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (ActiveLectureByConnection.TryRemove(
                    Context.ConnectionId,
                    out var lectureId))
            {
                var groupName = GetGroupName(lectureId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                await Clients.Group(groupName)
                    .SendAsync("StudentLeft", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private void EnsurePeerInLecture(int lectureId, string targetConnectionId)
        {
            EnsureJoinedLecture(lectureId);
            if (GetLiveLecture(lectureId) is null ||
                string.IsNullOrWhiteSpace(targetConnectionId) ||
                targetConnectionId == Context.ConnectionId ||
                !ActiveLectureByConnection.TryGetValue(
                    targetConnectionId,
                    out var targetLectureId) ||
                targetLectureId != lectureId)
            {
                throw new HubException(
                    "The other participant is not in this live lecture.");
            }
        }

        private void EnsureJoinedLecture(int lectureId)
        {
            if (!ActiveLectureByConnection.TryGetValue(
                    Context.ConnectionId,
                    out var activeLectureId) ||
                activeLectureId != lectureId)
            {
                throw new HubException(
                    "Join this live lecture before sending meeting data.");
            }
        }

        private LiveLecture? GetLiveLecture(int lectureId)
        {
            var lecture = _instructorRepository.GetLiveLectureById(lectureId);
            return lecture is not null &&
                lecture.Status == "Live" &&
                lecture.EndTime > DateTime.UtcNow
                    ? lecture
                    : null;
        }

        private bool IsOwningInstructor(string email, int instructorId)
        {
            var instructor = _instructorRepository.GetInstructorByEmail(email);
            return instructor is not null &&
                instructor.InstructorId == instructorId;
        }

        private static string GetGroupName(int lectureId) => $"Lecture_{lectureId}";
    }
}
