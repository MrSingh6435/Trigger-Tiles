using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool CanMove => _canMove;
    [SerializeField] private float _moveSpeed = 10f;

    private float moveX;
    private bool _canMove = true;
    private Rigidbody2D _rigidBody;
    private Knockback _knockback;
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _knockback = GetComponent<Knockback>();
    }

    void OnEnable()
    {
        _knockback.OnKnockbackStart += CanMoveFalse;
        _knockback.OnKnockbackEnd += CanMoveTure;
    }

    void OnDisable()
    {
        _knockback.OnKnockbackStart -= CanMoveFalse;
        _knockback.OnKnockbackEnd -= CanMoveTure; 
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void SetCurrentDirection(float currentDirection)
    {
        moveX = currentDirection;
    }

    private void CanMoveTure()
    {
        _canMove = true;
    }

    private void CanMoveFalse()
    {
        _canMove = false;
    }

    private void Move()
    {
        if(!_canMove) { return; }
        Vector2 movement = new Vector2(moveX * _moveSpeed, _rigidBody.linearVelocity.y);
        _rigidBody.linearVelocity = movement;
    }
}
