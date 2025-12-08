using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * _speed * Time.deltaTime;

        Vector3 direction = new Vector3(moveX, 0, moveZ);
        direction.Normalize();

        direction = Camera.main.transform.TransformDirection(direction);

        transform.position += direction * _speed * Time.deltaTime;
    }
}
