﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.FightingGame
{
    [RequireComponent(typeof(Animator))]
    public class Moveset : MonoBehaviour, IFrameCheckHandler
    {
        [SerializeField] private List<Move> _moveFill;
        private Dictionary<string, Move> moves;
        private Animator _animator;

        private string _currentState;
        public string GetCurrentState { get { return _currentState; } }

        public void ChangeState(string stateName)
        {
            if (_currentState == stateName)
                return;

            if (_currentState != null &&
                _currentState != string.Empty)
                moves[_currentState].DoExit();

            _currentState = stateName;
            _animator.Play(_currentState);
            moves[_currentState].DoEnter();

            GetComponent<HitboxDrawer>().move = moves[_currentState]; // TODO
        }

        public int currentFrame;



        public void Initialize()
        {
            _animator = GetComponent<Animator>();
            moves = new Dictionary<string, Move>();

            if (_moveFill == null)
                _moveFill = new List<Move>();

            GetComponent<HitboxDrawer>().inEditorDraw = false; // TODO

            foreach (Move m in _moveFill)
            {
                Move instance = Instantiate(m);
                instance.currentFrame = 0;
                moves.Add(instance.animationStateName, instance);
                moves[instance.animationStateName].Initialize(_animator, this);
            }
            ChangeState("idle"); // TODO
        }

        public void DoFixedUpdate()
        {
            moves[_currentState].DoFixedUpdate();
        }

        public void IMPLEMENTCOLLISIONAHANDLING(Hit hitbox)
        {

        }

        public void HandleCollision(Hit hit, Collision2D collision2D)
        {

        }

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
    }
}