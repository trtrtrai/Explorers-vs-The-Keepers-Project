using System;
using System.Collections.Generic;
using System.Linq;
using EventArgs;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Models
{
    [Serializable]
    public class Status
    {
        [SerializeField] private List<Stats> health;
        [SerializeField] private Stats attack;
        [SerializeField] private Stats defense;
        [SerializeField] private Stats critical;
        [SerializeField] private Stats speed;
        [SerializeField] private Stats step;
        [SerializeField] private Stats agility;
        [SerializeField] private Stats aim;
        [SerializeField] private int groupNumber = 1;
        [SerializeField] private int groupNumberImmutable = 1;
        /* divine for n group problem:
         * 1. little odd health points loss after divine. Fix: plus the points into first or last health bar
         * 2. Always control Scriptable object to get odd point after divine at 0
         */
        public int Hp => TotalHealth();
        public int Atk => attack.Mutable;
        public int Def => defense.Mutable;
        public int Crit => critical.Mutable;
        public int Spd => speed.Mutable;
        public int Step => step.Mutable;
        public int Agi => agility.Mutable;
        public int Aim => aim.Mutable;
        public int GroupNumber => groupNumber;
        public int GroupNumberImmutable => groupNumberImmutable;
        
        public event EventHandler<CharacterDeathEventArgs> OnDeath;
        public event EventHandler<CharacterStatsChangeEventArgs> OnStatsChange;

        public int GetDamage()
        {
            int totalDamage = 0;

            for (int i = 0; i < groupNumber; i++)
            {
                var damage = attack * 0.8f;

                if (critical / 500f - Random.value > 0)
                {
                    damage += attack * 0.25f;
                }

                totalDamage += Mathf.RoundToInt(damage);
            }

            return totalDamage;
        }

        public int TakeDamage(int damage)
        {
            if (damage < 0) return 0;
            
            var realDameTaken = Mathf.RoundToInt(Mathf.Clamp(damage - defense * 0.75f, damage * 0.2f, damage)); // defense just decrease maximum at 80% damage
            var damageAcrossHealth = realDameTaken;
            var loop = false;
            do
            {
                var shield = health.FirstOrDefault(h => h.StatsName == StatsType.Shield);
                
                if (shield is not null)
                {
                    var shieldAfterDamage = Mathf.Clamp(shield.Mutable - damageAcrossHealth, 0, shield.Immutable);

                    if (shieldAfterDamage == 0)
                    {
                        health.Remove(shield);
                        if (damageAcrossHealth - shield.Mutable > 0)
                        {
                            damageAcrossHealth -= shield.Mutable;
                            loop = true;
                        }
                        else
                        {
                            return realDameTaken;
                        }
                    }
                    else
                    {
                        shield.Mutable -= damageAcrossHealth;
                        return realDameTaken;
                    }
                }
            } while (loop);
            
            var healthBar = health[groupNumber - 1];

            var healthAfterDamage = Mathf.Clamp(healthBar.Mutable - damageAcrossHealth, 0, healthBar.Immutable);

            if (healthAfterDamage == 0)
            {
                var immutable = healthBar.Immutable * groupNumberImmutable;
                var oldV = TotalHealth();
                health.Remove(healthBar);
                var newV = TotalHealth();
                groupNumber--;
                
                OnStatsChange?.Invoke(this, new CharacterStatsChangeEventArgs(StatsType.Health, oldV, newV, immutable, groupNumber));
                if (CheckDeath())
                {
                    //Debug.Log("Health below 0. Active death event!");
                    OnDeath?.Invoke(this, new CharacterDeathEventArgs(-1));
                    return realDameTaken;
                }

                if (damageAcrossHealth - healthBar.Mutable > 0) // This is a feature not b-u-g. With group tag, attack odd after health bar removed was free
                {
                    /*damageAcrossHealth -= healthBar.Mutable;
                    Debug.Log("Health dissolve with odd " + damageAcrossHealth + " damage!");*/
                    return oldV - newV;
                }
                
                return realDameTaken;
            }
            else
            {
                var oldV = TotalHealth();
                healthBar.Mutable -= damageAcrossHealth;
                var newV = TotalHealth();
                var immutable = healthBar.Immutable * groupNumberImmutable;
                OnStatsChange?.Invoke(this, new CharacterStatsChangeEventArgs(StatsType.Health, oldV, newV, immutable, groupNumber));
                return realDameTaken;
            }

            //return -1; // test wrong logic
        }

        /// <summary>
        /// Healing character can resolve 3 method: Healing amount/Healing scale by max health/Healing scale by lost health
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="isScale"></param>
        /// <param name="scaleWithMaxHealth"></param>
        /// <returns></returns>
        public int Healing(float quantity, bool isScale, bool scaleWithMaxHealth)
        {
            if (quantity < 0) return 0;
            
            int realValueHeal = 0;
            var immutable = health[0].Immutable * groupNumberImmutable;
            var amount = isScale ? Mathf.RoundToInt(scaleWithMaxHealth ? immutable * quantity : (immutable - TotalHealth()) * quantity) : Mathf.RoundToInt(quantity);
                
            foreach (var healthBar in health)
            {
                var oldV = TotalHealth();
                healthBar.Mutable = Mathf.Clamp(healthBar.Mutable + amount, healthBar.Mutable, healthBar.Immutable);
                var newV = TotalHealth();

                realValueHeal += newV - oldV;
                OnStatsChange?.Invoke(this, new CharacterStatsChangeEventArgs(StatsType.Health, oldV, newV, immutable, groupNumberImmutable));
            }

            return realValueHeal;
        }

        /// <summary>
        /// Change stats mutable.
        /// </summary>
        /// <param name="statsType">Except health and shield.</param>
        /// <param name="quantity">Can be negative.</param>
        public int StatsChange(StatsType statsType, int quantity)
        {
            var oldValue = -1;
            var newValue = -1;
            var immutable = -1;
            switch (statsType)
            {
                case StatsType.Attack:
                    oldValue = attack.Mutable;
                    attack.Mutable = Mathf.Clamp(attack.Mutable + quantity, 0, int.MaxValue);
                    newValue = attack.Mutable;
                    immutable = attack.Immutable;
                    break;
                case StatsType.Defense:
                    oldValue = defense.Mutable;
                    defense.Mutable = Mathf.Clamp(defense.Mutable + quantity, 0, int.MaxValue);
                    newValue = defense.Mutable;
                    immutable = defense.Immutable;
                    break;
                case StatsType.Critical:
                    oldValue = critical.Mutable;
                    critical.Mutable = Mathf.Clamp(critical.Mutable + quantity, 0, int.MaxValue);
                    newValue = critical.Mutable;
                    immutable = critical.Immutable;
                    break;
                case StatsType.Speed:
                    oldValue = speed.Mutable;
                    speed.Mutable = Mathf.Clamp(speed.Mutable + quantity, 0, int.MaxValue);
                    newValue = speed.Mutable;
                    immutable = speed.Immutable;
                    break;
                case StatsType.Agility:
                    oldValue = agility.Mutable;
                    agility.Mutable = Mathf.Clamp(agility.Mutable + quantity, 0, int.MaxValue);
                    newValue = agility.Mutable;
                    immutable = agility.Immutable;
                    break;
            }

            if (immutable != -1)
            {
                OnStatsChange?.Invoke(this, new CharacterStatsChangeEventArgs(statsType, oldValue, newValue, immutable, groupNumber));

                return newValue - oldValue;
            }

            return 0;
        }

        public int StatsChangeArea(StatsType statsType, int quantity)
        {
            return 0;
        }

        private bool CheckDeath() => health.Count == 0;

        private int TotalHealth()
        {
            var total = 0;
            
            health.ForEach(h => total += h.Mutable);

            return total;
        }

        public Status(Status status)
        {
            health = new(status.health);
            attack = new(status.attack);
            defense = new(status.defense);
            critical = new(status.critical);
            speed = new(status.speed);
            step = new(status.step);
            agility = new(status.agility);
            aim = new(status.aim);
            groupNumber = status.groupNumber;
            groupNumberImmutable = status.groupNumberImmutable;
        }

        public Status(ScriptableObjects.Status status, CharacterTag? tag = null)
        {
            // with Group(n) tag, HP, ATK, DEF divide for n
            health = new ();
            
            if (tag is not null)
            {
                var isGroup = tag.ToString().StartsWith("Group");

                if (isGroup)
                {
                    var groupAmountStr = tag.ToString().Substring(5);
                    if (int.TryParse(groupAmountStr, out int groupAmount))
                    {
                        groupNumber = groupAmount;
                        groupNumberImmutable = groupAmount;
                        for (int i = 0; i < groupAmount; i++)
                        {
                            health.Add(
                                new ()
                                {
                                    StatsName = StatsType.Health,
                                    Immutable = status.Health / groupAmount,
                                    Mutable = status.Health / groupAmount
                                });
                        }
                        
                        attack = new()
                        {
                            StatsName = StatsType.Attack,
                            Immutable = status.Attack / groupAmount,
                            Mutable = status.Attack / groupAmount
                        };

                        defense = new()
                        {
                            StatsName = StatsType.Defense,
                            Immutable = status.Defense / groupAmount,
                            Mutable = status.Defense / groupAmount
                        };
                    }
                }
                else
                {
                    health.Add(new ()
                    {
                        StatsName = StatsType.Health,
                        Immutable = status.Health,
                        Mutable = status.Health
                    });
                    
                    attack = new()
                    {
                        StatsName = StatsType.Attack,
                        Immutable = status.Attack,
                        Mutable = status.Attack
                    };

                    defense = new()
                    {
                        StatsName = StatsType.Defense,
                        Immutable = status.Defense,
                        Mutable = status.Defense
                    };
                }
            }
            else
            {
                attack = new()
                {
                    StatsName = StatsType.Attack,
                    Immutable = status.Attack,
                    Mutable = status.Attack
                };

                defense = new()
                {
                    StatsName = StatsType.Defense,
                    Immutable = status.Defense,
                    Mutable = status.Defense
                };
            }

            critical = new()
            {
                StatsName = StatsType.Critical,
                Immutable = status.Critical,
                Mutable = status.Critical
            };
            
            speed = new()
            {
                StatsName = StatsType.Speed,
                Immutable = status.Speed,
                Mutable = status.Speed
            };
            
            step = new()
            {
                StatsName = StatsType.Step,
                Immutable = status.Step,
                Mutable = status.Step
            };
            
            agility = new()
            {
                StatsName = StatsType.Agility,
                Immutable = status.Agility,
                Mutable = status.Agility
            };
            
            aim = new()
            {
                StatsName = StatsType.Aim,
                Immutable = status.Aim,
                Mutable = status.Aim
            };
        }
    }
}