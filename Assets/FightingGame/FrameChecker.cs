using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class AnimatorExtension
{
    public static bool isPlayingOnLayer(this Animator animator, int fullPathHash, int layer)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).fullPathHash == fullPathHash;

    }

    public static double normalizedTime(this Animator animator, System.Int32 layer)
    {
        double time = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        return time > 1 ? 1 : time;
    }
    public static double normalizedTimeLooping(this Animator animator, System.Int32 layer)
    {
        double time = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        return time > 1 ? time - Math.Truncate(time) : time;
    }

}

[System.Serializable]
public class AnimationClipExtended
{
    public Animator animator;
    public AnimationClip clip;
    public string animatorStateName;
    public int layerNumber;

    private int _totalFrames = 0;
    private int _animationFullNameHash;

    public int totalFrames { get { return _totalFrames; } }

    public void initialize()
    {
        _totalFrames = Mathf.RoundToInt(clip.length * clip.frameRate);

        if (animator.isActiveAndEnabled)
        {
            string name = animator.GetLayerName(layerNumber) + "." + animatorStateName;
            _animationFullNameHash = Animator.StringToHash(name);
        }

    }

    public bool isActive()
    {
        return animator.isPlayingOnLayer(_animationFullNameHash, 0);
    }
    double percentageOnFrame(int frameNumber)
    {
        return (double)frameNumber / (double)_totalFrames;
    }

    public int currentFrameApproximation
    {
        get
        {
            return (int)(animator.normalizedTime(layerNumber) * totalFrames);
        }
    }

    public bool biggerOrEqualThanFrame(int frameNumber)
    {
        double percentage = animator.normalizedTime(layerNumber);
        return (percentage >= percentageOnFrame(frameNumber));
    }
    public bool itsOnLastFrame()
    {
        double percentage = animator.normalizedTime(layerNumber);
        return (percentage > percentageOnFrame(_totalFrames - 1));
    }
}

public interface IFrameCheckHandler
{
    void onHitFrameStart();
    void onHitFrameEnd();
    void onLastFrameStart();
    void onLastFrameEnd();
}

[System.Serializable]
public class FrameChecker
{
    public int hitFrameStart;
    public int hitFrameEnd;
    public int totalFrames;

    private IFrameCheckHandler _frameCheckHandler;
    private AnimationClipExtended _extendedClip;
    private bool _checkedHitFrameStart;
    private bool _checkedHitFrameEnd;
    private bool _lastFrame;

    public int CurrentFrame
    {
        get { return _extendedClip.currentFrameApproximation; }
    }

    public void initialize(IFrameCheckHandler frameCheckHandler, AnimationClipExtended extendedClip)
    {
        _frameCheckHandler = frameCheckHandler;
        _extendedClip = extendedClip;
        totalFrames = extendedClip.totalFrames;
        initCheck();
    }

    public void initCheck()
    {
        _checkedHitFrameStart = false;
        _checkedHitFrameEnd = false;
        _lastFrame = false;
    }

    public void checkFrames()
    {
        if (_lastFrame)
        {
            _lastFrame = false;
            _frameCheckHandler.onLastFrameEnd();
        }

        if (!_extendedClip.isActive())
        { return; }

        if (!_checkedHitFrameStart && _extendedClip.biggerOrEqualThanFrame(hitFrameStart))
        {
            _frameCheckHandler.onHitFrameStart();
            _checkedHitFrameStart = true;
        }
        else if (!_checkedHitFrameEnd && _extendedClip.biggerOrEqualThanFrame(hitFrameEnd))
        {
            _frameCheckHandler.onHitFrameEnd();
            _checkedHitFrameEnd = true;
        }
        if (!_lastFrame && _extendedClip.itsOnLastFrame())
        {
            _frameCheckHandler.onLastFrameStart();
            _lastFrame = true; // This is here so we don't skip the last frame
        }
    }

}


[System.Serializable]
public class FrameCheckerMulti
{
    private Dictionary<string, Move> _extendedClips;
    private IFrameCheckHandler _frameCheckHandler;
    private Animator _animator;
    private bool _checkedHitFrameStart;
    private bool _checkedHitFrameEnd;
    private bool _lastFrame;

    public Move CurrentClip(string currentState)
    {
        return _extendedClips[currentState];
    }

    public void Initialize(IFrameCheckHandler frameCheckHandler, Dictionary<string, Move> extendedClips, Animator animator)
    {
        _frameCheckHandler = frameCheckHandler;
        _extendedClips = extendedClips;
        this._animator = animator;
        Reset();
    }

    public void Reset()
    {
        _checkedHitFrameStart = false;
        _checkedHitFrameEnd = false;
        _lastFrame = false;
    }
    
    public int CurrentFrame(string currentState)
    {
        return CurrentFrameApproximation(_animator, CurrentClip(currentState));
    }

    /*
    public void CheckFrames(string currentState)
    {
        if (_lastFrame)
        {
            _lastFrame = false;
            _frameCheckHandler.onLastFrameEnd();
        }

        if (!IsActive(_animator, CurrentClip(currentState)))
        { return; }

        if (!_checkedHitFrameStart && BiggerOrEqualThanFrame(_animator, CurrentClip(currentState).hitFrameStart, CurrentClip(currentState)))
        {
            _frameCheckHandler.onHitFrameStart();
            _checkedHitFrameStart = true;
        }
        else if (!_checkedHitFrameEnd && BiggerOrEqualThanFrame(_animator, CurrentClip(currentState).hitFrameEnd, CurrentClip(currentState)))
        {
            _frameCheckHandler.onHitFrameEnd();
            _checkedHitFrameEnd = true;
        }
        if (!_lastFrame && OnLastFrame(_animator, CurrentClip(currentState)))
        {
            _frameCheckHandler.onLastFrameStart();
            _lastFrame = true; // This is here so we don't skip the last frame
        }
    }
    */

    public static bool IsActive(Animator animator, Move currentState)
    {
        return animator.isPlayingOnLayer(currentState._animationFullNameHash, 0);
    }

    public static double PercentageOnFrame(int frameNumber, Move currentState)
    {
        return (double)frameNumber / (double)currentState.TotalFrames;
    }

    public static int CurrentFrameApproximation(Animator animator, Move currentState)
    {
        return (int)(animator.normalizedTime(currentState.layerNumber) * currentState.TotalFrames);
    }

    public static bool BiggerOrEqualThanFrame(Animator animator, int frameNumber, Move currentState)
    {
        double percentage = animator.normalizedTime(currentState.layerNumber);
        return (percentage >= PercentageOnFrame(frameNumber, currentState));
    }
    public static bool OnLastFrame(Animator animator, Move currentState)
    {
        double percentage = animator.normalizedTime(currentState.layerNumber);
        return (percentage > PercentageOnFrame(currentState.TotalFrames - 1, currentState));
    }
}