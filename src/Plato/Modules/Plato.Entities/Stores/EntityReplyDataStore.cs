﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityReplyDataStore<T> : IStore<T> where T : class
    {

        Task<IEnumerable<T>> GetByReplyIdAsync(int replyId);

    }

    public class EntityReplyDataStore : IEntityReplyDataStore<IEntityReplyData>
    {

        private readonly ICacheManager _cacheManager;
        private readonly IEntityReplyDataRepository<IEntityReplyData> _entityDataRepository;
        private readonly ILogger<EntityDataStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ITypedModuleProvider _typedModuleProvider;
        
        public EntityReplyDataStore(
            ICacheManager cacheManager,
            IEntityReplyDataRepository<IEntityReplyData> entityDataRepository, 
            ILogger<EntityDataStore> logger,
            IDbQueryConfiguration dbQuery,
            ITypedModuleProvider typedModuleProvider)
        {
            _cacheManager = cacheManager;
            _entityDataRepository = entityDataRepository;
            _logger = logger;
            _dbQuery = dbQuery;
            _typedModuleProvider = typedModuleProvider;
        }
        
        public async Task<IEntityReplyData> CreateAsync(IEntityReplyData model)
        {
            var result =  await _entityDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<IEntityReplyData> UpdateAsync(IEntityReplyData model)
        {
            var result = await _entityDataRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(IEntityReplyData model)
        {
            var success = await _entityDataRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity reply data with key '{0}' for entity id {1}",
                        model.Key, model.ReplyId);
                }

                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<IEntityReplyData> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityDataRepository.SelectByIdAsync(id));
        }

        public IQuery<IEntityReplyData> QueryAsync()
        {
            var query = new EntityReplyDataQuery(this);
            return _dbQuery.ConfigureQuery<IEntityReplyData>(query); ;
        }

        public async Task<IPagedResults<IEntityReplyData>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityDataRepository.SelectAsync(dbParams));
        }

        public async Task<IEnumerable<IEntityReplyData>> GetByReplyIdAsync(int replyId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), replyId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityDataRepository.SelectByReplyIdAsync(replyId));
        }

    }

}
