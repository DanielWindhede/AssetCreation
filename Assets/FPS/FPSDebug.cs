using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fami.FPS
{

    public class FPSDebug : MonoBehaviour
    {
        [SerializeField] private bool _showCursor;
        public bool ShowCursor
        {
            get { return _showCursor; }
            set
            {
                _showCursor = value;
                Cursor.visible = value;

                if (!value)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
            }
        }

        private static FPSDebug _instance = null;
        public static FPSDebug Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(FPSDebug)) as FPSDebug;
                }

                if (_instance == null)
                {
                    var obj = new GameObject("FPSDebug");
                    _instance = obj.AddComponent<FPSDebug>();
                }

                return _instance;
            }
        }

        private void Start()
        {
            ShowCursor = _showCursor;
        }

        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}