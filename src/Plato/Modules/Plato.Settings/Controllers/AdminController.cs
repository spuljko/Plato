﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Settings.ViewModels;

namespace Plato.Settings.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IAuthorizationService _authorizationService;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly ILocaleProvider _localeProvider;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }


        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IAlerter alerter,
            ISiteSettingsStore siteSettingsStore,
            IBreadCrumbManager breadCrumbManager,
            ITimeZoneProvider timeZoneProvider,
            ILocaleProvider localeProvider)
        {
            _alerter = alerter;
            _siteSettingsStore = siteSettingsStore;
            _breadCrumbManager = breadCrumbManager;
            _timeZoneProvider = timeZoneProvider;
            _localeProvider = localeProvider;
            _authorizationService = authorizationService;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        #endregion

        #region "Actions"

        public async Task<IActionResult> Index()
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"]);
            });
            
            return View(await GetModel());

        }

        public async Task<IActionResult> CreateApiKey()
        {

            var settings = await _siteSettingsStore.GetAsync();
            if (settings == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            var result = await _siteSettingsStore.SaveAsync(settings);
            if (result != null)
            {
                _alerter.Success(T["Key Updated Successfully!"]);
            }
            else
            {
                _alerter.Danger(T["A problem occurred updating the settings. Please try again!"]);
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        [ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(SiteSettingsViewModel viewModel)
        {
            
            // TODO: Implement security
            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
            
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _alerter.Danger(T[error.ErrorMessage]);
                    }
                }
                return View(await GetModel());
            }

            // Update existing settings
            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                settings.SiteName = viewModel.SiteName;
                settings.TimeZone = viewModel.TimeZone;
                settings.DateTimeFormat = viewModel.DateTimeFormat;
                settings.Culture = viewModel.Culture;
            }
            else
            {
                // Create new settings
                settings = new SiteSettings()
                {
                    SiteName = viewModel.SiteName,
                    TimeZone = viewModel.TimeZone,
                    DateTimeFormat = viewModel.DateTimeFormat,
                    Culture = viewModel.Culture
                };
            }
        
            // Update settings
            var result = await _siteSettingsStore.SaveAsync(settings);
            if (result != null)
            {
                _alerter.Success(T["Settings Updated Successfully!"]);
            }
            else
            {
                _alerter.Danger(T["A problem occurred updating the settings. Please try again!"]);
            }
            
            return RedirectToAction(nameof(Index));
            
        }
        
        #endregion

        #region "Private Methods"

        private async Task<SiteSettingsViewModel> GetModel()
        {

            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                return new SiteSettingsViewModel()
                {
                    SiteName = settings.SiteName,
                    TimeZone = settings.TimeZone,
                    DateTimeFormat = settings.DateTimeFormat,
                    Culture = settings.Culture,
                    AvailableTimeZones = await GetAvailableTimeZonesAsync(),
                    AvailableDateTimeFormat = GetAvaialbleDateTimeFormats(),
                    AvailableCultures = await GetAvailableCulturesAsync()
                };
            }
            
            // return default settings
            return new SiteSettingsViewModel()
            {
                SiteName = "Example Site"
            };

        }

        #endregion

        #region "Private Methods"
        
        IEnumerable<SelectListItem> GetAvaialbleDateTimeFormats()
        {
            
            var formats = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };

            foreach (var value in DateTimeFormats.Defaults)
            {
                formats.Add(new SelectListItem
                {
                    Text = System.DateTime.UtcNow.ToString(value),
                    Value = value
                });
            }

            return formats;

        }

        async Task<IEnumerable<SelectListItem>> GetAvailableTimeZonesAsync()
        {
            // Build timezones 
            var timeZones = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };
            foreach (var z in await _timeZoneProvider.GetTimeZonesAsync())
            {
                timeZones.Add(new SelectListItem
                {
                    Text = z.DisplayName,
                    Value = z.Id
                });
            }

            return timeZones;
        }

        async Task<IEnumerable<SelectListItem>> GetAvailableCulturesAsync()
        {
            // Build timezones 
            var locales = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };

            // From all locale descriptors get a unique list of supported culture cdes
            var uniqueLocales = new List<string>();
            var localeDescriptors = await _localeProvider.GetLocalesAsync();
            foreach (var localDescriptor in localeDescriptors)
            {
                if (!uniqueLocales.Contains(localDescriptor.Descriptor.Name))
                {
                    uniqueLocales.Add(localDescriptor.Descriptor.Name);
                }
            }

            foreach (var locale in uniqueLocales)
            {
                locales.Add(new SelectListItem
                {
                    Text = locale,
                    Value = locale
                });
            }

            return locales;
        }

        #endregion

    }
}
