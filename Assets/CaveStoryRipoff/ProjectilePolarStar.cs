using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.CaveStory
{
    public class ProjectilePolarStar : Projectile
    {
        private bool isDestroyed;

        private float offset;
        [SerializeField] private float speed = 1.6f; // G.U
        [SerializeField] private float travelDistance = 3.5f;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        float timer;
        protected override void Update()
        {
            base.Update();

            if (!isDestroyed)
            {
                if (offset > travelDistance)
                {
                    owner.OnProjectileDestroy(this);
                    isDestroyed = true;
                }
                else
                {
                    Vector2 dir = GetVecFromDirection(Direction);

                    offset += speed * Time.deltaTime;
                    timer += Time.deltaTime;
                    transform.Translate(new Vector3(dir.x, dir.y, 0) * speed * Time.deltaTime);
                }
            }
        }
    }
}