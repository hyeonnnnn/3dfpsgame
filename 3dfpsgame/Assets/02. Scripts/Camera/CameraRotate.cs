using UnityEngine;
using UnityEngine.UIElements;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 200f;

    private float _accumulationX = 0f;
    private float _accumulationY = 0f;

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
        _accumulationY += mouseY * _rotationSpeed * Time.deltaTime;

        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);

        transform.eulerAngles = new Vector3(-_accumulationY, _accumulationX);
    }
}
