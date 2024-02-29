using System;
using System.Collections;
using Extensions;
using Models;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(Character))]
    public class CharacterMoving : MonoBehaviour
    {
        [SerializeField]
        private TileData myTilePosition;

        private void Awake()
        {
            if (TryGetComponent(out Character character))
                myTilePosition = character.Position;
        }

        public void MoveOn(TileData target, float speed, Action movingAnim)
        {
            if (TryGetComponent(out Character character))
                myTilePosition = character.Position;
            
            if (myTilePosition is null || target is null) return;
            
            int direct = myTilePosition.TilePosition.GetDirectionFromPosition(target.TilePosition);

            if (direct == -1 ||
                !myTilePosition.GetNeighbor(direct).Equals(target)) return;
            //Debug.Log("Moving " + myTilePosition.TilePosition + " to " + target.TilePosition);
            var targetPos = target.transform.localPosition;
            targetPos.y = 0;
            StartCoroutine(Moving(targetPos, speed, movingAnim));
        }

        private IEnumerator Moving(Vector3 target, float speed, Action movingAnim)
        {
            var position = transform.localPosition;
            var distance = (target - position).magnitude;

            movingAnim();
            while (distance > 0.01f)
            {
                /*var movementDirect = (target - position).normalized;
                var movePosition = movementDirect * (speed * Time.deltaTime);*/
                transform.localPosition = Vector3.MoveTowards(position, target, speed * Time.deltaTime);

                position = transform.localPosition;
                distance = (target - position).magnitude;

                yield return null;
            }
            
            transform.localPosition = target;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}