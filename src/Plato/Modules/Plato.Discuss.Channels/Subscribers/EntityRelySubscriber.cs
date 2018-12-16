﻿using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.Services;
using Plato.Discuss.Models;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Discuss.Channels.Subscribers
{

    /// <summary>
    /// Updates category meta data whenever an entity reply is created or updated.
    /// </summary>
    /// <typeparam name="TEntityReply"></typeparam>
    public class EntityReplySubscriber<TEntityReply> : IBrokerSubscriber where TEntityReply : class, IEntityReply
    {

        private readonly IBroker _broker;
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly IChannelDetailsUpdater _channelDetailsUpdater;

        public EntityReplySubscriber(
            IBroker broker,
            ICategoryStore<Channel> channelStore,
            IEntityStore<Topic> topicStore,
            IChannelDetailsUpdater channelDetailsUpdater)
        {
            _broker = broker;
            _channelStore = channelStore;
            _topicStore = topicStore;
            _channelDetailsUpdater = channelDetailsUpdater;
        }

        #region "Implementation"

        public void Subscribe()
        {
            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

            _broker.Sub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyUpdated(message.What));

        }

        public void Unsubscribe()
        {
            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyCreated"
            }, async message => await EntityReplyCreated(message.What));

            _broker.Unsub<TEntityReply>(new MessageOptions()
            {
                Key = "EntityReplyUpdated"
            }, async message => await EntityReplyUpdated(message.What));

        }
        
        public void Dispose()
        {
            Unsubscribe();
        }

        #endregion

        #region "Private Methods"

        async Task<TEntityReply> EntityReplyCreated(TEntityReply reply)
        {

            // No need to update cateogry for private entities
            if (reply.IsPrivate)
            {
                return reply;
            }

            // No need to update cateogry for soft deleted replies
            if (reply.IsDeleted)
            {
                return reply;
            }

            // No need to update cateogry for replies flagged as spam
            if (reply.IsSpam)
            {
                return reply;
            }

            // Get the entity we are replying to
            var entity = await _topicStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return reply;
            }
            
            // Ensure we have a categoryId for the entity
            if (entity.CategoryId <= 0)
            {
                return reply;
            }

            // Ensure we found the category
            var channel = await _channelStore.GetByIdAsync(entity.CategoryId);
            if (channel == null)
            {
                return reply;
            }

            // Update channel details
            await _channelDetailsUpdater.UpdateAsync(channel.Id);
            
            // return 
            return reply;

        }

        async Task<TEntityReply> EntityReplyUpdated(TEntityReply reply)
        {

            // No need to update cateogry for private entities
            if (reply.IsPrivate)
            {
                return reply;
            }

            // No need to update cateogry for soft deleted replies
            if (reply.IsDeleted)
            {
                return reply;
            }

            // No need to update cateogry for replies flagged as spam
            if (reply.IsSpam)
            {
                return reply;
            }

            // Get the entity we are replying to
            var entity = await _topicStore.GetByIdAsync(reply.EntityId);
            if (entity == null)
            {
                return reply;
            }

            // Ensure we have a categoryId for the entity
            if (entity.CategoryId <= 0)
            {
                return reply;
            }

            // Ensure we found the category
            var channel = await _channelStore.GetByIdAsync(entity.CategoryId);
            if (channel == null)
            {
                return reply;
            }

            // Update channel details
            await _channelDetailsUpdater.UpdateAsync(channel.Id);

            // return 
            return reply;

        }
        
        #endregion

    }

}
