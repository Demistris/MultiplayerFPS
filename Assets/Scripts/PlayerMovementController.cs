using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;
    [SerializeField] private RigidbodyFirstPersonController _rigidbodyController;
    [SerializeField] private FixedTouchField _fixedTouchField;

    private void FixedUpdate()
    {
        _rigidbodyController.JoystickInputAxis.x = _joystick.Horizontal;
        _rigidbodyController.JoystickInputAxis.y = _joystick.Vertical;

        _rigidbodyController.mouseLook.LookInputAxis = _fixedTouchField.TouchDist;
    }
}
