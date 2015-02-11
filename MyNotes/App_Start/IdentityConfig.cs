using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Mandrill;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MyNotes.Models;

namespace MyNotes
{
    public class EmailService : IIdentityMessageService
    {
        private readonly MandrillApi _mandrill;
        private const string EmailFromAddress = "no-reply@mynotes.com";
        private const string EmailFromName = "My Notes";

        public EmailService()
        {
            _mandrill = new MandrillApi(ConfigurationManager.AppSettings["MandrillApiKey"]);
        }

        public Task SendAsync(IdentityMessage message)
        {
            var task = _mandrill.SendMessageAsync(new EmailMessage
            {
                from_email = EmailFromAddress,
                from_name = EmailFromName,
                subject = message.Subject,
                to = new List<Mandrill.EmailAddress> { new EmailAddress(message.Destination) },
                html = message.Body
            });

            return task;
        }

        public Task SendWelcomeEmail(string firstName, string email)
        {
            const string subject = "Welcome to My Notes";

            var emailMessage = new EmailMessage
            {
                from_email = EmailFromAddress,
                from_name = EmailFromName,
                subject = subject,
                to = new List<Mandrill.EmailAddress> { new EmailAddress(email) },
                merge = true,
            };

            emailMessage.AddGlobalVariable("subject", subject);
            emailMessage.AddGlobalVariable("FIRST_NAME", firstName);

            var task = _mandrill.SendMessageAsync(emailMessage, "welcome-my-notes-saas", null);

            task.Wait();

            return task;
        }

        public Task SendResetPasswordEmail(string firstName, string email, string resetLink)
        {
            const string subject = "Reset My Notes Password Request";

            var emailMessage = new EmailMessage
            {
                from_email = EmailFromAddress,
                from_name = EmailFromName,
                subject = subject,
                to = new List<Mandrill.EmailAddress> { new EmailAddress(email) }
            };
            emailMessage.AddGlobalVariable("subject", subject);
            emailMessage.AddGlobalVariable("FIRST_NAME", firstName);
            emailMessage.AddGlobalVariable("RESET_PASSWORD_LINK", resetLink);

            var task = _mandrill.SendMessageAsync(emailMessage, "reset-password-notes-saas", null);

            return task;
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public new EmailService EmailService
        {
            get { return base.EmailService as EmailService; }
            set { base.EmailService = value; }
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
