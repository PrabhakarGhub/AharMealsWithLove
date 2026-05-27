using System.Net;
using System.Net.Mail;

namespace AharMealsWithLove.Data
{
    public static class EmailService
    {
        private static readonly IConfiguration? _config;

        static EmailService()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }

        public static (bool success, string message) SendEmail(
            string toEmail, string subject, string body)
        {
            try
            {
                string smtpHost = _config?["AppSettings:SmtpHost"] ?? "smtp.gmail.com";
                int smtpPort = int.Parse(_config?["AppSettings:SmtpPort"] ?? "587");
                string smtpUser = _config?["AppSettings:SmtpUser"] ?? "";
                string smtpPass = _config?["AppSettings:SmtpPass"] ?? "";

                if (string.IsNullOrEmpty(smtpUser))
                    return (false, "SMTP not configured - showing OTP on screen (demo mode)");

                var message = new MailMessage
                {
                    From = new MailAddress(smtpUser, "AHAR - Meals with Love"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUser, smtpPass)
                };
                client.Send(message);
                return (true, "Email sent successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Email failed: {ex.Message}");
            }
        }

        public static string BuildOtpEmailBody(string otp, string purpose = "Registration") =>
            $@"<div style='font-family:Arial;max-width:600px;margin:0 auto;'>
                <div style='background:linear-gradient(135deg,#f25f34,#e44d23);padding:30px;text-align:center;'>
                    <h1 style='color:#fff;margin:0;'>🍲 AHAR</h1>
                    <p style='color:#fff;margin:5px 0 0;'>Meals with Love</p>
                </div>
                <div style='padding:30px;background:#fff;'>
                    <h2 style='color:#333;'>Your OTP for {purpose}</h2>
                    <div style='background:#f8f9fa;border-left:4px solid #f25f34;padding:20px;margin:20px 0;text-align:center;'>
                        <span style='font-size:2.5rem;font-weight:bold;color:#f25f34;letter-spacing:8px;'>{otp}</span>
                    </div>
                    <p style='color:#666;'>This OTP is valid for 10 minutes. Do not share with anyone.</p>
                    <p style='color:#999;font-size:0.9rem;'>If you didn't request this, please ignore this email.</p>
                </div>
                <div style='background:#1a1a2e;padding:20px;text-align:center;'>
                    <p style='color:#aaa;margin:0;font-size:0.85rem;'>AHAR - Meals with Love | Ranchi, Jharkhand | 80923 08696</p>
                </div>
            </div>";

        public static string BuildPickupEmailBody(string userName, string venue, string date, string qty) =>
            $@"<div style='font-family:Arial;max-width:600px;margin:0 auto;'>
                <div style='background:linear-gradient(135deg,#f25f34,#e44d23);padding:30px;text-align:center;'>
                    <h1 style='color:#fff;margin:0;'>🍲 AHAR - Pickup Confirmed</h1>
                </div>
                <div style='padding:30px;background:#fff;'>
                    <h2 style='color:#333;'>Food Pickup Request Received</h2>
                    <table style='width:100%;border-collapse:collapse;margin:20px 0;'>
                        <tr style='background:#f8f9fa;'><th style='padding:10px;text-align:left;border:1px solid #ddd;'>User</th><td style='padding:10px;border:1px solid #ddd;'>{userName}</td></tr>
                        <tr><th style='padding:10px;text-align:left;border:1px solid #ddd;'>Venue</th><td style='padding:10px;border:1px solid #ddd;'>{venue}</td></tr>
                        <tr style='background:#f8f9fa;'><th style='padding:10px;text-align:left;border:1px solid #ddd;'>Date</th><td style='padding:10px;border:1px solid #ddd;'>{date}</td></tr>
                        <tr><th style='padding:10px;text-align:left;border:1px solid #ddd;'>Quantity</th><td style='padding:10px;border:1px solid #ddd;'>{qty}</td></tr>
                    </table>
                    <p style='color:#28a745;font-weight:bold;'>Our team will reach you soon. Thank you for helping fight hunger!</p>
                </div>
            </div>";
    }
}
