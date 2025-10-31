using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : MonoBehaviour
{
    public static Action OnShoot;
    public static Action OnGrenadeShoot;
    [SerializeField] private Transform _bulletSpawnPoint;
    
    [Header("Bullet")]
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = .5f;
    [SerializeField] private GameObject _muzzelFlash;
    [SerializeField] private float _muzzelFlashTime = .05f;

    [Header("Grenade")]
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private float _grenadeShootCD = .8f;

    private Coroutine _muzzelFlashRoutine;
    private ObjectPool<Bullet> _bulletPool;
    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private Vector2 _mousePos;
    private float _lastFireTime = 0f;
    private float _lastGrenadeTime = 0f;

    private PlayerInput _playerInput;
    private PlayerInput.FrameInput _frameInput;
    private Animator _animator;
    private CinemachineImpulseSource _impulseSource;

    void Start()
    {
        CreateBulletPool();
    }
    void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponentInParent<PlayerInput>();
        _frameInput = _playerInput.FrameInputs;
    }

    private void Update()
    {
        GeatherInput();
        Shoot();
        RotateGun();
    }

    void OnEnable()
    {
        OnShoot += ShootProjectile;
        OnShoot += ResetLastFireTime;
        OnShoot += FireAnimation;
        OnShoot += GunScreenShake;
        OnShoot += MuzzelFlash;
        OnGrenadeShoot += ShootGrenade;
        OnGrenadeShoot += FireAnimation;
        OnGrenadeShoot += ResetLastGrenadeShootTime;
    }

    void OnDisable()
    {
        OnShoot -= ShootProjectile;
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        OnShoot -= GunScreenShake;
        OnShoot -= MuzzelFlash;
        OnGrenadeShoot -= ShootGrenade;
        OnGrenadeShoot -= FireAnimation;
        OnGrenadeShoot -= ResetLastGrenadeShootTime;
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    private void GeatherInput()
    {
        _frameInput = _playerInput.FrameInputs;
    }

    void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() =>
        {
            return Instantiate(_bulletPrefab);
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet);
        }, false, 20, 40);
    }

    private void Shoot()
    {
        if (Input.GetMouseButton(0) && Time.time >= _lastFireTime)
        {
            OnShoot?.Invoke();
        }

        if (_frameInput.Grenade && Time.time >= _lastGrenadeTime)
        {
            OnGrenadeShoot?.Invoke();
        }
    }

    private void ShootProjectile()
    {
        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, _mousePos);
    }

    private void ShootGrenade()
    {
        Instantiate(_grenadePrefab, _bulletSpawnPoint.position, Quaternion.identity);
        _lastGrenadeTime = Time.time;
    }

    void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f);
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD;
    }

    private void ResetLastGrenadeShootTime()
    {
        _lastGrenadeTime = Time.time + _grenadeShootCD;
    }

    void GunScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }

    private void RotateGun()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2 direction = _mousePos - (Vector2)PlayerController.Instance.transform.position;
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(_mousePos);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void MuzzelFlash()
    {
        if (_muzzelFlashRoutine != null)
        {
            StopCoroutine(_muzzelFlashRoutine);
        }

        _muzzelFlashRoutine = StartCoroutine(MuzzelFlashRoutine());
    }

    private IEnumerator MuzzelFlashRoutine()
    {
        _muzzelFlash.SetActive(true);
        yield return new WaitForSeconds(_muzzelFlashTime);
        _muzzelFlash.SetActive(false);
    }
}
