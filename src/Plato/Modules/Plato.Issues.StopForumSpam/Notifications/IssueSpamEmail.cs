﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Plato.Issues.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Emails.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Localization.Extensions;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Issues.StopForumSpam.NotificationTypes;

namespace Plato.Issues.StopForumSpam.Notifications
{
    public class IssueSpamEmail : INotificationProvider<Issue>
    {

        private readonly IDummyClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly ICapturedRouterUrlHelper _capturedRouterUrlHelper;
        private readonly IContextFacade _contextFacade;
        private readonly ILocaleStore _localeStore;
        private readonly IEmailManager _emailManager;
        
        public IssueSpamEmail(
            IDummyClaimsPrincipalFactory<User> claimsPrincipalFactory,
            ICapturedRouterUrlHelper capturedRouterUrlHelper,
            IContextFacade contextFacade,
            IEmailManager emailManager,
            ILocaleStore localeStore)
        {
            _capturedRouterUrlHelper = capturedRouterUrlHelper;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _contextFacade = contextFacade;
            _emailManager = emailManager;
            _localeStore = localeStore;
        }

        public async Task<ICommandResult<Issue>> SendAsync(INotificationContext<Issue> context)
        {
            // Ensure correct notification provider
            if (!context.Notification.Type.Name.Equals(EmailNotifications.IssueSpam.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Create result
            var result = new CommandResult<Issue>();

            // Get email template
            const string templateId = "NewIssueSpam";

            // Tasks run in a background thread and don't have access to HttpContext
            // Create a dummy principal to represent the user so we can still obtain
            // the current culture for the email
            var principal = await _claimsPrincipalFactory.CreateAsync((User)context.Notification.To);
            var culture = await _contextFacade.GetCurrentCultureAsync(principal.Identity);
            var email = await _localeStore.GetFirstOrDefaultByKeyAsync<LocaleEmail>(culture, templateId);
            if (email == null)
            {
                return result.Failed(
                    $"No email template with the Id '{templateId}' exists within the 'locales/{culture}/emails.json' file!");
            }
            
            // Build topic url
            var baseUri = await _capturedRouterUrlHelper.GetBaseUrlAsync();
            var url = _capturedRouterUrlHelper.GetRouteUrl(baseUri, new RouteValueDictionary()
            {
                ["area"] = "Plato.Issues",
                ["controller"] = "Home",
                ["action"] = "Display",
                ["opts.id"] = context.Model.Id,
                ["opts.alias"] = context.Model.Alias
            });

            // Build message from template
            var message = email.BuildMailMessage();
            message.Body = string.Format(
                email.Message,
                context.Notification.To.DisplayName,
                context.Model.Title,
                baseUri + url);
            ;
            message.IsBodyHtml = true;
            message.To.Add(new MailAddress(context.Notification.To.Email));

            // Send message
            var emailResult = await _emailManager.SaveAsync(message);
            if (emailResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            return result.Failed(emailResult.Errors?.ToArray());
            
        }

    }

}
