using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.FPS
{
    [RequireComponent(typeof(FPSController))]
    public class FPSPlayer : MonoBehaviour
    {
        [SerializeField] private GameObject _knifePrefab;
        [SerializeField] private float _speed;
        private GameObject _knife;
        private FPSInput _input;
        private FPSController _controller;

        // Start is called before the first frame update
        void Awake()
        {
            _controller = GetComponent<FPSController>();

            _input = new FPSInput();
            _input.FPS.Jump.performed += x => DoJump();
            _input.FPS.Special.performed += x => DoKnifeHandling();
        }

        // Update is called once per frame
        void Update()
        {
            _controller.DoUpdate();
            //DoKnifeHandling();
        }

        private bool _hasThrown;
        private void DoKnifeHandling()
        {
            if (!_hasThrown)
            {
                _hasThrown = true;
                _knife = Instantiate(_knifePrefab);
                _knife.transform.position = transform.position;
                _knife.GetComponent<Knife>().InitialVelocity = Camera.main.transform.rotation.eulerAngles * _speed;
            }
            else
            {
                _controller.SetPosition(_knife.transform.position);
                Destroy(_knife);
                _hasThrown = false;
            }
        }

        private void DoJump()
        {
            _controller.SetVelocity(-_controller.GravityDirection * 10);
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }
    }
}