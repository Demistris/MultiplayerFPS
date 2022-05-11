using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] _fpsHands;
    [SerializeField] private GameObject[] _soldier;
    [SerializeField] private GameObject _playerUIPrefab;
    [SerializeField] private PlayerMovementController _playerMovementController;
    [SerializeField] private RigidbodyFirstPersonController _rigidbodyController;
    [SerializeField] private Camera _camera;
    [SerializeField] private Animator _animator;
    [SerializeField] private Shooting _shooter;

    private void Start()
    {
        SoldierComponentsActivity();
    }

    private void SoldierComponentsActivity()
    {
        if (photonView.IsMine)
        {
            //Activate FPS Hands, Deactivate Soldier
            ObjectsActivity(_fpsHands, true);
            ObjectsActivity(_soldier, false);
            _animator.SetBool("IsSoldier", false);

            InstantiatePlayerUI();
            return;
        }

        //Activate Soldier, Deactivate FPS Hands
        ObjectsActivity(_fpsHands, false);
        ObjectsActivity(_soldier, true);
        _animator.SetBool("IsSoldier", true);

        _playerMovementController.enabled = false;
        _rigidbodyController.enabled = false;
        _camera.enabled = false;
    }

    private void ObjectsActivity(GameObject[] objectsArray, bool activity)
    {
        foreach (GameObject objects in objectsArray)
        {
            objects.SetActive(activity);
        }
    }

    private void InstantiatePlayerUI()
    {
        GameObject playerUI = Instantiate(_playerUIPrefab);
        _playerMovementController.Joystick = playerUI.GetComponentInChildren<Joystick>();
        _playerMovementController.FixedTouchField = playerUI.GetComponentInChildren<FixedTouchField>();
        playerUI.GetComponentInChildren<Button>().onClick.AddListener(() => _shooter.Fire());

        _camera.enabled = true;
    }
}
