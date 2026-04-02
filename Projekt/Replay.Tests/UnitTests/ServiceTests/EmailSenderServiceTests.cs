using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Configuration;
using Xunit;
using MailKit.Net.Smtp;
using MimeKit;
using Replay.Models;
using Replay.Services;

namespace Replay.Tests.UnitTests.ServiceTests
{
    /// <summary>
    /// This calss tests the fucntions from the EmailSenderService
    /// </summary>
    /// <author> Robert Figl </author>
    public class EmailSenderServiceTests
    {
        // IConfigurations is a interface which allows managing configuring informations
        // In this case IConfiguratons is used to simulate specific configuration values 
        // -> enables testing without accessing extern data
        private readonly Mock<IConfiguration> _mockConfiguration;

        // instance of the class which is tested 
        private readonly EmailSenderService _emailSenderService;

        // Mock-Object for SmtpClient, to simulate SMTP-interactions
        // interfaces are commonly used in UniTests because it enables testing without concrete implementations of the methods 
        private readonly Mock<ISmtpClient> _mockSmtpClient;

        /// <summary>
        ///  Constructor of the Test class, instantiate the Mock Obects and the class which will be tested
        /// </summary>
        /// <author> Robert Figl </author>
        public EmailSenderServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockSmtpClient = new Mock<ISmtpClient>();
            _emailSenderService = new EmailSenderService
            {
                // In the Test class Email Sender Service works with the Mock Object, so no real SMTP Server 
                // Because of the interface it behaves like a SMTPClient class, but no real connecting to servers or sending emails
                smtpClient = _mockSmtpClient.Object
            };
        }

        /// <summary>
        /// Test-method for checking when the send initial password email is sended succesfully
        /// </summary>
        /// <author> Robert Figl </author>
        [Fact]
        public void SendInitialPasswordEmail_ShouldReturnTrue_WhenEmailSendSuccesfully()
        {
            // Arrange

            // Creating a security Model with an unique id in the Guid format, and an email 
            var securityModel = new SecurityModel { Id = Guid.NewGuid(), Email = "test@example.com" };
            // Creating and assigning the name of the receiver
            var name = "Test User";

            // Setting up Mock Configurations
            _mockConfiguration.SetupGet(c => c["EmailSettings:SENDER_NAME"]).Returns("Replay by Makandra");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SENDER_EMAIL_ADDRESS"]).Returns("makandra@replay.com");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SMTP_SERVER"]).Returns("smtp.test.com");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SMTP_PORT"]).Returns("1025");
            _mockConfiguration.SetupGet(c => c["EmailSettings:AUTH"]).Returns("false");
            _mockConfiguration.SetupGet(c => c["REDIRECT_URL"]).Returns("http://localhost");

            // Setting up Mock-SMPTP-Client Methods 
            _mockSmtpClient.Setup(s => s.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Verifiable();
            _mockSmtpClient.Setup(s => s.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null)).Verifiable();
            _mockSmtpClient.Setup(s => s.Disconnect(It.IsAny<bool>(), It.IsAny<CancellationToken>())).Verifiable();

            // Act
            var result = _emailSenderService.SendInitialPasswordEmail(_mockConfiguration.Object, securityModel, name);

            // Assert

            // Checking if true is returned
            Assert.True(result);

            // checking if the SMTP-Client methods are called exactly once 
            // checking if they are called exactly ones is important because no unnecessary connections are kept open, when exaxtly one is connected, sent, and disconnected
            // and sending only one email, is better than sending multiple times the same email
            _mockSmtpClient.Verify(s => s.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockSmtpClient.Verify(s => s.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null), Times.Once);
            _mockSmtpClient.Verify(s => s.Disconnect(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Test to verify unsuccessful email sending for the inital password email.
        /// </summary>
        /// <author> Robert Figl </author>
        [Fact]
        public void SendInitialPasswordEmail_ShoudReturnFalse_WhenEmailSendUnsuccessfully()
        {
            // Arrange 
            var securityModel = new SecurityModel{ Id = Guid.NewGuid(), Email = "test@example.com"};
            var name = "Test User";

            // Setting up mock configuration values.
            _mockConfiguration.SetupGet(c => c["EmailSettings:SENDER_NAME"]).Returns("Replay by Makandra");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SENDER_EMAIL_ADDRESS"]).Returns("makandra@replay.com");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SMTP_SERVER"]).Returns("smtp.test.com");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SMTP_PORT"]).Returns("1025");
            _mockConfiguration.SetupGet(c => c["EmailSettings:AUTH"]).Returns("false");
            _mockConfiguration.SetupGet(c => c["REDIRECT_URL"]).Returns("http://localhost");

            // setting up mock SMTP client methods with exception.
            _mockSmtpClient.Setup(s => s.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Verifiable();
            _mockSmtpClient.Setup(s => s.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null)).Throws(new Exception("SMTP eror")).Verifiable();
            _mockSmtpClient.Setup(s => s.Disconnect(It.IsAny<bool>(), It.IsAny<CancellationToken>())).Verifiable();

            // Act      
            var result = _emailSenderService.SendInitialPasswordEmail(_mockConfiguration.Object, securityModel, name);

            // Assert
            Assert.False(result);
            _mockSmtpClient.Verify(s => s.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockSmtpClient.Verify(s => s.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null), Times.Once);
            _mockSmtpClient.Verify(s => s.Disconnect(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        ///  Test to verify successfull email sending for the reset password email.
        /// </summary>
        /// <author> Robert Figl </author>
        [Fact]
        public void SendResetPasswordEmail_ShouldReturnTrue_WhenEmailSendSuccessfully()
        {
            // Arrange
            var securityModel = new SecurityModel { Id = Guid.NewGuid(), Email = "test@example.com" };
            var name = "Test User";

            // Setting up mock configuration values.
            _mockConfiguration.SetupGet(c => c["EmailSettings:SENDER_NAME"]).Returns("Replay by Makandra");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SENDER_EMAIL_ADDRESS"]).Returns("makandra@replay.com");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SMTP_SERVER"]).Returns("smtp.test.com");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SMTP_PORT"]).Returns("1025");
            _mockConfiguration.SetupGet(c => c["EmailSettings:AUTH"]).Returns("false");
            _mockConfiguration.SetupGet(c => c["REDIRECT_URL"]).Returns("http://localhost");

            // Setting up mock SMTP client methods.
            _mockSmtpClient.Setup(s => s.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Verifiable();
            _mockSmtpClient.Setup(s => s.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null)).Verifiable();
            _mockSmtpClient.Setup(s => s.Disconnect(It.IsAny<bool>(), It.IsAny<CancellationToken>())).Verifiable();

            // Act
            var result = _emailSenderService.SendResetPasswordEmail(_mockConfiguration.Object, securityModel, name);

            // Assert
            Assert.True(result);
            _mockSmtpClient.Verify(s => s.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockSmtpClient.Verify(s => s.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null), Times.Once);
            _mockSmtpClient.Verify(s => s.Disconnect(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Test to verify unsuccessfull email sending for the reset password email.
        /// </summary>
        /// <author> Robert Figl </author>
        [Fact]
        public void SendResetPasswordEmail_ShouldReturnFalse_WhenEmailSendUnsuccessfully()
        {
            // Arrange
            var securityModel = new SecurityModel { Id = Guid.NewGuid(), Email = "test@example.com" };
            var name = "Test User";

            // Setting up mock configuration values.
            _mockConfiguration.SetupGet(c => c["EmailSettings:SENDER_NAME"]).Returns("Replay by Makandra");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SENDER_EMAIL_ADDRESS"]).Returns("makandra@replay.com");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SMTP_SERVER"]).Returns("smtp.test.com");
            _mockConfiguration.SetupGet(c => c["EmailSettings:SMTP_PORT"]).Returns("1025");
            _mockConfiguration.SetupGet(c => c["EmailSettings:AUTH"]).Returns("false");
            _mockConfiguration.SetupGet(c => c["REDIRECT_URL"]).Returns("http://localhost");

            // Setting up mock SMTP client methods with exception
            _mockSmtpClient.Setup(s => s.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).Verifiable();
            _mockSmtpClient.Setup(s => s.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null)).Throws(new Exception("SMTP error")).Verifiable();
            _mockSmtpClient.Setup(s => s.Disconnect(It.IsAny<bool>(), It.IsAny<CancellationToken>())).Verifiable();

            // Act
            var result = _emailSenderService.SendResetPasswordEmail(_mockConfiguration.Object, securityModel, name);

            // Assert   
            Assert.False(result);
            _mockSmtpClient.Verify(s => s.Connect(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockSmtpClient.Verify(s => s.Send(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null), Times.Once);
            _mockSmtpClient.Verify(s => s.Disconnect(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}