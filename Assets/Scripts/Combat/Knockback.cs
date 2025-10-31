using UnityEngine;
using System;
using System.Collections;

public class Knockback : MonoBehaviour
{
    public Action OnKnockbackStart;
    public Action OnKnockbackEnd;

    [SerializeField] float _knokbackTime = 0.2f;

    private Vector3 _hitDirection;
    private float _knockbackThrust;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        OnKnockbackStart += ApplyKnockbackForce;
        OnKnockbackEnd += StopKnockRoutine;
    }

    void OnDisable()
    {
        OnKnockbackStart -= ApplyKnockbackForce;
        OnKnockbackEnd -= StopKnockRoutine;
    }

    /// <summary>
    /// Call this to apply knockback force.
    /// 'hitDirection' should be the direction from attacker to this object.
    /// </summary>
    public void GetKnockedBack(Vector3 hitDirection, float knockbackThrust)
    {
        _hitDirection = hitDirection.normalized;  // FIX: Normalize here directly
        _knockbackThrust = knockbackThrust;

        OnKnockbackStart?.Invoke();
    }

    private void ApplyKnockbackForce()
    {
        // ✅ FIXED KNOCKBACK DIRECTION
        _rigidbody.AddForce(_hitDirection * _knockbackThrust, ForceMode2D.Impulse);

        StartCoroutine(KnockbackRoutine());

        /*
        ❌ OLD LOGIC (commented out):
        Vector3 direction = (transform.position - _hitDirection).normalized;
        _rigidbody.AddForce(direction * _knockbackThrust, ForceMode2D.Impulse);
        */
    }

    private IEnumerator KnockbackRoutine()
    {
        yield return new WaitForSeconds(_knokbackTime);
        OnKnockbackEnd?.Invoke();
    }

    private void StopKnockRoutine()
    {
        _rigidbody.linearVelocity = Vector2.zero;
    }
}
