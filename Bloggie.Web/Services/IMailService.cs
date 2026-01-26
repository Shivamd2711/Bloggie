using Bloggie.Web.Models.EmailModels;

namespace Bloggie.Web.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailRequestModel mailRequest);

    }
}
