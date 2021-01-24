using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/*
[System.Serializable]
public class Moveset : IFrameCheckHandler
{
    [SerializeField] private Dictionary<string, Move> moves;

    void IFrameCheckHandler.onHitFrameEnd()
    {
        throw new System.NotImplementedException();
    }

    void IFrameCheckHandler.onHitFrameStart()
    {
        throw new System.NotImplementedException();
    }

    void IFrameCheckHandler.onLastFrameEnd()
    {
        throw new System.NotImplementedException();
    }

    void IFrameCheckHandler.onLastFrameStart()
    {
        throw new System.NotImplementedException();
    }
    /*
    private bool _isActive;
    public bool IsHitboxActive { get { return _isActive; } }
    string _currentState;
    public string CurrentState { get { return _currentState; } }
    Dictionary<string, Move> _moves;
    FrameCheckerMulti _frameCheckerMulti;
    Animator _animator;

    public Moveset(Animator animator)
    {
        _animator = animator;
        _moves = new Dictionary<string, Move>();
        _frameCheckerMulti = new FrameCheckerMulti();
    }

    public void AddMove(Move move)
    {
        _moves.Add(move.animationStateName, move);
    }

    public void Initialize()
    {
        foreach (KeyValuePair<string, Move> entry in _moves)
        {
            entry.Value.Initialize(_animator);
        }

        _frameCheckerMulti.Initialize(this, _moves, _animator);
    }

    public void ChangeState(string stateName, Animator animator)
    {
        //((IFrameCheckHandler)this).onHitFrameEnd();
        _isActive = false;
        _currentState = stateName;
        animator.Play(_currentState);
        _frameCheckerMulti.Reset();
    }

    //bool isStateValid();

    public Move CurrentMove { get { return _frameCheckerMulti.CurrentClip(_currentState); } }

    public void DoFixedUpdate()
    {
        if (_currentState != "idle")
            _frameCheckerMulti.CheckFrames(_currentState);
    }

    void IFrameCheckHandler.onHitFrameEnd()
    {
        _isActive = false;
        Debug.Log("frame end " + _frameCheckerMulti.CurrentFrame(_currentState));
    }

    void IFrameCheckHandler.onHitFrameStart()
    {
        _isActive = true;
        Debug.Log("frame start " + _frameCheckerMulti.CurrentFrame(_currentState));
    }

    void IFrameCheckHandler.onLastFrameEnd()
    {
        _animator.Play("idle", 0);
        //print("last frame end");
    }

    void IFrameCheckHandler.onLastFrameStart()
    {
        //print("last frame start");
    }
}
*/
[System.Serializable, CreateAssetMenu]
public class Move : ScriptableObject
{
    // Cosmetic
    public string moveName;
    public string description;


    public AnimationClip clip;
    public string animationStateName;
    public int layerNumber;
    public int hitFrameStart;
    public int hitFrameEnd;
    public int _animationFullNameHash;

    public SerializedAnimationMap animMap = new SerializedAnimationMap();
    public float _animationSpeed;
    public int fps;
    public WrapMode wrapMode;
    public Hit[] hitCollection;
    public bool fixedSpeed;



    public int TotalFrames { get { return Mathf.RoundToInt(clip.length * clip.frameRate); } }


    LayerMask mask = 0b1;
    private HashSet<Hit> _activeHitboxes;
    private Dictionary<int, List<Hit>> _hitboxes;
    private Moveset _parent;
    private Animator _animator;

    private HashSet<GameObject> _alreadyHit;
    private List<Hit> _hitTimes;
    [HideInInspector] public Collider2D[] isHit;
    public void Initialize(Animator animator, Moveset parent)
    {
        _parent = parent;
        _animator = animator;
        _hitboxes = new Dictionary<int, List<Hit>>();
        _activeHitboxes = new HashSet<Hit>();

        _alreadyHit = new HashSet<GameObject>();
        _hitTimes = new List<Hit>();

        if (animator.isActiveAndEnabled)
        {
            string name = animator.GetLayerName(layerNumber) + "." + animationStateName;
            _animationFullNameHash = Animator.StringToHash(name);
        }

        foreach (Hit hit in hitCollection)
        {
            if (!_hitboxes.ContainsKey(hit.frameStart))
                _hitboxes.Add(hit.frameStart, new List<Hit>());
            _hitboxes[hit.frameStart].Add(hit);

            if (!_hitboxes.ContainsKey(hit.frameEnd))
                _hitboxes.Add(hit.frameEnd, new List<Hit>());
            _hitboxes[hit.frameEnd].Add(hit);
        }
    }

    public int CurrentFrameApproximation()
    {
        if (_animator.GetCurrentAnimatorStateInfo(layerNumber).loop)
            return (int)(_animator.normalizedTimeLooping(layerNumber) * TotalFrames);
        else
            return (int)(_animator.normalizedTime(layerNumber) * TotalFrames);
    }

    public void DoFixedUpdate()
    {
        if (_hitboxes.Keys.Count > 0) // small optimization
        {
            int currentFrame = CurrentFrameApproximation();

            if (_hitboxes.ContainsKey(currentFrame))
            {
                foreach (Hit hit in _hitboxes[currentFrame])
                {
                    if (hit.frameStart == currentFrame)
                        _activeHitboxes.Add(hit);
                    else if (hit.frameEnd == currentFrame)
                        _activeHitboxes.Remove(hit);
                }
            }

            DoCollisionCheck();
            DoDebug();
        }
    }


    private void DoCollisionCheck()
    {
        foreach (Hit hit in _activeHitboxes)
        {
            Transform t = hit.follow != null ? hit.follow : _parent.transform;
            hit.isHit = Physics2D.OverlapBoxAll(t.TransformPoint(hit.Pos), hit.Size, 0, mask);

            foreach (Collider2D enemy in isHit)
            {
                if (!_alreadyHit.Contains(enemy.gameObject)) // optimera contains så att om den finns så sätts en bool så att den inte behöver kolla igenom hela tiden, utan det är en dynamic algorithm
                {
                    _hitTimes.Add(hit);
                }
            }
        }

        if (_hitTimes.Count > 0)
        {
            int highestPriorityIndex = _hitTimes.Min(x => x.priority);
            /*
            for (int i = 1; i < _hitTimes.Count; i++)
            {
                if (_hitTimes[i].priority < _hitTimes[highestPriorityIndex].priority)
                {
                    highestPriorityIndex = i;
                }
            }
            */

            foreach (Collider2D enemy in _hitTimes[highestPriorityIndex].isHit)
            {
                if (enemy != null && !_alreadyHit.Contains(enemy.gameObject))
                {
                    var targetEntity = enemy.gameObject.GetComponent<Character>();

                    /*
                    if (!targetEntity.invulerable) // intangible behavior atm, stöd för båda borde finnas! Man blir samt slagen om invun. tar slut medans man blir träffad
                    */

                    Hit hitbox = _hitTimes[highestPriorityIndex];
                    _parent.IMPLEMENTCOLLISIONAHANDLING(hitbox);
                    _alreadyHit.Add(enemy.gameObject);
                }
            }

            _hitTimes.Clear();
        }
    }
    private void DoDebug()
    {
        
        foreach (Hit hit in _activeHitboxes)
        {
            DrawHitbox(hit, hit.follow != null ? hit.follow : _parent.transform);
        }
    }

    public static void DrawHitbox(Hit hit, Transform follow = null)
    {
        Color color = Color.red;
        Vector2 pos = hit.Pos + (follow == null ? Vector2.zero : (Vector2)follow.position);

        //Right
        Debug.DrawLine(new Vector3(pos.x + hit.Size.x * 0.5f, pos.y + hit.Size.y * 0.5f, 0),
                       new Vector3(pos.x + hit.Size.x * 0.5f, pos.y - hit.Size.y * 0.5f, 0), color);

        //Bottom
        Debug.DrawLine(new Vector3(pos.x + hit.Size.x * 0.5f, pos.y - hit.Size.y * 0.5f, 0),
                       new Vector3(pos.x - hit.Size.x * 0.5f, pos.y - hit.Size.y * 0.5f, 0), color);

        //Left
        Debug.DrawLine(new Vector3(pos.x - hit.Size.x * 0.5f, pos.y - hit.Size.y * 0.5f, 0),
                       new Vector3(pos.x - hit.Size.x * 0.5f, pos.y + hit.Size.y * 0.5f, 0), color);

        //Top
        Debug.DrawLine(new Vector3(pos.x - hit.Size.x * 0.5f, pos.y + hit.Size.y * 0.5f, 0),
                       new Vector3(pos.x + hit.Size.x * 0.5f, pos.y + hit.Size.y * 0.5f, 0), color);
    }

    public void DoEnter()
    {
        _activeHitboxes.Clear();
    }

    public void DoExit()
    {
        _activeHitboxes.Clear();
    }

    

    public Move()
    {
        /*
        // animation events genom kod
        Animator a = new Animator();
        Animation aa = new Animation();
        aa[""].time
            */
    }
}
[System.Serializable]
public class SerializedAnimationMap
{
    public AnimationMap[] animationMaps = new AnimationMap[0];

    public AnimationClip clip;
    public string animatorStateName;
    public int layerNumber;

    private int _totalFrames = 0;
    private int _animationFullNameHash;

    public int totalFrames { get { return _totalFrames; } }

    public void Initialize(Animator animator)
    {
        _totalFrames = Mathf.RoundToInt(clip.length * clip.frameRate);

        if (animator.isActiveAndEnabled)
        {
            string name = animator.GetLayerName(layerNumber) + "." + animatorStateName;
            _animationFullNameHash = Animator.StringToHash(name);
        }

    }

    public bool IsActive(Animator animator)
    {
        return animator.isPlayingOnLayer(_animationFullNameHash, 0);
    }

    double PercentageOnFrame(int frameNumber)
    {
        return (double)frameNumber / (double)_totalFrames;
    }

    public int CurrentFrameApproximation(Animator animator)
    {
        return (int)(animator.normalizedTime(layerNumber) * totalFrames);
    }

    public bool BiggerOrEqualThanFrame(Animator animator, int frameNumber)
    {
        double percentage = animator.normalizedTime(layerNumber);
        return (percentage >= PercentageOnFrame(frameNumber));
    }

    public bool OnLastFrame(Animator animator)
    {
        double percentage = animator.normalizedTime(layerNumber);
        return (percentage > PercentageOnFrame(_totalFrames - 1));
    }
}
[System.Serializable]
public class AnimationMap
{
    public int frame;
    public int boneID;
}


[System.Serializable]
public class Hit
{
    public int priority;
    public Transform follow;

    public int frameStart;
    public int frameEnd;

    [SerializeField] private bool _isMultiHit;
    public bool isMultiHit
    {
        get { return _isMultiHit; }
        set
        {
            _isMultiHit = value;
            CalculateMultiHitFrames();
        }
    }
    public int multiHitTimes;
    public int multiHitDelay;
    public int multiHitActiveFrames;

    [SerializeField] private int[] _multiHitFrameStart;
    [SerializeField] private int[] _multiHitFrameEnd;

    public int FrameStart { get { return frameStart; } }
    public int FrameEnd { get { return frameEnd; } }

    public int[] MultiFrameStart { get { return _multiHitFrameStart; } }
    public int[] MultiFrameEnd { get { return _multiHitFrameEnd; } }

    public void CalculateMultiHitFrames()
    {
        if (isMultiHit)
        {
            HashSet<Vector2Int> fillBounds = new HashSet<Vector2Int>();

            for (int time = 1; time <= multiHitTimes; time++)
            {
                Vector2Int vec = new Vector2Int();

                vec.x = frameStart + (time - 1) * multiHitDelay + (time - 1) * multiHitActiveFrames;
                vec.y = frameStart + time * multiHitActiveFrames - 1 + (time - 1) * multiHitDelay;

                fillBounds.Add(vec);
            }

            _multiHitFrameStart = new int[fillBounds.Count];
            _multiHitFrameEnd = new int[fillBounds.Count];

            Vector2Int[] arr = fillBounds.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                _multiHitFrameStart[i] = arr[i].x;
                _multiHitFrameEnd[i] = arr[i].y;
            }
        }
        else
        {
            _multiHitFrameStart = null;
            _multiHitFrameEnd = null;

            /*
            multiHitTimes = 0;
            multiHitDelay = 0;
            multiHitActiveFrames = 0;
            */
        }
    }

    public Rect bounds;
    public float damage;

    public bool hitboxEditorToggle;

    [HideInInspector] public Collider2D[] isHit;
    public Vector2 Pos { get { return new Vector2(bounds.x, bounds.y); } }
    public Vector2 Size { get { return new Vector2(bounds.width, bounds.height); } }
}
