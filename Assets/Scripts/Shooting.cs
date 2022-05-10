using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _hitEffectPrefab;

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if(Physics.Raycast(ray, out hit, 100f))
        {
            //Debug.Log("Hit " + hit.collider.gameObject.name);
            GameObject hitEffect = Instantiate(_hitEffectPrefab, hit.point, Quaternion.identity);
            Destroy(hitEffect, 0.5f);
        }
    }
}
