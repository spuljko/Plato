﻿using System;
using System.Threading.Tasks;
using Plato.Entities.History.Models;
using Plato.Entities.History.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Text.Abstractions;

namespace Plato.Entities.History.Services
{

    public class EntityHistoryManager : IEntityHistoryManager<EntityHistory> 
    {
        
        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;
        private readonly IContextFacade _contextFacade;
        private readonly IBroker _broker;

        public EntityHistoryManager(
            IEntityHistoryStore<EntityHistory> entityHistoryStore,
            IContextFacade contextFacade,
            IBroker broker)
        {
            _entityHistoryStore = entityHistoryStore;
            _contextFacade = contextFacade;
            _broker = broker;
        }

        #region "Implementation"

        public async Task<ICommandResult<EntityHistory>> CreateAsync(EntityHistory model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // We should never have an Id for inserts
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            // We always need an eneityId
            if (model.EntityId <= 0)
            {
                throw new ArgumentNullException(nameof(model.EntityId));
            }
            
            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }
            
            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }
            
            // Invoke EntityHistoryCreating subscriptions
            foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryCreating"))
            {
                model = await handler.Invoke(new Message<EntityHistory>(model, this));
            }

            var result = new CommandResult<EntityHistory>();

            var newEntityHistory = await _entityHistoryStore.CreateAsync(model);
            if (newEntityHistory != null)
            {

                // Invoke EntityHistoryCreated subscriptions
                foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryCreated"))
                {
                    newEntityHistory = await handler.Invoke(new Message<EntityHistory>(newEntityHistory, this));
                }

                // Return success
                return result.Success(newEntityHistory);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the entity history entry."));
            
        }

        public async Task<ICommandResult<EntityHistory>> UpdateAsync(EntityHistory model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // We always need an Id for updates
            if (model.Id <= 0)
            {
                throw new ArgumentNullException(nameof(model.Id));
            }
            
            // We always need an entityId
            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }
            
            // Invoke EntityHistoryUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryUpdating"))
            {
                model = await handler.Invoke(new Message<EntityHistory>(model, this));
            }

            var result = new CommandResult<EntityHistory>();

            var label = await _entityHistoryStore.UpdateAsync(model);
            if (label != null)
            {

                // Invoke EntityHistoryUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryUpdated"))
                {
                    label = await handler.Invoke(new Message<EntityHistory>(label, this));
                }

                // Return success
                return result.Success(label);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update the entity history entry"));
            
        }

        public async Task<ICommandResult<EntityHistory>> DeleteAsync(EntityHistory model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            // Invoke EntityHistoryDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryDeleting"))
            {
                model = await handler.Invoke(new Message<EntityHistory>(model, this));
            }
            
            var result = new CommandResult<EntityHistory>();
            if (await _entityHistoryStore.DeleteAsync(model))
            {

                // Invoke EntityHistoryDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityHistory>(this, "EntityHistoryDeleted"))
                {
                    model = await handler.Invoke(new Message<EntityHistory>(model, this));
                }

                // Return success
                return result.Success();

            }
            
            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the entity history entry"));
            
        }

        #endregion
        
    }

}
