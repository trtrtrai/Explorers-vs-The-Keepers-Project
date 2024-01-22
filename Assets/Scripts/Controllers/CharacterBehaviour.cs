using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// Implement when character move and should attack who by event.
    /// </summary>
    public class CharacterBehaviour : MonoBehaviour
    {
        protected Character character;

        public delegate void MoveAction();
        public event MoveAction MoveToward;
        
        public delegate void AttackAction(TileData position, Character enemy);
        public event AttackAction AttackTo;
        private void Awake()
        {
            character = GetComponent<Character>();
            
            //this script must be know how many Action character have to autorun
        }

        protected virtual void Update()
        {
            if (!character.IsMoving && !character.IsAttacking)
            {
                //Debug.Log("Moved");
                MoveToward?.Invoke();
            }
        }

        public virtual void EnemyAroundCheck(TileData position, List<TileData> aimPath, int aim)
        {
            /*Debug.Log("Aim Path");
            foreach (var a in aimPath)
            {
                    Debug.Log(a.TilePosition);
            }*/
            for (int j = 0; j <= aim * 2; j++)
            {
                if (aimPath.ElementAtOrDefault(j) is null) break;
                RaycastHit[] hit = new RaycastHit[32];
                
                var futurePosition = new Vector3(1.73f * position.TilePosition.X, 0f, 1.5f * position.TilePosition.Z);
                var direction = (aimPath[j].transform.localPosition - futurePosition).normalized;
                float distance = Mathf.FloorToInt((position.TilePosition - aimPath[j].TilePosition).magnitude) * 1.75f; //distance depend aim * {distance of next tile path}

                var ownLayer = LayerMask.LayerToName(gameObject.layer);
                int layerMask = WorldManager.GetEnemyLayer(ownLayer);
                //Debug.Log(position.TilePosition + " " + aimPath[j].TilePosition + " " + distance);
                
                if (character.Position.Equals(aimPath[j]))
                {
                    //Debug.Log("Zero direction");
                    var positionOffset = transform.TransformDirection(Vector3.forward) * 0.5f;
                    futurePosition -= positionOffset;
                    direction = transform.TransformDirection(Vector3.forward);
                    distance = 1f;
                }
                
                var size = Physics.RaycastNonAlloc(futurePosition, direction, hit, distance, 1 << layerMask);
                var enemy = GetTarget(hit, size);

                if (enemy is not null)
                {
                    AttackTo?.Invoke(position, enemy);
                    return;
                }
            }
            
            AttackTo?.Invoke(null, null);
        }

        private Character GetTarget(RaycastHit[] hits, int size)
        {
            //Defender -- HqPrior: Defender attack HqPrior first, HqPrior attack Defender first, Headquarter seconds
            //SkyForce -- SkyAttack: only SkyAttack can attack SkyForce
            //Destroyer -- Headquarter: only attack Headquarter
            //All except HqPrior --: SDefender always attack, higher priority than Headquarter
            
            var priorityNum = -1;
            Character topAttackEnemy = null;
            var listCharTagRefAtk = WorldManager.Instance.GetAllPrioritiesOf(character.CharacterTag);
            for (int i = 0; i < size; i++)
            {
                //Debug.Log(hit[i].transform.parent.name + " did hit!");
                if (hits[i].transform.parent.TryGetComponent(out Character enemy))
                {
                    var enemyPriority = WorldManager.Instance.GetPriorityOf(enemy.CharacterTag, listCharTagRefAtk);

                    if (enemyPriority > priorityNum)
                    {
                        priorityNum = enemyPriority;
                        topAttackEnemy = enemy;
                    }
                }
            }

            if (topAttackEnemy is null) return null;
            
            //Debug.Log("Priority attack: " + priorityNum + ". Enemy name: " + topAttackEnemy.name);
            return topAttackEnemy;
        }

        protected void InvokeAttackTo(TileData position, Character target)
        {
            AttackTo?.Invoke(position, target);
        }
    }
}