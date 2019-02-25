﻿using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.Services
{
    public interface IEntityService<TModel> where TModel : class, IEntity
    {

        Task<IPagedResults<TModel>> GetResultsAsync(EntityIndexOptions options, PagerOptions pager);

    }
    
}
