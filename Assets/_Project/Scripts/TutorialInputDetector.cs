using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace _Project.Scripts
{
    public class TutorialInputDetector : MonoBehaviour
    {
        [SerializeField]
        private PlayerInput input;
        [SerializeField]
        private GameObject KeyboardControls;
        [SerializeField]
        private GameObject XboxControls;
        [SerializeField]
        private GameObject PSControls;

        private void Start()
        {
            UpdateButtonImages(input.currentControlScheme);
        }

        void OnEnable() {
            InputUser.onChange += OnInputDeviceChange;
        }
 
        void OnDisable() {
            InputUser.onChange -= OnInputDeviceChange;
        }
    
        private void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice device) {
            if (change == InputUserChange.ControlSchemeChanged) {
                Debug.Log($"Changed control scheme: {user.controlScheme.Value.name}");
                UpdateButtonImages(user.controlScheme.Value.name);
            }
        }
    
        private void UpdateButtonImages(string schemeName)
        {
            switch (schemeName)
            {
                case "Xbox Controller":
                    XboxControls.SetActive(true);
                    PSControls.SetActive(false);
                    KeyboardControls.SetActive(false);
                    break;
                case "PS Controller":
                    XboxControls.SetActive(false);
                    PSControls.SetActive(true);
                    KeyboardControls.SetActive(false);
                    break;
                default:
                    XboxControls.SetActive(false);
                    PSControls.SetActive(false);
                    KeyboardControls.SetActive(true);
                    break;
            }
        }
    }
}
