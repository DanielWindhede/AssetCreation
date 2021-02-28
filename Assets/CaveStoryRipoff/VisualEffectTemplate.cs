using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.CaveStory
{
    [CreateAssetMenu]
    public class VisualEffectTemplate : ScriptableObject
    {
        public Sprite initialSprite;
        public Animator animator;
    }
}