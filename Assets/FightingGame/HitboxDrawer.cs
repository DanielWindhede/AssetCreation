using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxDrawer : MonoBehaviour
{
    public Move move;
    public int frame;

    private Hit current(int index)
    {
        return move.hitCollection[index];
    }

    private void OnDrawGizmos()
    {
        if (move != null && move.hitCollection != null)
        {
            for (int i = 0; i < move.hitCollection.Length; i++)
            {
                if (current(i).frameStart <= frame && current(i).frameEnd >= frame)
                {
                    switch (current(i).priority)
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

                    Vector2 pos = current(i).Pos + (current(i).follow == null ? (Vector2)gameObject.transform.position : (Vector2)current(i).follow.position);
                    Gizmos.DrawWireCube(pos, current(i).Size);
                }
            }
        }
    }
}
