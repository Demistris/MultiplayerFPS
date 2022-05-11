using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class Shooting : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerSetup _playerSetup;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerMovementController _playerMovementController;

    [Header("Health")]
    [SerializeField] private float _startHealth = 100f;
    [SerializeField] private Image _healthBar;

    private float _heatlh;

    private void Start()
    {
        RegainHealth();
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if(Physics.Raycast(ray, out hit, 100f))
        {
            photonView.RPC("CreateHitEffect", RpcTarget.All, hit.point);

            GameObject hitObject = hit.collider.gameObject;
            PhotonView hitPhotonView = hitObject.GetComponent<PhotonView>();

            if (hitObject.CompareTag("Player") && !hitPhotonView.IsMine)
            {
                hitPhotonView.RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
            }
        }
    }

    [PunRPC]
    private void TakeDamage(float damage, PhotonMessageInfo info)
    {
        _heatlh -= damage;
        _healthBar.fillAmount = _heatlh / _startHealth;
        Debug.Log(_heatlh);

        if(_heatlh <= 0f)
        {
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    private void CreateHitEffect(Vector3 position)
    {
        GameObject hitEffect = Instantiate(_hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffect, 0.5f);
    }

    private void Die()
    {
        if(photonView.IsMine)
        {
            _animator.SetBool("IsDead", true);
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        Text respawnText = _playerSetup.PlayerUI.RespawnText;

        float respawnTime = 8f;

        while(respawnTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            respawnTime -= 1f;

            _playerMovementController.enabled = false;
            respawnText.text = "You are killed! Respawning at: " + respawnTime.ToString(".00");
        }

        _animator.SetBool("IsDead", false);
        respawnText.text = "";

        int randomPosition = Random.Range(-20, 20);
        transform.position = new Vector3(randomPosition, 0f, randomPosition);
        _playerMovementController.enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RegainHealth()
    {
        _heatlh = _startHealth;
        _healthBar.fillAmount = _heatlh / _startHealth;
    }
}
