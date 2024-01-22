using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Models.SpecialCharacter
{
    public sealed class MinefieldWorker : Reinforced
    {
        private int loop = 1;
        private int roadStart = -1;
        
        protected override void AttackAction(TileData position, Character target)
        {
            isMoving = false;
            // setup obj
            var obj = Instantiate(objectSetupPrefab);
            obj.transform.localPosition = position.transform.localPosition;
            obj.layer = gameObject.layer;

            // move to headquarter
            var pathBackToHQ = WorldManager.Instance.GetPathBackToHeadquarter(team, roadIndex, positionIndex);
            if (pathBackToHQ.Count == 0) return;

            StartCoroutine(MinefieldWorkerMoveBackToHeadquarter(pathBackToHQ));
        }

        protected override void MoveAction()
        {
            isMoving = true;

            if (objAmount == 0)
            {
                //Debug.Log("Reinforce completed");
                //Destroy
                TakeDamage(9999);
            }
            else
            {
                if (roadStart == -1)
                {
                    roadStart = roadIndex;
                }
                else
                {
                    roadIndex = (roadIndex + 1) % WorldManager.Instance.RoadAmount;

                    if (roadIndex == roadStart) loop++;
                }
            
                var curPath = WorldManager.Instance.GetPath(team, roadIndex, positionIndex, loop);
                if (curPath.Count == 0) return;

                StartCoroutine(MinefieldWorkerMoveSetup(curPath));
            }
        }

        private IEnumerator MinefieldWorkerMoveSetup(List<TileData> path)
        {
            foreach (var target in path)
            {
                TurnTo(target.transform.localPosition);
                moving.MoveOn(target, status.Spd / 200f);

                while (!transform.localPosition.x.Equals(target.transform.localPosition.x) ||
                       !transform.localPosition.z.Equals(target.transform.localPosition.z))
                {
                    yield return null;
                }
                
                yield return new WaitForSeconds(100f / status.Spd);
                Position = target;
                positionIndex++;
            }

            isAttacking = true;
        }
        
        private IEnumerator MinefieldWorkerMoveBackToHeadquarter(List<TileData> path)
        {
            foreach (var target in path)
            {
                TurnTo(target.transform.localPosition);
                moving.MoveOn(target, status.Spd / 200f);

                while (!transform.localPosition.x.Equals(target.transform.localPosition.x) ||
                       !transform.localPosition.z.Equals(target.transform.localPosition.z))
                {
                    yield return null;
                }
                
                yield return new WaitForSeconds(100f / status.Spd);
                Position = target;
            }

            positionIndex = 0;
            objAmount--;
            isAttacking = false;
        }
    }
}