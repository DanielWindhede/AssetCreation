using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.FightingGame
{
    // DW
    [RequireComponent(typeof(HitboxLegacy))]
    public class HitboxControllerLegacy : MonoBehaviour
    {
        private HitboxGroupLegacy _parent;
        private HitboxLegacy _hitbox;

        private void Awake()
        {
            _parent = GetComponentInParent<HitboxGroupLegacy>();
            _hitbox = GetComponent<HitboxLegacy>();
            _hitbox.enabled = false;
        }

        private void OnEnable()
        {
            _parent.onEnableHitboxes += Enable;
            _parent.onDisableHitboxes += Disable;
        }

        private void OnDisable()
        {
            _parent.onEnableHitboxes -= Enable;
            _parent.onDisableHitboxes -= Disable;
        }

        private void Enable(int id)
        {
            if (id < 1 || _hitbox.id == id) // If ID is less than or equal to 0; every hitbox is enabled
            {
                _hitbox.enabled = true;
            }
        }

        private void Disable(int id)
        {
            if (id < 1 || _hitbox.id == id)
            {
                _hitbox.enabled = false;
            }
        }
    }
}