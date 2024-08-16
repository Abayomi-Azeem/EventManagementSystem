namespace EventManagementSystem.Api.Services.Interfaces;

public interface IEmailService
{
    void SendEventRegistrationEmail(string firstName, string email, string eventTitle);
}
