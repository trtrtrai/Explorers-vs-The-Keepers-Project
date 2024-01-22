using Controllers;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;

namespace Models.Spells
{
    public static class SpellsExecute
    {
        public static bool Activate(Character character, SpellsEffect effect)
        {
            if (effect is StatsSpells statsSpells)
            {
                if (statsSpells.StatsType == StatsType.Health
                    || statsSpells.StatsType == StatsType.Shield
                    || statsSpells.StatsType == StatsType.Step
                    || statsSpells.StatsType == StatsType.Aim) return false;
                
                if (!character.gameObject.activeSelf) return false;
                
                var changeAmount = statsSpells.Quantity;

                if (statsSpells.IsScale)
                {
                    var mutable = -1;
                    var status = character.GetStatus();

                    switch (statsSpells.StatsType)
                    {
                        case StatsType.Attack:
                            mutable = status.Atk;
                            break;
                        case StatsType.Defense:
                            mutable = status.Def;
                            break;
                        case StatsType.Critical:
                            mutable = status.Crit;
                            break;
                        case StatsType.Speed:
                            mutable = status.Spd;
                            break;
                        case StatsType.Agility:
                            mutable = status.Agi;
                            break;
                    }

                    if (mutable == -1) return false;

                    changeAmount *= mutable;
                }

                var realAmount = statsSpells.IsBoost ? Mathf.RoundToInt(changeAmount) : -Mathf.RoundToInt(changeAmount);
                var realChange = character.StatsChange(statsSpells.StatsType, realAmount);

                if (statsSpells.EffectTimer > 0f)
                {
                    var script = character.AddComponent<StatsSpellsAttach>();

                    if (script.Setup(statsSpells.EffectTimer, statsSpells.StatsType, realChange))
                    {
                        script.StartCounting();
                        return true;
                    }
                }
            }
            else if (effect is HealthEffectSpells healthEffSpells)
            {
                if (healthEffSpells.IsDamage)
                {
                    var damage = Mathf.RoundToInt(healthEffSpells.Quantity); // default IsScale == false
                    var maxHealth = character.CharacterInfo.Status.Health;
                    
                    if (healthEffSpells.IsScale)
                    {
                        if (healthEffSpells.IsScaleWithMaxHealth)
                        {
                            damage = Mathf.RoundToInt(maxHealth * healthEffSpells.Quantity);
                        }
                        else
                        {
                            damage = Mathf.RoundToInt((maxHealth - character.GetStatus().Hp) *
                                                      healthEffSpells.Quantity);
                        }
                    }

                    if (healthEffSpells.IsPassDefense)
                    {
                        var addedDefResist = Mathf.RoundToInt(Mathf.Clamp(damage - character.GetStatus().Def * 0.75f, damage * 0.2f, damage));
                        var result= character.TakeDamage(damage + addedDefResist);
                        Debug.Log(character.name + " take " + result + " damage from pass defense spells.");
                        return true;
                    }
                    else
                    {
                        var result= character.TakeDamage(Mathf.RoundToInt(damage));
                        Debug.Log(character.name + " take " + result + " damage from spells.");
                        return true;
                    }
                }
                
                var healing = character.Healing(healthEffSpells.Quantity, healthEffSpells.IsScale, healthEffSpells.IsScaleWithMaxHealth);
                Debug.Log(character.name + " get heal " + healing + " point(s) from spells.");
                return true;
            }
            else if (effect is ControlSpells controlSpells)
            {
                switch (controlSpells.ControlType)
                {
                    case ControlType.Stun:
                        var stunScript = character.AddComponent<ControlSpellsAttach>();
                        stunScript.Setup(controlSpells.ControlType, controlSpells.EffectTimer);
                        return true;
                    case ControlType.MindControl:
                        return true;
                }
            }
            else if (effect is EnergyBoostSpells energyBoostSpells)
            {
                switch (energyBoostSpells.BoostType)
                {
                    case EnergyBoostType.BoostRegenerate:
                        return true;
                    case EnergyBoostType.ReduceConsume:
                        var eBoostScript = WorldManager.Instance.gameObject.AddComponent<EnergyReduceConsume>();
                        eBoostScript.Setup(energyBoostSpells.Target, energyBoostSpells.EnergyReducePerConsume, energyBoostSpells.Loop);
                        return true;
                }
            }
            else if (effect is TeleportSpells teleportSpells)
            {
                character.Teleport(teleportSpells.TeleportType, teleportSpells.Step);
                return true;
            }
            else if (effect is SummonSpells summonSpells)
            {
                for (int i = 0; i < summonSpells.Quantity; i++)
                {
                    WorldManager.Instance.CreateCharacter(summonSpells.CharacterPrefab, summonSpells.RoadIndex, WorldManager.Instance.GetAllyTeam(), summonSpells);
                }
                
                return true;
            }
            else if (effect is EnvironmentSpells environmentSpells)
            {
                foreach (var tile in environmentSpells.ListSettingUp)
                {
                    var envir = WorldManager.InstantiatePrefab(environmentSpells.EnvironmentPrefab);
                    envir.transform.localPosition = tile.transform.localPosition;
                    var script = envir.GetComponentInChildren<PoisonSwamp>(); //Type will be abstract after
                    script.Setup(environmentSpells.DamagePerContact, environmentSpells.EffectTimer, environmentSpells.IgnoreList);
                }
            }
            else if (effect is ApocalypseSpells apocalypseSpells)
            {
                if (character is not null)
                {
                    var characterGroup = character.GetStatus().GroupNumber;

                    for (int i = 0; i < characterGroup; i++)
                    {
                        character.TakeDamage(9999);
                    }
                }
            }
            
            return false;
        }
    }
}