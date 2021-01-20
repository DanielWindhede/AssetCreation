using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    /*
    [SerializeField] private List<Move> moves;

    Animator animator;
    Moveset moveset;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        moveset = new Moveset(animator);

        foreach (Move m in moves)
        {
            moveset.AddMove(m);
        }

        moveset.Initialize();

        moveset.ChangeState("idle", animator);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        moveset.DoFixedUpdate();
        if (moveset.IsHitboxActive)
        {
            frames++;
            print("Current frame: " + FrameCheckerMulti.CurrentFrameApproximation(animator, moveset.CurrentMove) + "Active for frame: " + frames);
        }
        else
        {
            frames = 0;
        }
    }
    int frames;

    public bool attack;
    public bool cancel;
    private void OnValidate()
    {
        if (attack)
        {
            attack = false;
            moveset.ChangeState("attack", animator);
        }
        if (cancel)
        {
            cancel = false;
            moveset.ChangeState("idle", animator);
            //animator.Play("idle", 0);
        }
    }

    public bool debug = false;
    private void OnDrawGizmos()
    {
        if (moveset != null && moveset.IsHitboxActive && debug)
        {
            //Gizmos.matrix = Matrix4x4.TRS(transform.position + _offset, transform.rotation, transform.localScale);
            foreach (Hit hit in moveset.CurrentMove.hits)
            {
                if (hit.follow == null)
                    Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(hit.Pos), transform.rotation, transform.localScale);
                else
                    Gizmos.matrix = Matrix4x4.TRS(hit.follow.TransformPoint(hit.Pos), hit.follow.rotation, transform.localScale);

                switch (hit.priority)
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

                Gizmos.DrawWireCube(Vector3.zero, hit.Size);
            }

            /*
            if (circleCollider)
            {
                Gizmos.DrawWireSphere(Vector3.zero, circleRadius);
            }
            else
            {
                Gizmos.DrawWireCube(Vector3.zero, _size);
            }
            */
    /*
        }
    }
*/
}
/*
public class Hitbox : MonoBehaviour, IFrameCheckHandler
{
    public FrameChecker checker;

    bool active;
    void IFrameCheckHandler.onHitFrameEnd()
    {
        active = false;
        frames = 0;
        //print("frame end");
    }

    void IFrameCheckHandler.onHitFrameStart()
    {
        active = true;
        //print("frame start");
    }

    void IFrameCheckHandler.onLastFrameEnd()
    {
        animator.Play("idle", 0);
        //print("last frame end");
    }

    void IFrameCheckHandler.onLastFrameStart()
    {
        //print("last frame start");
    }

    public bool attack;
    public bool cancel;
    public AnimationClipExtended clip;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        if (checker == null)
            checker = new FrameChecker();

        animator = GetComponent<Animator>();
        clip.initialize();
        checker.initialize(this, clip);
    }

    int frames;
    // Update is called once per frame
    void FixedUpdate()
    {
        checker.checkFrames();
        if (active)
        {
            frames++;
            print("Current frame: " + checker.CurrentFrame + ", Counted active frames: " + frames);
        }
    }


    private void OnValidate()
    {
        if (attack)
        {
            attack = false;
            animator.Play("attack", 0);
            checker.initCheck();
        }
        if (cancel)
        {
            cancel = false;
            animator.Play("idle", 0);
        }
    }
}
*/