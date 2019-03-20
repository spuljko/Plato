﻿using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;

namespace Plato.Tags.Subscribers
{
    public class EntityTagSubscriber : IBrokerSubscriber
    {
        
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly ITagManager<TagBase> _tagManager;
        private readonly ITagStore<TagBase> _tagStore;
        private readonly IBroker _broker;
        
        // Updates tag metadata whenever a entity & tag relationship is added or removed.
        public EntityTagSubscriber(
            IEntityTagStore<EntityTag> entityTagStore,
            ITagManager<TagBase> tagManager,
            ITagStore<TagBase> tagStore,
            IBroker broker)
        {
            _entityTagStore = entityTagStore;
            _tagManager = tagManager;
            _tagStore = tagStore;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {

            // Created
            _broker.Sub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagCreated"
            }, async message => await EntityTagCreated(message.What));

            // Deleted
            _broker.Sub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagDeleted"
            }, async message => await EntityTagDeleted(message.What));

        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagCreated"
            }, async message => await EntityTagCreated(message.What));

            // Deleted
            _broker.Unsub<EntityTag>(new MessageOptions()
            {
                Key = "EntityTagDeleted"
            }, async message => await EntityTagDeleted(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<EntityTag> EntityTagCreated(EntityTag entityTag)
        {

            if (entityTag == null)
            {
                throw new ArgumentNullException(nameof(entityTag));
            }

            if (entityTag.EntityId <= 0)
            {
                return entityTag;
            }

            if (entityTag.TagId <= 0)
            {
                return entityTag;
            }

            // Get tag
            var tag = await _tagStore.GetByIdAsync(entityTag.TagId);

            // No tag found no further work needed
            if (tag == null)
            {
                return entityTag;
            }

            // Get count for entities & replies tagged with this tag
            var entityTags = await _entityTagStore.QueryAsync()
                .Take(1)
                .Select<EntityTagQueryParams>(q =>
                {
                    q.TagId.Equals(tag.Id);
                })
                .ToList();
            
            // Update 
            tag.TotalEntities = entityTags?.Total ?? 0;
            tag.LastSeenDate = DateTimeOffset.Now;

            // Persist 
            await _tagManager.UpdateAsync(tag);

            return entityTag;

        }

        async Task<EntityTag> EntityTagDeleted(EntityTag entityLabel)
        {

            if (entityLabel == null)
            {
                throw new ArgumentNullException(nameof(entityLabel));
            }

            if (entityLabel.EntityId <= 0)
            {
                return entityLabel;
            }

            if (entityLabel.TagId <= 0)
            {
                return entityLabel;
            }

            // Get tag
            var tag = await _tagStore.GetByIdAsync(entityLabel.TagId);

            // No tag found no further work needed
            if (tag == null)
            {
                return entityLabel;
            }
            
            // Get count for entities & replies tagged with this tag
            var entityTags = await _entityTagStore.QueryAsync()
                .Take(1)
                .Select<EntityTagQueryParams>(q =>
                {
                    q.TagId.Equals(tag.Id);
                })
                .ToList();

            // Update tag
            tag.TotalEntities = entityTags?.Total ?? 0;
            
            // Persist updates
            await _tagManager.UpdateAsync(tag);

            return entityLabel;

        }

        #endregion

    }

}
