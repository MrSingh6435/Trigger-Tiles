using TMPro.Examples;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInputs { get; private set; }
    private PlayerInputActions _playerInputAcion;
    private InputAction _move, _jump, _jetpack, _grenade;

    void Awake()
    {
        _playerInputAcion = new PlayerInputActions();

        _move = _playerInputAcion.Player.Move;
        _jump = _playerInputAcion.Player.Jump;
        _jetpack = _playerInputAcion.Player.Jetpack;
        _grenade = _playerInputAcion.Player.Grenade;
    }
    void OnEnable()
    {
        _playerInputAcion.Enable();
    }

    void OnDisable()
    {
        _playerInputAcion.Disable();
    }

    void Update()
    {
        FrameInputs = GatherInput();
    }

    private FrameInput GatherInput()
    {
        return new FrameInput
        {
            Move = _move.ReadValue<Vector2>(),
            Jump = _jump.WasPressedThisFrame(),
            Jetpack = _jetpack.WasPressedThisFrame(),
            Grenade = _grenade.WasPressedThisFrame(),
        };
    }

    public struct FrameInput
    {
        public Vector2 Move;
        public bool Jump;
        public bool Jetpack;
        public bool Grenade;
    }
}
