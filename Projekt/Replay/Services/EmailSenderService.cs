// @author Daniel Feustel

using MailKit.Net.Smtp;
// Bibliothek for emails
using MimeKit;
using Replay.Data;
using Replay.Models;
using Replay.Repositories;
using Replay.ViewModels;

namespace Replay.Services
{
/// <author>Daniel Feustel</author>
    public class EmailSenderService  {

        // #NOTE ich habe in dem catch block nocch ein disconnect rein gemacht, und den smtpClient hochgeholt (dependency Injection und Konstruktor )

        public ISmtpClient smtpClient {get; set;}

        public EmailSenderService()
        {
            smtpClient = new SmtpClient();
        }

        /// <summary>
        /// This method creates and sends an email, which requests a user to create an initial password for his account.
        /// 
        /// </summary>
        /// <param name="conf">Of the type IConfiguration. IConfiguration is a interface, which makes configuration informations available, usually from a apsetting.json file.
        /// In this case the EmailSettings from the appsettings.LocalDocker.json are made available.
        /// </param>
        /// <param name="model">Model is of the type SecurityModel, it is no extern interface or class but a simple project intern model 
        /// that saves security related informations like id and email. Is used to recall the email adress of the receiver.</param>
        /// <param name="name">Name of the receiver. To personalize the email (Hello Max Mustermann, ....)</param>
        /// <returns></returns>
        public bool SendInitialPasswordEmail(IConfiguration conf, SecurityModel model, string name) 
        {
            
            // MimeMessage is from the Email bibliothek MimeKit
            // MimeMesagge is used to create Email-Messages which can have texts, HTML, and attachments
            var msg = new MimeMessage();

            // adds the Mailboxadresses to the E-mail message. A new Mailboxadress is created with the Name and the email-adress of the receiver 
            
            // adds the sender Email-adress to the Email message
            msg.From.Add(new MailboxAddress(conf["EmailSettings:SENDER_NAME"], conf["EmailSettings:SENDER_EMAIL_ADDRESS"]));
            
            // adds the receiver email adress to the email message 
            msg.To.Add(new MailboxAddress(name, model.Email));

            // sets the subject to Initialpasswort setzen (Betreff)
            msg.Subject = "Initialpasswort setzen";

            string urlHost= conf["REDIRECT_URL"] ?? conf["Kestrel:Endpoints:Http:Url"];
            // string port = conf["APP_PORT"];
            string url = urlHost+"/Account/InitialPassword?id="+model.Id;
            
            // setting the contetn of the email as a html text 
            msg.Body = new TextPart("html") {
                Text =
                "<h1>Hallo "+name+"</h1>"+
                "<h3>Du wurdest bei Replay registriert</h3>"+
                "Klicke <a href="+url+">hier</a> um das Passwort für deinen Account anzulegen"
            };


            // SmptClient is used to send Emails via SMTP. It has attributes and methods useful for connecting to a
            // SMTP server, sending Emails and disconnecting
            // using var client = new SmtpClient();
            try
            {
                // Connecting to the SMTP server using the server address, port, and authentication flag from the configuration.
                smtpClient.Connect(conf["EmailSettings:SMTP_SERVER"], Int32.Parse(conf["EmailSettings:SMTP_PORT"]), bool.Parse(conf["EmailSettings:AUTH"]));
                // Sending the email message created earlier (msg).
                smtpClient.Send(msg);
                // Disconnects from the SMTP server. The 'true' parameter ensures that all messages are sent before disconnecting.
                smtpClient.Disconnect(true);
            }
            catch (Exception e)
            {
                // If an exception occurs (e.g., connection failure, sending error), it is caught and printed on the console.
                // The method returns false to indicate the failure.
                // also dont forget to disconnect in case there is an exception
                Console.WriteLine(e);
                smtpClient.Disconnect(true);
                return false;
            }
            return true;
        }



        /// <summary>
        /// This method creates and sends an email, which requests a user to reset his password.
        /// </summary>
        /// <param name="conf">Of the type IConfiguration. IConfiguration is a interface, which makes configuration informations available, usually from a apsetting.json file.
        /// In this case the EmailSettings from the appsettings.LocalDocker.json are made available.</param>
        /// <param name="model">Model is of the type SecurityModel, it is no extern interface or class but a simple project intern model 
        /// that saves security related informations like id and email. Is used to recall the email adress of the receiver.</param>
        /// <param name="name">Name of the receiver. To personalize the email (Hello Max Mustermann, ....)</param>
        /// <returns></returns>
         public  bool SendResetPasswordEmail(IConfiguration conf, SecurityModel model, string name) 
         {
            // creating a new email message
            var msg = new MimeMessage();

            // Add the sender email address to the email message
            msg.From.Add(new MailboxAddress(conf["EmailSettings:SENDER_NAME"], conf["EmailSettings:SENDER_EMAIL_ADDRESS"]));
            
            // Add the recipent email address to the email message
            msg.To.Add(new MailboxAddress(name, model.Email));
            
            // Set the subject of the email
            msg.Subject = "Initialpasswort setzen";

            // Construct the reset password Url using configuration settings and the users Id 
            string urlHost= conf["REDIRECT_URL"] ?? conf["Kestrel:Endpoints:Http:Url"];
            string url = urlHost+"/Account/ResetPassword?id="+model.Id;
            
            // set the content of the email as HTML
            msg.Body = new TextPart("html") {
                Text =
                "<h1>Hallo "+name+"</h1>"+
                "<h3>Du, oder ein Admin hat eine Rücksetzung deines Passworts angefordert</h3>"+
                "Klicke <a href="+url+">hier</a> um dein Passwort zurück zu setzen"
            };

            // using var SmtpClient = new SmtpClient();

            try
            {
                // Connect to the SMTP server using the server address, port, and authentication flag from the configuration
                smtpClient.Connect(conf["EmailSettings:SMTP_SERVER"], Int32.Parse(conf["EmailSettings:SMTP_PORT"]), bool.Parse(conf["EmailSettings:AUTH"]));
                
                // Send the email message created earlier
                smtpClient.Send(msg);

                // Disconnect from the SMTP server. The 'true' parameter ensures that all messages are sent before disconnecting
                smtpClient.Disconnect(true);
            }
            catch (Exception e)
            {
                // If an exception occurs (e.g., connection failure, sending error), it is caught and printed on the console
                // The method returns false to indicate the failure
                // Also ensure to disconnect in case there is an exception
                Console.WriteLine(e);
                smtpClient.Disconnect(true);
                return false;
            }
            return true;
        }

        


    }

}