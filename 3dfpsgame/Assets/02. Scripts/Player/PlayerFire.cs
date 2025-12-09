using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private float ThrowPower = 15f;
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Bomb bomb = Instantiate(_bombPrefab, _fireTransform.position, Quaternion.identity);
            Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();

            rigidbody.AddForce(_cameraTransform.forward * ThrowPower, ForceMode.VelocityChange);
        }
    }
}
