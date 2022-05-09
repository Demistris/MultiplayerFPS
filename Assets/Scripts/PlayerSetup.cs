using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _fpsHands;
    [SerializeField] private GameObject _soldier;

    private void Start()
    {
        SoldierBodyAcitvity();
    }

    private void SoldierBodyAcitvity()
    {
        if (photonView.IsMine)
        {
            //Activate FPS Hands, Deactivate Soldier
            _fpsHands.SetActive(true);
            _soldier.SetActive(false);
            return;
        }

        //Activate Soldier, Deactivate FPS Hands
        _fpsHands.SetActive(false);
        _soldier.SetActive(true);
    }
}
