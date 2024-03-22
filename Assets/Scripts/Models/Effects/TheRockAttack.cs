using UnityEngine;

namespace Models.Effects
{
    public class TheRockAttack : EffectsAction
    {
        [SerializeField] private float speed;
        [SerializeField] private bool startFollow;
        public override void Activate(Vector3 target)
        {
            // Turn to target
            var targetDirection = target - transform.localPosition;
            var selfDirection = transform.TransformDirection(Vector3.forward);

            var newRotation= Vector3.RotateTowards(selfDirection, targetDirection, 360f, 0f);
            transform.localRotation = Quaternion.LookRotation(newRotation);
            transform.Rotate(0f, -90f, 0f);

            // Move to target & Destroy after goal
            targeted = target;
            startFollow = true;
        }

        private void Update()
        {
            if (!startFollow) return;
            
            var curPos = transform.localPosition;
            var newPosition = Vector3.MoveTowards(curPos, targeted, speed * Time.deltaTime);
            transform.localPosition = newPosition;
            
            if (Vector3.Distance(transform.localPosition, targeted) < 0.01f)
            {
                startFollow = false;
                Destroy(gameObject);
            }
        }
    }
}