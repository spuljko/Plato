﻿using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.Services;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Channels.Subscribers
{

    /// <summary>
    /// Updates category meta data whenever an entity is created or updated.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    //public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    //{

    //    private readonly IBroker _broker;
    //    private readonly ICategoryStore<Channel> _channelStore;
    //    private readonly IChannelDetailsUpdater _channelDetailsUpdater;

    //    public EntitySubscriber(
    //        IBroker broker,
    //        ICategoryStore<Channel> channelStore,
    //        IChannelDetailsUpdater channelDetailsUpdater)
    //    {
    //        _broker = broker;
    //        _channelStore = channelStore;
    //        _channelDetailsUpdater = channelDetailsUpdater;
    //    }

    //    #region "Implementation"

    //    public void Subscribe()
    //    {
    //        // Created
    //        _broker.Sub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityCreated"
    //        }, async message => await EntityCreated(message.What));

    //        // Updated
    //        _broker.Sub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityUpdated"
    //        }, async message => await EntityUpdated(message.What));

    //        // Deleted
    //        _broker.Sub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityDeleted"
    //        }, async message => await EntityDeleted(message.What));
    //    }

    //    public void Unsubscribe()
    //    {

    //        // Created
    //        _broker.Unsub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityCreated"
    //        }, async message => await EntityCreated(message.What));

    //        // Updated
    //        _broker.Unsub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityUpdated"
    //        }, async message => await EntityUpdated(message.What));

    //        // Deleted
    //        _broker.Unsub<TEntity>(new MessageOptions()
    //        {
    //            Key = "EntityDeleted"
    //        }, async message => await EntityDeleted(message.What));

    //    }
        
    //    #endregion

    //    #region "Private Methods"

    //    async Task<TEntity> EntityCreated(TEntity entity)
    //    {

    //        // No need to update category for private entities
    //        if (entity.IsHidden())
    //        {
    //            return entity;
    //        }

    //        // Ensure we have a categoryId for the entity
    //        if (entity.CategoryId <= 0)
    //        {
    //            return entity;
    //        }

    //        // Ensure we found the category
    //        var channel = await _channelStore.GetByIdAsync(entity.CategoryId);
    //        if (channel == null)
    //        {
    //            return entity;
    //        }

    //        // Update channel details
    //        await _channelDetailsUpdater.UpdateAsync(channel.Id);

    //        // Return
    //        return entity;

    //    }

    //    async Task<TEntity> EntityUpdated(TEntity entity)
    //    {
            
    //        // Ensure we have a categoryId for the entity
    //        if (entity.CategoryId <= 0)
    //        {
    //            return entity;
    //        }

    //        // Ensure we found the category
    //        var channel = await _channelStore.GetByIdAsync(entity.CategoryId);
    //        if (channel == null)
    //        {
    //            return entity;
    //        }

    //        // Update channel details
    //        await _channelDetailsUpdater.UpdateAsync(channel.Id);

    //        // Return
    //        return entity;

    //    }

    //    async Task<TEntity> EntityDeleted(TEntity entity)
    //    {

    //        // Ensure we have a categoryId for the entity
    //        if (entity.CategoryId <= 0)
    //        {
    //            return entity;
    //        }

    //        // Ensure we found the category
    //        var channel = await _channelStore.GetByIdAsync(entity.CategoryId);
    //        if (channel == null)
    //        {
    //            return entity;
    //        }

    //        // Update channel details
    //        await _channelDetailsUpdater.UpdateAsync(channel.Id);

    //        // Return
    //        return entity;

    //    }
        
    //    #endregion

    //}

}
