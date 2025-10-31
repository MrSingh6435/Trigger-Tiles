using System;
using System.Collections;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private float _disableCollidersTime = 1f;
    private bool _playerOnPlatform;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            _playerOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            _playerOnPlatform = false;
        }
    }

    private void Update()
    {
        DetectPlayerInput();
    }

    private void DetectPlayerInput()
    {
        // if (!_playerOnPlatform) return; // i think this line has error please check it because if i se ti to be commented my player can fall down the platform with s key down

        if (PlayerController.Instance.MoveInput.y < 0f)
        {
            // Debug.Log("player hit s");
            StartCoroutine(DisablePlatformColliderRoutine());
        }
    }

    private IEnumerator DisablePlatformColliderRoutine()
    {
        Collider2D[] playerColliders = PlayerController.Instance.GetComponents<Collider2D>();

        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, true);
        }

        yield return new WaitForSeconds(_disableCollidersTime);

        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, false);
        }
    }
}
