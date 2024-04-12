using judo_backend.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Utils;

namespace judo_backend.Services
{
    public class EmailService : IEmailService
    {
        private string fromEmail;
        private string password;
        private string host;
        private int port;

        private string webUrl;
        private string email;
        private string phone;

        public EmailService(IConfiguration configuration)
        {
            this.host = configuration.GetSection("SmtpClient:host").Value;
            this.port = int.Parse(configuration.GetSection("SmtpClient:port").Value);
            this.fromEmail = configuration.GetSection("SmtpClient:from").Value;
            this.password = configuration.GetSection("SmtpClient:password").Value;

            this.webUrl = configuration.GetSection("EmailData:weburl").Value;
            this.email = configuration.GetSection("EmailData:email").Value;
            this.phone = configuration.GetSection("EmailData:phone").Value;
        }

        public void SendWelcome(string to, string username)
        {
            var body = Body("Welcome");
            body = body.Replace("@USERNAME", username);

            var bodyBuilder = this.BodyBuilder(body);

            this.SendEmail(to, "Bienvenue au club de judo EJT de Montagny", bodyBuilder);
        }

        public void SendPasswordChanged(string to, string username)
        {
            var body = Body("PasswordChanged");
            body = body.Replace("@USERNAME", username);

            var bodyBuilder = this.BodyBuilder(body);

            this.SendEmail(to, "EJT - Changement de mot de passe", bodyBuilder);
        }

        public void SendPasswordForgot(int userId, string to, string username, string token, string code)
        {
            var body = Body("PasswordForgot");
            body = body.Replace("@USERNAME", username);
            body = body.Replace("@TOKEN", token);
            body = body.Replace("@CODE", code);
            body = body.Replace("@USERID", userId.ToString());

            var bodyBuilder = this.BodyBuilder(body);

            this.SendEmail(to, "EJT - Mot de passe oublié", bodyBuilder);
        }


        public void SendGoodBye(string to, string username)
        {
            var body = Body("GoodBye");
            body = body.Replace("@USERNAME", username);

            var bodyBuilder = this.BodyBuilder(body);

            this.SendEmail(to, "Suppression de votre compte EJT", bodyBuilder);
        }

        public void SendCompetitionRegistred(string to, string username, string adherentName, string competitionName)
        {
            var body = Body("CompetitionRegistred");
            body = body.Replace("@USERNAME", username);
            body = body.Replace("@COMPETITIONNAME", competitionName);
            body = body.Replace("@ADHERENTNAME", adherentName);

            var bodyBuilder = this.BodyBuilder(body);

            this.SendEmail(to, "Inscription", bodyBuilder);
        }

        public void SendStageRegistred(string to, string username, string adherentName, string stageName, string start, string end)
        {
            var body = Body("StageRegistred");
            body = body.Replace("@USERNAME", username);
            body = body.Replace("@STAGENAME", stageName);
            body = body.Replace("@ADHERENTNAME", adherentName);
            body = body.Replace("@START", start);
            body = body.Replace("@END", end);

            var bodyBuilder = this.BodyBuilder(body);

            this.SendEmail(to, "Inscription", bodyBuilder);
        }

        private void SendEmail(string to, string subject, BodyBuilder bodyBuilder)
        {
            try
            {
                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress("EJT", this.fromEmail));
                mail.To.Add(new MailboxAddress(to, to));
                mail.Subject = subject;
                mail.Body = bodyBuilder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Connect(this.host, this.port);
                    smtpClient.Authenticate(this.fromEmail, this.password);
                    smtpClient.Send(mail);
                    smtpClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private string Body(string htmlBody)
        {
            var body = "";

            var htmlFilePath = $"Services\\HtmlBody\\{htmlBody}.html";

            using (StreamReader reader = File.OpenText(htmlFilePath)) // Path to your 
            {
                body = reader.ReadToEnd();                
            }

            body = body.Replace("@WEBURL", this.webUrl);
            body = body.Replace("@EMAIL", this.email);
            body = body.Replace("@PHONE", this.phone);

            return body;
        }

        private BodyBuilder BodyBuilder(string body)
        {
            var bodyBuilder = new BodyBuilder();

            var pathImageHeader = $"Services\\HtmlBody\\Images\\header.png";
            var imageHeader = bodyBuilder.LinkedResources.Add(pathImageHeader);
            imageHeader.ContentId = MimeUtils.GenerateMessageId();
            body = body.Replace("@IMAGE_HEADER", imageHeader.ContentId);

            var pathImageFooter = $"Services\\HtmlBody\\Images\\footer.png";
            var imageFooter = bodyBuilder.LinkedResources.Add(pathImageFooter);
            imageFooter.ContentId = MimeUtils.GenerateMessageId();
            body = body.Replace("@IMAGE_FOOTER", imageFooter.ContentId);

            var pathImageLogo = $"Services\\HtmlBody\\Images\\logo.png";
            var imageLogo = bodyBuilder.LinkedResources.Add(pathImageLogo);
            imageLogo.ContentId = MimeUtils.GenerateMessageId();
            body = body.Replace("@IMAGE_LOGO", imageLogo.ContentId);

            var pathImageFacebook = $"Services\\HtmlBody\\Images\\facebook-icon.png";
            var imageFacebook = bodyBuilder.LinkedResources.Add(pathImageFacebook);
            imageFacebook.ContentId = MimeUtils.GenerateMessageId();
            body = body.Replace("@IMAGE_FACEBOOK", imageFacebook.ContentId);

            var pathImageInstagram = $"Services\\HtmlBody\\Images\\instagram-icon.png";
            var imageInstagram = bodyBuilder.LinkedResources.Add(pathImageInstagram);
            imageInstagram.ContentId = MimeUtils.GenerateMessageId();
            body = body.Replace("@IMAGE_INSTAGRAM", imageInstagram.ContentId);

            var pathImageYoutube = $"Services\\HtmlBody\\Images\\youtube-icon.png";
            var imageYoutube = bodyBuilder.LinkedResources.Add(pathImageYoutube);
            imageYoutube.ContentId = MimeUtils.GenerateMessageId();
            body = body.Replace("@IMAGE_YOUTUBE", imageYoutube.ContentId);

            bodyBuilder.HtmlBody = body;

            return bodyBuilder;
        }
    }
}
