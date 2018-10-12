﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Badges.Models
{
    public class DefaultBadges<TBadge> where TBadge : class, IBadge
    {

        public string Feature { get; set; }

        public IEnumerable<TBadge> Badges { get; set; }

    }


}
