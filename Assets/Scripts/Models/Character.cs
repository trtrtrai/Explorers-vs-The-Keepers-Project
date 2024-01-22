using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using EventArgs;
using Models.Interfaces;
using ScriptableObjects;
using UnityEngine;
using CharacterInfo = ScriptableObjects.CharacterInfo;

namespace Models
{
    [RequireComponent(typeof(CharacterBehaviour))]
    public class Character : MonoBehaviour, IMovable, IAttackable
    {
        public TileData Position;
        
        [SerializeField] protected int team;
        [SerializeField] protected int roadIndex;
        [SerializeField] protected int positionIndex;

        [SerializeField] protected CharacterInfo characterInfo;

        [SerializeField] protected Status status;

        public BitArray CharacterTag;

        [SerializeField] protected List<Transform> characterObjs;

        [SerializeField]
        protected CharacterBehaviour behaviour;
        [SerializeField]
        protected CharacterMoving moving;
        [SerializeField]
        protected CharacterAttacking attacking;

        protected IEnumerator movingCoroutine;
        [SerializeField] protected bool isMoving;
        [SerializeField] protected bool isAttacking;
        public bool IsMoving => isMoving;
        public bool IsAttacking => isAttacking;

        public event EventHandler<CharacterDeathEventArgs> OnCharacterDeath;
        public event EventHandler<CharacterStatsChangeEventArgs> OnCharacterStatsChange;
        private void Awake()
        {
            SetupMoving(); // it will be auto setup when IMovable found in every character
            SetupAttacking();

            characterObjs = new List<Transform>(gameObject.GetComponentsInChildren<MeshFilter>().Select(m => m.transform.parent));
            
            // move it to factory
            var groupTag = characterInfo.CharacterTags.FirstOrDefault(t => t.ToString().StartsWith("Group"));
            status = new Status(characterInfo.Status, groupTag);
            status.OnDeath += OnDeath;
            status.OnStatsChange += OnStatsChange;
        }

        private void Start()
        {
            CharacterTag = WorldManager.Instance.CreateCharacterTag(characterInfo.CharacterTags);
        }

        public void Setup(int roadIndex, int team)
        {
            this.team = team;
            this.roadIndex = roadIndex;
            
            var teamLayer = team == 0 ? LayerMask.NameToLayer("Team1") : LayerMask.NameToLayer("Team2");

            gameObject.layer = teamLayer;
            foreach (Transform t in transform)
            {
                t.gameObject.layer = teamLayer;
            }
        }

        public void SetupMoving()
        {
            if (!TryGetComponent(out moving))
            {
                moving = gameObject.AddComponent<CharacterMoving>();
            }

            if (TryGetComponent(out behaviour))
            {
                behaviour.MoveToward += MoveAction;
            }
        }

        protected virtual void MoveAction()
        {
            isMoving = true;
            var curPath = WorldManager.Instance.GetPath(team, roadIndex, positionIndex, status.Step);
            if (curPath.Count == 0) return;
            
            movingCoroutine = WalkCooldown(curPath);
            StartCoroutine(movingCoroutine);
        }

        private IEnumerator WalkCooldown(List<TileData> path)
        {
            foreach (var target in path)
            {
                if (!isMoving) break;
                
                TurnTo(target.transform.localPosition);
                moving.MoveOn(target, status.Spd / 200f);
                
                while (!transform.localPosition.x.Equals(target.transform.localPosition.x) ||
                    !transform.localPosition.z.Equals(target.transform.localPosition.z))
                {
                    yield return null;
                }

                //Debug.Log("Check run");
                yield return new WaitForSeconds(100f / status.Spd);
                Position = target;
                positionIndex++;
                var aimPath = WorldManager.Instance.GetAimPath(team, roadIndex, positionIndex, status.Aim);
                if (aimPath.Count != 0) behaviour.EnemyAroundCheck(target, aimPath, status.Aim);
            }

            var timer = 100f / status.Spd + 60f / status.Agi;
            var p = WorldManager.Instance.GetAimPath(team, roadIndex, positionIndex, status.Aim);
            if (p.Count == 0) timer = 0f;
            while (timer > 0f && isMoving)
            {
                behaviour.EnemyAroundCheck(Position, p, status.Aim);

                timer -= Time.deltaTime / 2;
                yield return null;
            }
            
            isMoving = false;
        }

        protected void TurnTo(Vector3 target)
        {
            var targetDirection = target - transform.localPosition;
            var selfDirection = transform.TransformDirection(Vector3.forward);
            targetDirection.y = 0;
            var angle = Vector3.SignedAngle(selfDirection, targetDirection, Vector3.up);
            //Debug.Log(angle + " * " + rotateDirect);
            transform.Rotate(new Vector3(0f, angle, 0f));
        }

        public void SetupAttacking()
        {
            if (!TryGetComponent(out attacking))
            {
                attacking = gameObject.AddComponent<CharacterAttacking>();
            }
            
            if (TryGetComponent(out behaviour))
            {
                behaviour.AttackTo += AttackAction;
            }
        }

        public void DisableAttacking()
        {
            behaviour.AttackTo -= AttackAction;
        }

        protected virtual void AttackAction(TileData position, Character target)
        {
            if (position is null)
            {
                isAttacking = false;
                return;
            }
            
            isAttacking = true;
            isMoving = false;
            //StopCoroutine(movingCoroutine);

            StartCoroutine(AttackCooldown(position, target));
        }

        private IEnumerator AttackCooldown(TileData position, Character target)
        {
            TurnTo(target.transform.localPosition);
            var timer = 0.2f;
            var cannotAtk = false;
            while (!Position.Equals(position))
            {
                if (timer <= 0f)
                {
                    cannotAtk = true;
                }
                else
                {
                    timer -= Time.deltaTime / 2;
                }
                yield return null;
            }

            if (!cannotAtk)
            {
                attacking.AttackEnemy(target, status.GetDamage());
                
                yield return new WaitForSeconds(100f / status.Agi); // attack delay
            }
            
            var aimPath = WorldManager.Instance.GetAimPath(team, roadIndex, positionIndex, status.Aim);
            if (aimPath.Count != 0) behaviour.EnemyAroundCheck(position, aimPath, status.Aim);
        }
        
        public int TakeDamage(int value) => status.TakeDamage(value);

        public int Healing(float quantity, bool isScale, bool scaleWithMaxHealth) =>
            status.Healing(quantity, isScale, scaleWithMaxHealth);

        public void Teleport(TeleportType type, int step)
        {
            OnDisable();
            
            switch (type)
            {
                case TeleportType.BackToHeadquarter:
                    positionIndex = 0;
                    Position = team == 0
                        ? WorldManager.Instance.Team1StartPoint
                        : WorldManager.Instance.Team2StartPoint;
                    break;
                case TeleportType.Forward:
                    var pathForward = WorldManager.Instance.GetPath(team, roadIndex, positionIndex, step);
                    positionIndex += pathForward.Count;
                    Position = pathForward.Last();
                    break;
                case TeleportType.Backward:
                    positionIndex = Math.Clamp(positionIndex - step, 0, positionIndex);
                    var pathBackward = WorldManager.Instance.GetPath(team, roadIndex, positionIndex, step);
                    Position = pathBackward.First();
                    break;
            }

            transform.localPosition = Position.transform.localPosition;
            Enable();
        }
        
        public CharacterInfo CharacterInfo => characterInfo;

        public Status GetStatus() => new(status);

        public int StatsChange(StatsType statsType, int quantity) => status.StatsChange(statsType, quantity);

        private void OnStatsChange(object sender, CharacterStatsChangeEventArgs args)
        {
            if (sender is not Status || !sender.Equals(status)) return;
            
            OnCharacterStatsChange?.Invoke(this, new CharacterStatsChangeEventArgs(args.StatsType, args.OldValue, args.NewValue, args.Immutable, args.GroupNumber));

            if (characterObjs.Count > status.GroupNumber)
            {
                characterObjs[status.GroupNumber].gameObject.SetActive(false);
            }
        }

        private void OnDeath(object sender, CharacterDeathEventArgs args)
        {
            if (sender is not Status || !sender.Equals(status)) return;
            
            OnCharacterDeath?.Invoke(this, new CharacterDeathEventArgs());
            
            gameObject.SetActive(false);
            Destroy(gameObject, 0.5f);
        }

        protected virtual void OnDisable()
        {
            behaviour.MoveToward -= MoveAction;
            behaviour.AttackTo -= AttackAction;
            isMoving = false;
            isAttacking = false;
            StopAllCoroutines();
        }

        protected virtual void Enable()
        {
            behaviour.MoveToward += MoveAction;
            behaviour.AttackTo += AttackAction;
        }
    }
}