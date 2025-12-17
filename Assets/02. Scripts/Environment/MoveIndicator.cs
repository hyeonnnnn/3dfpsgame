using UnityEngine;

public class MoveIndicator : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _deactivateRadius = 1.5f;

    private void Update()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.position);
        if (distance <= _deactivateRadius)
        {
            Hide();
        }
    }

    public void Show(Vector3 clickPosition)
    {
        if (_player == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _player = player.transform;
        }

        transform.position = clickPosition;
        SetDirection(clickPosition);
        gameObject.SetActive(true);
    }

    private void SetDirection(Vector3 clickPosition)
    {
        if (_player == null) return;

        Vector3 direction = _player.position - clickPosition;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
