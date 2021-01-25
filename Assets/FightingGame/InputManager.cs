using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementInputType
{
    Idle, Up, Right, Down, Left,
}
public enum SpecialInputType
{
    Idle, Attack, Special, Jump, Dash,
}


public struct InputInfo
{
    public bool holdable;
    public MovementInputType direction;
    public SpecialInputType special;
}

public class LimitedQueue<T> : Queue<T>
{
    private int _capacity;

    public LimitedQueue()
    {
        _capacity = int.MaxValue;
    }

    public LimitedQueue(int capacity) : base(capacity)
    {
        _capacity = capacity;
    }

    public new void Enqueue(T item)
    {
        while (Count >= _capacity)
        {
            Dequeue();
        }
        base.Enqueue(item);
    }
}


public class InputManager
{
    private FightingGameInputActions _controls;

    public LimitedQueue<InputInfo> buffer;

    public InputManager(int bufferCapacity)
    {
        ChangeBufferCapacity(bufferCapacity);
        _controls = new FightingGameInputActions();
    }

    public void ChangeBufferCapacity(int bufferCapacity)
    {
        buffer = new LimitedQueue<InputInfo>(bufferCapacity);
    }

    // Update is called once per frame
    public void DoFixedUpdate()
    {
        Debug.Log(_controls.Actions.Attack.triggered);
    }
}
