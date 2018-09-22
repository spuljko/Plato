﻿using System;
using System.Threading.Tasks;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Labels.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Services;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<LabelBase>
    {
        
        private readonly IContextFacade _contextFacade;
        private readonly ILabelStore<Label> _labelStore;
        private readonly ILabelManager<Label> _labelManager;
        private readonly IFeatureFacade _featureFacade;

        public AdminViewProvider(
            IContextFacade contextFacade,
            ILabelStore<Label> labelStore,
            ILabelManager<Label> labelManager,
            IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _labelStore = labelStore;
            _labelManager = labelManager;
            _featureFacade = featureFacade;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(LabelBase label, IUpdateModel updater)
        {
            var indexViewModel = await GetIndexModel();
         
            return Views(
                View<LabelsViewModel>("Admin.Index.Header", model => indexViewModel).Zone("header").Order(1),
                View<LabelsViewModel>("Admin.Index.Tools", model => indexViewModel).Zone("tools").Order(1),
                View<LabelsViewModel>("Admin.Index.Content", model => indexViewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(LabelBase label, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(LabelBase label, IUpdateModel updater)
        {

            EditLabelViewModel editLabelViewModel = null;
            if (label.Id == 0)
            {
                editLabelViewModel = new EditLabelViewModel()
                {
                    IsNewLabel = true
                };
            }
            else
            {
                editLabelViewModel = new EditLabelViewModel()
                {
                    Id = label.Id,
                    Name = label.Name,
                    Description = label.Description,
                    ForeColor = label.ForeColor,
                    BackColor = label.BackColor
                };
            }
            
            return Task.FromResult(Views(
                View<EditLabelViewModel>("Admin.Edit.Header", model => editLabelViewModel).Zone("header").Order(1),
                View<EditLabelViewModel>("Admin.Edit.Content", model => editLabelViewModel).Zone("content").Order(1),
                View<EditLabelViewModel>("Admin.Edit.Actions", model => editLabelViewModel).Zone("actions").Order(1),
                View<EditLabelViewModel>("Admin.Edit.Footer", model => editLabelViewModel).Zone("footer").Order(1)
            ));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(LabelBase label, IUpdateModel updater)
        {

            var model = new EditLabelViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(label, updater);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();
            
            //Category category = null;

            if (updater.ModelState.IsValid)
            {
                
                var result = await _labelManager.UpdateAsync(new Label()
                {
                    Id = label.Id,
                    FeatureId = label.FeatureId,
                    Name = model.Name,
                    Description = model.Description,
                    ForeColor = model.ForeColor,
                    BackColor = model.BackColor
                });

                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(label, updater);


        }

        #endregion

        #region "Private Methods"

        async Task<LabelsViewModel> GetIndexModel()
        {
            var feature = await GetcurrentFeature();
            var categories = await _labelStore.GetByFeatureIdAsync(feature.Id);

            return new LabelsViewModel()
            {
                Labels = categories
            };
        }
        
        async Task<IShellFeature> GetcurrentFeature()
        {
            var featureId = "Plato.Discuss.Labels";
            var feature = await _featureFacade.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the module '{featureId}'");
            }
            return feature;
        }

        #endregion

    }
}
