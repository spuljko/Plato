﻿namespace Plato.Badges.Models
{
    public interface IBadge
    {

        string Name { get; set; }

        string Description { get; set; }

        int Threshold { get; set; }

        int BonusPoints { get; set; }

        bool Enabled { get; set; }

        BadgeLevel Level { get; set; }

    }

    public enum BadgeLevel
    {
        Gold,
        Silver,
        Bronze
    }


}
