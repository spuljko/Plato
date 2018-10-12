﻿using System;
using System.Collections.Generic;
using System.Text;
using Plato.Badges.Models;

namespace Plato.Badges.Services
{
    public interface IBadgeProvider<TBadge> where TBadge : class, IBadge
    {
        IEnumerable<TBadge> GetBadges();

        IEnumerable<DefaultBadges<TBadge>> GetDefaultBadges();
        
    }

}
