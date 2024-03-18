using System.Collections;
using UnityEngine;

namespace Controllers
{
    public class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private bool isDeath;
        [SerializeField] private AudioSource atkSfx;
        [SerializeField] private bool atkSfxAvailable;
        [SerializeField] private bool attacking;

        private void Start()
        {
            isDeath = false;
            anim = GetComponent<Animator>();
            atkSfx = GetComponentInParent<AudioSource>();
            atkSfxAvailable = atkSfx is not null;
            attacking = false;
            PlayIdle();
        }

        public void PlayIdle()
        {
            if (isDeath) return;
            /*anim.ResetTrigger("Walking");
            anim.SetTrigger("Idle");*/
            if (attacking)
            {
                atkSfx.Stop();
                attacking = false;
            }
            anim.Play("Armature|Idle");
        }

        public void PlayWalk()
        {
            if (isDeath) return;
            /*anim.ResetTrigger("Idle");
            anim.SetTrigger("Walking");*/
            if (attacking)
            {
                atkSfx.Stop();
                attacking = false;
            }
            anim.Play("Armature|Walk");
        }

        public void PlayAttack()
        {
            if (isDeath) return;
            
            if (atkSfxAvailable)
            {
                attacking = true;
                atkSfx.Play();
            }
            anim.Play("Armature|Attack");
        }

        public void PlayDeath()
        {
            if (isDeath) return;
            
            if (attacking)
            {
                atkSfx.Stop();
                attacking = false;
            }
            isDeath = true;
            anim.Play("Armature|Death");
            StartCoroutine(WaitToPlayDeath());
        }

        public void PlayDeathDestroy(GameObject obj)
        {
            if (isDeath) return;
            
            if (attacking)
            {
                atkSfx.Stop();
                attacking = false;
            }
            isDeath = true;
            anim.Play("Armature|Death");
            StartCoroutine(WaitToPlayDeathDestroy(obj));
        }

        IEnumerator WaitToPlayDeath()
        {
            var clip = anim.GetCurrentAnimatorClipInfo(0)[0];
            var deathLength = clip.clip.length + 0.2f;
            
            while (deathLength > 0f)
            {
                deathLength -= Time.deltaTime;

                yield return null;
            }
            
            gameObject.SetActive(false);
        }
        
        IEnumerator WaitToPlayDeathDestroy(GameObject obj)
        {
            var clip = anim.GetCurrentAnimatorClipInfo(0)[0];
            var deathLength = clip.clip.length + 0.2f;
            
            while (deathLength > 0f)
            {
                deathLength -= Time.deltaTime;

                yield return null;
            }
            
            gameObject.SetActive(false);
            obj.SetActive(false);
            Destroy(obj, 0.5f);
        }

        public float GetCurrentClipLength()
        {
            var clip = anim.GetCurrentAnimatorClipInfo(0)[0];
            return clip.clip.length + 0.2f;
        }
    }
}