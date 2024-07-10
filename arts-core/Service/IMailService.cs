using arts_core.Models;
using arts_core.RequestModels;

namespace arts_core.Service
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailrequest);

        Task<bool> SendMail(MailRequestNhan request);
    }
}