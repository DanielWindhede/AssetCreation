// GENERATED AUTOMATICALLY FROM 'Assets/FPS/FPSInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @FPSInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @FPSInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""FPSInput"",
    ""maps"": [
        {
            ""name"": ""FPS"",
            ""id"": ""79fac9eb-99e7-41b0-8090-b77eb76fcbb3"",
            ""actions"": [
                {
                    ""name"": ""MouseDelta"",
                    ""type"": ""Value"",
                    ""id"": ""0a9292d1-5221-4581-a5be-5b33f240ab32"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveDirection"",
                    ""type"": ""Value"",
                    ""id"": ""e472f921-5c07-4398-9c61-7dd92a051eea"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Special"",
                    ""type"": ""Button"",
                    ""id"": ""366f3e5e-d928-49b2-b24d-280e40743182"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""fc85de96-e3a4-4194-8751-9e9e91281c7f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ee2275d7-5f2c-42df-b286-549e2a7794db"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Value"",
                    ""id"": ""ea4eeb49-1bca-4e66-9f10-165b385745ef"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveDirection"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1de9a20a-9490-4649-9421-ea137fa1bcd3"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""9c41d268-a65a-4644-a3cd-cc2b41b61fe5"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""510e2443-3405-476b-abd1-944b754dc04b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e64d5c1d-c45a-4de9-b101-cfebcb568d39"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveDirection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""11f06c53-ce70-4f0e-8f1f-02dcbffaf5c4"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Special"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc8fe245-5269-43cf-816b-3d8eae71cda1"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // FPS
        m_FPS = asset.FindActionMap("FPS", throwIfNotFound: true);
        m_FPS_MouseDelta = m_FPS.FindAction("MouseDelta", throwIfNotFound: true);
        m_FPS_MoveDirection = m_FPS.FindAction("MoveDirection", throwIfNotFound: true);
        m_FPS_Special = m_FPS.FindAction("Special", throwIfNotFound: true);
        m_FPS_Jump = m_FPS.FindAction("Jump", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // FPS
    private readonly InputActionMap m_FPS;
    private IFPSActions m_FPSActionsCallbackInterface;
    private readonly InputAction m_FPS_MouseDelta;
    private readonly InputAction m_FPS_MoveDirection;
    private readonly InputAction m_FPS_Special;
    private readonly InputAction m_FPS_Jump;
    public struct FPSActions
    {
        private @FPSInput m_Wrapper;
        public FPSActions(@FPSInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseDelta => m_Wrapper.m_FPS_MouseDelta;
        public InputAction @MoveDirection => m_Wrapper.m_FPS_MoveDirection;
        public InputAction @Special => m_Wrapper.m_FPS_Special;
        public InputAction @Jump => m_Wrapper.m_FPS_Jump;
        public InputActionMap Get() { return m_Wrapper.m_FPS; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FPSActions set) { return set.Get(); }
        public void SetCallbacks(IFPSActions instance)
        {
            if (m_Wrapper.m_FPSActionsCallbackInterface != null)
            {
                @MouseDelta.started -= m_Wrapper.m_FPSActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.performed -= m_Wrapper.m_FPSActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.canceled -= m_Wrapper.m_FPSActionsCallbackInterface.OnMouseDelta;
                @MoveDirection.started -= m_Wrapper.m_FPSActionsCallbackInterface.OnMoveDirection;
                @MoveDirection.performed -= m_Wrapper.m_FPSActionsCallbackInterface.OnMoveDirection;
                @MoveDirection.canceled -= m_Wrapper.m_FPSActionsCallbackInterface.OnMoveDirection;
                @Special.started -= m_Wrapper.m_FPSActionsCallbackInterface.OnSpecial;
                @Special.performed -= m_Wrapper.m_FPSActionsCallbackInterface.OnSpecial;
                @Special.canceled -= m_Wrapper.m_FPSActionsCallbackInterface.OnSpecial;
                @Jump.started -= m_Wrapper.m_FPSActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_FPSActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_FPSActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_FPSActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseDelta.started += instance.OnMouseDelta;
                @MouseDelta.performed += instance.OnMouseDelta;
                @MouseDelta.canceled += instance.OnMouseDelta;
                @MoveDirection.started += instance.OnMoveDirection;
                @MoveDirection.performed += instance.OnMoveDirection;
                @MoveDirection.canceled += instance.OnMoveDirection;
                @Special.started += instance.OnSpecial;
                @Special.performed += instance.OnSpecial;
                @Special.canceled += instance.OnSpecial;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }
        }
    }
    public FPSActions @FPS => new FPSActions(this);
    public interface IFPSActions
    {
        void OnMouseDelta(InputAction.CallbackContext context);
        void OnMoveDirection(InputAction.CallbackContext context);
        void OnSpecial(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
}
