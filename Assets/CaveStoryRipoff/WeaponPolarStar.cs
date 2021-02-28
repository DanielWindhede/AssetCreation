using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.CaveStory
{
    public class WeaponPolarStar : Weapon
    {
        bool pressed;

        public override void DoUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                pressed = true;
                Experience -= 1;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                pressed = true;
                Experience += 1;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                Shoot();
            }

            if (pressed)
            {
                pressed = false;
                Debug.Log("Exp: " + Experience + ", Fraction: " + ExperienceFraction());
            }
        }

        public override void Awake()
        {
            base.Awake();
            projectileCount = new HashSet<Projectile>();
            OnProjectileDestroy += DestroyProjectle;
        }

        private void OnDestroy()
        {
            OnProjectileDestroy -= DestroyProjectle;
        }

        private void Shoot()
        {
            if (projectileCount.Count < projectileLimit)
            {
                ProjectilePolarStar p = GameObject.Instantiate(GetCurrentProjectile).GetComponent<ProjectilePolarStar>();
                p.owner = this;
                p.Direction = this.Direction;
                p.transform.position = (Vector2)transform.position + p.GetVecFromDirection(Direction) * 0.75f + Vector2.down * 0.2f;

                projectileCount.Add(p);
            }
        }

        protected override void DestroyProjectle(Projectile projectile)
        {
            projectileCount.Remove(projectile);
            Destroy(projectile.gameObject);
        }
    }
}