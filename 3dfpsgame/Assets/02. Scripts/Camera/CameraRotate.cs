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
        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 마우스 입력 누적하기
        _accumulationX += mouseX * _rotationSpeed * Time.deltaTime;
        _accumulationY += mouseY * _rotationSpeed * Time.deltaTime;

        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);

        // 누적한 방향으로 카메라 회전하기
        transform.eulerAngles = new Vector3(-_accumulationY, _accumulationX);
    }
}
