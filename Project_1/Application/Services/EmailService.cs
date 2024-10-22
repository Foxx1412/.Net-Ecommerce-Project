using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
namespace Project_1.Application.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string messageBody)
    {
        if (string.IsNullOrEmpty(toEmail))
        {
            throw new ArgumentNullException(nameof(toEmail), "Địa chỉ email người nhận không được để trống.");
        }

        var fromEmail = _configuration["Smtp:Username"];
        if (string.IsNullOrEmpty(fromEmail))
        {
            throw new ArgumentNullException(nameof(fromEmail), "Địa chỉ email người gửi không được để trống.");
        }

        var host = _configuration["Smtp:Host"];
        var port = _configuration["Smtp:Port"];
        var password = _configuration["Smtp:Password"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Cấu hình SMTP không hợp lệ.");
        }

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Your App", fromEmail));
        emailMessage.To.Add(new MailboxAddress("", toEmail));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = messageBody };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(host, int.Parse(port), false);
                client.Authenticate(fromEmail, password);

                await client.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                // Log lỗi và/hoặc xử lý lỗi theo yêu cầu của ứng dụng
                throw new InvalidOperationException("Không thể gửi email.", ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }

}