namespace SurvayBasket.Application.Services.Notification;

public interface INotificationService
{
    Task SendNewPollNotification(int? PollId = null);
}
