using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendSmsAsync(string phoneNumber, string message);
        Task SendPushNotificationAsync(string deviceToken, string title, string message, Dictionary<string, string>? data = null);
        Task SendInAppNotificationAsync(Guid userId, string title, string message, string? actionUrl = null);
    }
}
