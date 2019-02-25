﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Articles.Models;
using Plato.Articles.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Entities.ViewModels;

namespace Plato.Articles.Services
{
    
    //public class ArticleService : IArticleService
    //{

    //    private readonly IContextFacade _contextFacade;
    //    private readonly IEntityStore<Article> _topicStore;
    //    private readonly IFeatureFacade _featureFacade;
    //    private readonly IPlatoRoleStore _roleStore;
    //    private readonly IAuthorizationService _authorizationService;
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public ArticleService(
    //        IContextFacade contextFacade,
    //        IEntityStore<Article> topicStore,
    //        IFeatureFacade featureFacade,
    //        IPlatoRoleStore roleStore,
    //        IAuthorizationService authorizationService,
    //        IHttpContextAccessor httpContextAccessor)
    //    {
    //        _contextFacade = contextFacade;
    //        _topicStore = topicStore;
    //        _featureFacade = featureFacade;
    //        _roleStore = roleStore;
    //        _authorizationService = authorizationService;
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    public async Task<IPagedResults<Article>> GetResultsAsync(EntityIndexOptions options, PagerOptions pager)
    //    {
            
    //        if (options == null)
    //        {
    //            options = new EntityIndexOptions();
    //        }

    //        if (pager == null)
    //        {
    //            pager = new PagerOptions();
    //        }

    //        // Get articles features
    //        var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");
            
    //        // Get authenticated user for use within view adapter below
    //        var user = await _contextFacade.GetAuthenticatedUserAsync();

    //        // Get principal
    //        var principal = _httpContextAccessor.HttpContext.User;

    //        // Get anonymous role 
    //        //var anonymousRole = await _roleStore.GetByNameAsync(DefaultRoles.Anonymous);

    //        // Return tailored results
    //        return await _topicStore.QueryAsync()
    //            .Take(pager.Page, pager.PageSize)
    //            .Select<EntityQueryParams>(async q => 
    //            {

    //                if (feature != null)
    //                {
    //                    q.FeatureId.Equals(feature.Id);
    //                }

    //                switch (options.Filter)
    //                {
    //                    case FilterBy.MyTopics:
    //                        if (user != null)
    //                        {
    //                            q.CreatedUserId.Equals(user.Id);
    //                        }

    //                        break;
    //                    case FilterBy.Participated:
    //                        if (user != null)
    //                        {
    //                            q.ParticipatedUserId.Equals(user.Id);
    //                        }

    //                        break;
    //                    case FilterBy.Following:
    //                        if (user != null)
    //                        {
    //                            q.FollowUserId.Equals(user.Id, b =>
    //                            {
    //                                // Restrict follows by topic
    //                                b.Append(" AND f.[Name] = 'Topic'");
    //                            });
    //                        }

    //                        break;
    //                    case FilterBy.Starred:
    //                        if (user != null)
    //                        {
    //                            q.StarUserId.Equals(user.Id, b =>
    //                            {
    //                                // Restrict follows by topic
    //                                b.Append(" AND s.[Name] = 'Topic'");
    //                            });
    //                        }

    //                        break;

    //                    case FilterBy.NoReplies:

    //                        q.TotalReplies.Equals(0);
    //                        break;
    //                }

    //                // Restrict results via user role if the channels feature is enabled
    //                //if (channelFeature != null)
    //                //{
    //                //    if (user != null)
    //                //    {
    //                //        q.RoleId.IsIn(user.UserRoles?.Select(r => r.Id).ToArray());
    //                //    }
    //                //    else
    //                //    {
    //                //        if (anonymousRole != null)
    //                //        {
    //                //            q.RoleId.Equals(anonymousRole.Id);
    //                //        }
    //                //    }
    //                //}

              
    //                    if (options.ChannelId > 0)
    //                    {
    //                        q.CategoryId.Equals(options.ChannelId);
    //                    }

    //                    if (options.ChannelIds != null)
    //                    {
    //                        q.CategoryId.IsIn(options.ChannelIds);
    //                    }
                        
    //                    if (options.LabelId > 0)
    //                    {
    //                        q.LabelId.Equals(options.LabelId);
    //                    }

    //                    if (options.TagId > 0)
    //                    {
    //                        q.TagId.Equals(options.TagId);
    //                    }

    //                    if (options.CreatedByUserId > 0)
    //                    {
    //                        q.CreatedUserId.Equals(options.CreatedByUserId);
    //                    }

                

    //                // Hide private?
    //                if (!await _authorizationService.AuthorizeAsync(principal,
    //                    Permissions.ViewPrivateTopics))
    //                {
    //                    q.HidePrivate.True();
    //                }

    //                // Hide spam?
    //                if (!await _authorizationService.AuthorizeAsync(principal,
    //                    Permissions.ViewSpamTopics))
    //                {
    //                    q.HideSpam.True();
    //                }

    //                // Hide deleted?
    //                if (!await _authorizationService.AuthorizeAsync(principal,
    //                    Permissions.ViewDeletedTopics))
    //                {
    //                    q.HideDeleted.True();
    //                }
                    
    //                //q.IsPinned.True();
    //                //if (!string.IsNullOrEmpty(filterOptions.Search))
    //                //{
    //                //    q.UserName.IsIn(filterOptions.Search).Or();
    //                //    q.Email.IsIn(filterOptions.Search);
    //                //}
    //                // q.UserName.IsIn("Admin,Mark").Or();
    //                // q.Email.IsIn("email440@address.com,email420@address.com");
    //                // q.Id.Between(1, 5);

    //            })
    //            .OrderBy("IsPinned",  OrderBy.Desc)
    //            .OrderBy(options.Sort.ToString(), options.Order)
    //            .ToList();

    //    }

    //}

}
