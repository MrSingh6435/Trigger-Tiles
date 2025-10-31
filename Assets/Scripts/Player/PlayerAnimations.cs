using Unity.Cinemachine;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private ParticleSystem _moveDustVFX;
    [SerializeField] private ParticleSystem _poofDustVFX;
    [SerializeField] private float _tiltAngle = 20f;
    [SerializeField] private float _tiltSpeed = 5f;
    [SerializeField] private Transform _characterSpriteTransform;
    [SerializeField] private Transform _cowboyHatTransform;
    [SerializeField] private float _cowboyHatTiltModifier = 2f;
    [SerializeField] private float _yLandVelocityCheck = -10f;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    private Vector2 _velocityBeforePhysicsUpdate;
    private Rigidbody2D _rigidBody;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        DetectMoveDust();
        ApplyTilt();
    }

    void OnEnable()
    {
        PlayerController.OnJump += PlayPoofMoveDust;
    }

    void OnDisable()
    {
        PlayerController.OnJump -= PlayPoofMoveDust;
    }

    private void FixedUpdate()
    {
        _velocityBeforePhysicsUpdate = _rigidBody.linearVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_velocityBeforePhysicsUpdate.y < _yLandVelocityCheck)
        {
            PlayPoofMoveDust();
            _impulseSource.GenerateImpulse();
        }
    }

    private void DetectMoveDust()
    {
        if (PlayerController.Instance.CheckGrounded())
        {
            if (!_moveDustVFX.isPlaying)
            {
                _moveDustVFX.Play();
            }
        }
        else
        {
            if (_moveDustVFX.isPlaying)
            {
                _moveDustVFX.Stop();
            }
        }
    }
    private void PlayPoofMoveDust()
    {
        _poofDustVFX.Play();
    }

    private void ApplyTilt()
    {
        float targetAngle;

        if (PlayerController.Instance.MoveInput.x < 0f)
        {
            targetAngle = _tiltAngle;
        }
        else if (PlayerController.Instance.MoveInput.x > 0f)
        {
            targetAngle = -_tiltAngle;
        }
        else
        {
            targetAngle = 0f;
        }

        // Player Sprite
        Quaternion currentCharacterRotations = _characterSpriteTransform.rotation;
        Quaternion targetCharacterRotation = Quaternion.Euler(currentCharacterRotations.eulerAngles.x,
            currentCharacterRotations.eulerAngles.y, targetAngle);

        _characterSpriteTransform.rotation = Quaternion.Lerp(currentCharacterRotations, targetCharacterRotation, _tiltSpeed * Time.deltaTime);


        // Cowboy Hat Sprite
        Quaternion currentHatRotations = _cowboyHatTransform.rotation;
        Quaternion targetHatRotation = Quaternion.Euler(currentHatRotations.eulerAngles.x,
            currentHatRotations.eulerAngles.y, -targetAngle - _cowboyHatTiltModifier);

        _cowboyHatTransform.rotation = Quaternion.Lerp(currentHatRotations, targetHatRotation, _tiltSpeed * _cowboyHatTiltModifier * Time.deltaTime);

    }
}
