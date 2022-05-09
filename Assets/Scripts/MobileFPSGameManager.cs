using UnityEngine;
using Photon.Pun;

public class MobileFPSGameManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    private void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        if(_playerPrefab != null)
        {
            int randomSpawnPoint = Random.Range(-10, 10);
            PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(randomSpawnPoint, 0f, randomSpawnPoint), Quaternion.identity);
        }
    }
}
