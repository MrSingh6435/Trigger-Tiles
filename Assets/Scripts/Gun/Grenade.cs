using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public Action OnExplod;
    public Action OnBeep;

    [SerializeField] private GameObject _explodeVFX;
    [SerializeField] private GameObject _grenadeLight;
    [SerializeField] private float _launchForce = 15f;
    [SerializeField] private float _torqueAmount = 2f;
    [SerializeField] private float _explosionRadius = 3.5f;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private int _damageAmount = 3;
    [SerializeField] private float _lightBlinkTime = .15f;
    [SerializeField] private int _totalBlinks = 3;
    [SerializeField] private int _explodeTime = 3;

    private int _currentBlinks;
    private Rigidbody2D _rigidbody;
    private CinemachineImpulseSource _impulseSource;

    void OnEnable()
    {
        OnExplod += Explosion;
        OnExplod += GernadeScreenShake;
        OnExplod += DamageNearBy;
        OnExplod += AudioManager.Instance.Grenade_OnExplode;
        OnBeep += BlinkLight;
        OnBeep += AudioManager.Instance.Grenade_OnBeep;
    }

    void OnDisable()
    {
        OnExplod -= Explosion;
        OnExplod -= GernadeScreenShake;
        OnExplod -= DamageNearBy;
        OnExplod -= AudioManager.Instance.Grenade_OnExplode;
        OnBeep -= BlinkLight;
        OnBeep -= AudioManager.Instance.Grenade_OnBeep;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        LaunchGrenade();
        StartCoroutine(CountDownExplodeRoutine());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            OnExplod?.Invoke();
        }
    }

    private void LaunchGrenade()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diractionToMouse = (mousePos - (Vector2)transform.position).normalized;
        _rigidbody.AddForce(diractionToMouse * _launchForce, ForceMode2D.Impulse);
        _rigidbody.AddTorque(_torqueAmount, ForceMode2D.Impulse);
    }

    private void Explosion()
    {
        Instantiate(_explodeVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void GernadeScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }

    private void DamageNearBy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _enemyLayerMask);
        foreach (Collider2D hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            health?.TakeDamage(_damageAmount);
        }
    }

    private IEnumerator CountDownExplodeRoutine()
    {
        while (_currentBlinks < _totalBlinks)
        {
            yield return new WaitForSeconds(_explodeTime / _totalBlinks);
            OnBeep?.Invoke();

            yield return new WaitForSeconds(_lightBlinkTime);
            _grenadeLight.SetActive(false);
        }

        OnExplod?.Invoke();
    }

    private void BlinkLight()
    {
        _grenadeLight.SetActive(true);
        _currentBlinks++;
    }
}
