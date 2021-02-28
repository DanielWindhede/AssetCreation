using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.FightingGame
{
    [RequireComponent(typeof(Animator), typeof(Moveset))]
    public class Character : MonoBehaviour
    {
        private Moveset moveset;
        private Animator animator;
        private InputManager inputManager;

        void Start()
        {
            Application.targetFrameRate = 60;
            animator = GetComponent<Animator>();
            moveset = GetComponent<Moveset>();
            inputManager = new InputManager(10);

            moveset.Initialize();
            //moveset = new Moveset(animator);
            //moveset.Initialize();




            availableStates = new Dictionary<int, StateTest>();
            //availableStates.Add(new StateTest(0, new InputInfo((MovementInputType)Random.Range(0, 1), (SpecialInputType)Random.Range(1, 2)), i - 1));
            //StateTest t = new StateTest(0, new InputInfo((MovementInputType)Random.Range(0, 1), (SpecialInputType)Random.Range(1, 2)), 1);
            StateTest t = new StateTest(0, 1, new InputInfo(MovementInputType.Down, SpecialInputType.Idle),
                                                new InputInfo(MovementInputType.DownForward, SpecialInputType.Idle),
                                                new InputInfo(MovementInputType.Forward, SpecialInputType.Attack));

            //t.inputToId.Add(new InputInfoSequence(new InputInfo(MovementInputType.Forward, SpecialInputType.Attack)), 2);
            availableStates.Add(0, t);

            availableStates.Add(1, new StateTest(1, 0, new InputInfo(MovementInputType.Idle, SpecialInputType.Special)));
            //availableStates.Add(2, new StateTest(2, 0, new InputInfo(MovementInputType.Back, SpecialInputType.Attack)));

            currentState = availableStates[0];
        }

        StateTest currentState;
        Dictionary<int, StateTest> availableStates;

        private class StateTest
        {
            public int id;
            public Dictionary<InputInfoSequence, int> inputToId = new Dictionary<InputInfoSequence, int>();
            public StateTest(int id, int goToState, params InputInfo[] input)
            {
                this.id = id;
                inputToId.Add(new InputInfoSequence(input), goToState);
            }

            public int GetIdFromInput(LimitedQueue<InputInfo> buffer)
            {
                return -1;
                //return GetIdFromInput(new InputInfoSequence(buffer));
            }

            public int GetIdFromInput(InputInfoSequence sequence)
            {
                return inputToId.ContainsKey(sequence) ? inputToId[sequence] : -1;
            }

            public InputInfoSequence IsBufferValid(LimitedQueue<InputInfo> buffer)
            {
                HashSet<InputInfo[]> validSequences = new HashSet<InputInfo[]>();
                /*
                foreach (var i in inputToId.Keys)
                {
                    InputInfo[] s = i.sequence;
                    System.Array.Reverse(s);
                    validSequences.Add(s);
                }

                InputInfo[] b = buffer.ToArray();
                int index = 0;

                for (int i = 0; i < b.Length; i++) // implement recursive/looping tree structure support for multiple valid inputs of same type
                {
                    if (s[index].Equals(b[i]))
                        index++;
                    else if (s.Length - index > b.Length - i)
                        return null;
                }

                if (index == s.Length - 1)
                    return true;
                */



                /*
                 * 
                InputInfo[] s = sequence;
                System.Array.Reverse(s);

                InputInfo[] b = buffer.ToArray();
                int index = 0;

                for (int i = 0; i < b.Length; i++) // implement recursive/looping tree structure support for multiple valid inputs of same type
                {
                    if (s[index].Equals(b[i]))
                        index++;
                    else if (s.Length - index > b.Length - i)
                        return null;
                }

                if (index == s.Length - 1)
                    return true;
                 */

                return null;
            }

            /*
            public int GetIdFromInputSequence(InputInfoSequence sequence)
            {
                return inputToId.ContainsKey(input) ? inputToId[input] : -1;
            }
            */
        }


        // Update is called once per frame
        void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                moveset.ChangeState("idle");
            if (Input.GetKeyDown(KeyCode.W))
                moveset.ChangeState("attack");

            inputManager.DoFixedUpdate();

            /*
            if (currentState.GetIdFromInput(inputManager.buffer.Peek()) >= 0)
            {
                currentState = availableStates[currentState.GetIdFromInput(inputManager.buffer.Peek())];
                //currentState = availableStates[currentState.GetIdFromInput(inputManager.buffer.Peek())];
            }
            */

            Debug.Log(currentState.id);

            moveset.DoFixedUpdate();
        }

        void OnGUI()
        {
            List<string> content = new List<string>();
            int longestLength = 0;

            // Automatic Layout
            foreach (var item in inputManager.buffer)
            {
                string finalString = item.direction.ToString();
                foreach (var special in item.special)
                {
                    finalString += ", " + special.ToString();
                }
                content.Add(finalString);
                if (longestLength < finalString.Length)
                    longestLength = finalString.Length;
            }

            GUI.Box(new Rect(0, 0, longestLength * 7, 25 * content.Count), "");

            foreach (var item in content)
            {
                GUILayout.Label(item);
            }
        }
    }
}