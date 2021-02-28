using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.FightingGame
{
    public class HitboxDrawer : MonoBehaviour
    {
        public Move move;
        public int frame;
        public bool drawHitboxes = true;
        public bool inEditorDraw = true;

        private Hit current(int index)
        {
            return move.hitCollection[index];
        }

        private void OnDrawGizmos()
        {
            if (drawHitboxes && move != null && move.hitCollection != null)
            {
                if (!inEditorDraw)
                {
                    foreach (Hit hitbox in move.activeHitboxes)
                    {
                        DrawHitbox(hitbox);
                    }
                }
                else
                {
                    for (int i = 0; i < move.hitCollection.Length; i++)
                    {
                        if (current(i).isMultiHit)
                        {
                            List<int> start = new List<int>(current(i).MultiFrameStart);
                            List<int> end = new List<int>(current(i).MultiFrameEnd);

                            for (int j = 0; j < start.Count; j++)
                            {
                                if (start[j] <= frame && end[j] >= frame)
                                {
                                    DrawHitbox(current(i));
                                }
                                //print("Current: " + f + ", Start: " + start[j] + ", End; " + end[j]);
                            }
                        }
                        else if (current(i).frameStart <= frame && current(i).frameEnd >= frame)
                        {
                            DrawHitbox(current(i));
                        }
                    }
                }
            }
        }

        private void DrawHitbox(Hit hitbox)
        {
            switch (hitbox.priority)
            {
                case int n when (n == 0):
                    Gizmos.color = Color.red;
                    break;
                case int n when (n == 1):
                    Gizmos.color = Color.blue;
                    break;
                case int n when (n == 2):
                    Gizmos.color = Color.magenta;
                    break;
                case int n when (n >= 3):
                    Gizmos.color = Color.green;
                    break;
                default:
                    break;
            }

            Vector2 pos = hitbox.Pos + (hitbox.follow == null ? (Vector2)gameObject.transform.position : (Vector2)hitbox.follow.position);
            Gizmos.DrawWireCube(pos, hitbox.Size);
            print("drawing...");
        }
    }
}