using BusinessLogicLayer.IServices;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly FirebaseClient _firebaseClient;

        public FirebaseService()
        {
            _firebaseClient = new FirebaseClient(Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL"));
        }

        public async Task SendNotificationToUsersAsync(List<string> userIds, string title, string message, NotificationType type, string? warehouseId)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var tasks = userIds.Select(userId =>
                _firebaseClient
                    .Child("notifications")
                    .Child(userId)
                    .PostAsync(new
                    {
                        id = Guid.NewGuid().ToString(),
                        title,
                        message,
                        warehouseId,
                        type,
                        timestamp,
                        read = false
                    })
            );

            await Task.WhenAll(tasks);
        }
    }

    public enum NotificationType
    {
        GOOD_REQUEST_CREATED,
        GOOD_REQUEST_APPROVED,
        GOOD_REQUEST_REJECTED,
        GOOD_REQUEST_CONFIRMED,
        RECEIVE_NOTE_CREATED,
        ISSUE_NOTE_CREATED,
    }
}
