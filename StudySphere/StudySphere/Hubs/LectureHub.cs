using Microsoft.AspNetCore.SignalR;

namespace StudySphere.Hubs
{
    public class LectureHub : Hub
    {
        public async Task JoinLecture(int lectureId)
        {
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