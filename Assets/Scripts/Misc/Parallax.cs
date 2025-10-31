using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float _parallaxOfSet = -0.1f;
    private Vector2 _startPos;
    private Vector2 _travel => (Vector2)_mainCamera.transform.position - _startPos;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _startPos = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = _startPos + new Vector2(_travel.x * _parallaxOfSet, 0f);
        transform.position = new Vector2(newPosition.x, transform.position.y);
    }
}
