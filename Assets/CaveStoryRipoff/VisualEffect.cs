using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.CaveStory
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class VisualEffect : MonoBehaviour
    {
        [SerializeField] private VisualEffectTemplate effect;

        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private IEnumerator wait;
        // Start is called before the first frame update
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = effect.initialSprite;

            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = effect.animator.runtimeAnimatorController;
            wait = IWaitForAnimation();
            StartCoroutine(wait);
        }

        private IEnumerator IWaitForAnimation()
        {
            /*
            while (animator.)
            {

            }
            */
            yield return null;

        }
    }
}