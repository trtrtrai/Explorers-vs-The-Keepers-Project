using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class Stats
    {
        public StatsType StatsName;

        public int Immutable;
        public int Mutable;

        public Stats(){}
        
        public Stats(Stats stats)
        {
            StatsName = stats.StatsName;
            Immutable = stats.Immutable;
            Mutable = stats.Mutable;
        }

        /// <summary>
        /// Add integer into Stats. Health cannot higher its maximum.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int operator +(Stats a, int b)
        {
            var c = a.Mutable + b;
            
            if (a.StatsName == StatsType.Health)
            {
                c = Mathf.Clamp(c, 0, a.Immutable);
            }
            
            return c;
        }
        
        /// <summary>
        /// Subtract integer into Stats. Health cannot higher its maximum.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int operator -(Stats a, int b)
        {
            var c = a.Mutable - b;
            
            if (a.StatsName == StatsType.Health)
            {
                c = Mathf.Clamp(c, 0, a.Immutable);
            }

            return c;
        }

        /// <summary>
        /// Get ratio value from Stats.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float operator *(Stats a, float b)
        {
            return a.Mutable * b;
        }

        /// <summary>
        /// Get ratio value from Stats.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float operator /(Stats a, float b)
        {
            return a.Mutable / b;
        }
    }

    public enum StatsType
    {
        Health,
        Attack,
        Defense,
        Critical,
        Speed,
        Step,
        Agility,
        Aim,
        Shield
    }
}