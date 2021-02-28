using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementInputType
{
    UpBack = 2,     Up = 3,     UpForward = 4,
    Back = -1,      Idle = 0,   Forward = 1,
    DownBack = -4,  Down = -3,  DownForward = -2,
}
public enum SpecialInputType
{
    Idle = 1, Attack, Special, Jump, Dash,
}

public class InputInfoSequence
{
    public InputInfo[] sequence;
    public InputInfoSequence(params InputInfo[] sequence)
    {
        this.sequence = sequence;
    }


    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(InputInfoSequence))
        {
            InputInfoSequence o = (InputInfoSequence)obj;

            if (sequence.Length != o.sequence.Length)
                return false;

            for (int i = 0; i < o.sequence.Length; i++)
            {
                if (!o.sequence[i].Equals(sequence[i]))
                    return false;
            }

            return true;
        }
        else if (obj.GetType() == typeof(InputInfo))
        {
            if (sequence.Length == 1)
                return sequence[0].Equals((InputInfo)obj);
        }

        return false;
    }

    public override int GetHashCode()
    {
        int val = 0;
        foreach (InputInfo i in sequence)
        {
            val += i.GetHashCode();
        }
        return val;
    }
}

public struct InputInfo
{
    //public bool holdable;
    public MovementInputType direction;
    public HashSet<SpecialInputType> special;

    public InputInfo(MovementInputType direction, params SpecialInputType[] special)
    {
        this.direction = direction;
        this.special = new HashSet<SpecialInputType>(special);
        /*
        List<SpecialInputType> specials = new List<SpecialInputType>(special);
        specials.Sort((val1, val2) => ((int)val1).CompareTo((int)val2));
        this.special = specials.ToArray();
        */
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(InputInfo))
            return false;

        InputInfo o = (InputInfo)obj;

        return this.direction == o.direction && this.special.SetEquals(o.special);
    }

    public override int GetHashCode()
    {
        int val = (int)direction;
        foreach (SpecialInputType item in special)
        {
            val += (int)item * 100; // 2 potens istället??
        }
        return val; //base.GetHashCode()
    }
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

        _controls.Enable();
        //_controls.Actions.Attack.performed += _ => pressed = _.ReadValue<float>();
    }



    public void ChangeBufferCapacity(int bufferCapacity)
    {
        buffer = new LimitedQueue<InputInfo>(bufferCapacity);
    }

    // Update is called once per frame
    public void DoFixedUpdate()
    {
        DoInputPolling();
    }

    private void DoInputPolling()
    {
        // Special inputs
        List<SpecialInputType> specialInput = new List<SpecialInputType>();

        if (_controls.Actions.Attack.ReadValue<float>() > .1f) //ändra till att enbart hantera för en frame
            specialInput.Add(SpecialInputType.Attack);

        if (_controls.Actions.Jump.ReadValue<float>() > .1f)
            specialInput.Add(SpecialInputType.Jump);

        if (_controls.Actions.Special.ReadValue<float>() > .1f)
            specialInput.Add(SpecialInputType.Special);

        if (specialInput.Count == 0)
            specialInput.Add(SpecialInputType.Idle);

        // Movement inputs
        var m = _controls.Actions.Move.ReadValue<Vector2>();
        Vector2Int moveVec = new Vector2Int(Mathf.RoundToInt(m.x), Mathf.RoundToInt(m.y));

        // By multiplying the y-axis with a factor of three and retrieving the vector length you will recieve unique values ranging from -4 to 4
        moveVec *= new Vector2Int(1, 3);
        int sum = moveVec.x + moveVec.y;

        MovementInputType movementInput = (MovementInputType)sum;

        // Adding to buffer
        buffer.Enqueue(new InputInfo(movementInput, specialInput.ToArray()));
    }

    public InputInfo GetLatestLegalInput(InputInfo legalMove)
    {
        return GetLatestLegalInput(new InputInfo[] { legalMove });
    }
    public InputInfo GetLatestLegalInput(InputInfo[] legalMoves)
    {
        InputInfo[] arr = buffer.ToArray();
        for (int i = 0; i < arr.Length; i++)
        {
            if (System.Array.Exists<InputInfo>(legalMoves, x => x.Equals(arr)))
                return arr[i];
        }

        return new InputInfo();
    }

    public bool IsInputLatest(InputInfo input)
    {
        return buffer.Peek().Equals(input);
    }
}