using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    [HideInInspector]
    public Joystick Joystick;
    [HideInInspector]
    public FixedTouchField FixedTouchField;

    [SerializeField] private RigidbodyFirstPersonController _rigidbodyController;
    [SerializeField] private Animator _animator;

    private void FixedUpdate()
    {
        if (Joystick != null)
        {
            SetInputs();
            SetAnimations();
            Running();
        }
    }

    private void SetInputs()
    {
        _rigidbodyController.JoystickInputAxis.x = Joystick.Horizontal;
        _rigidbodyController.JoystickInputAxis.y = Joystick.Vertical;

        _rigidbodyController.mouseLook.LookInputAxis = FixedTouchField.TouchDist;
    }

    private void SetAnimations()
    {
        _animator.SetFloat("Horizontal", Joystick.Horizontal);
        _animator.SetFloat("Vertical", Joystick.Vertical);
    }

    private void Running()
    {
        if(Mathf.Abs(Joystick.Horizontal) > 0.9f || Mathf.Abs(Joystick.Vertical) > 0.9f)
        {
            _rigidbodyController.movementSettings.ForwardSpeed = 16;
            _animator.SetBool("IsRunning", true);
            return;
        }

        _rigidbodyController.movementSettings.ForwardSpeed = 8;
        _animator.SetBool("IsRunning", false);
    }
}
