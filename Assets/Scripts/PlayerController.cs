using UnityEngine;

public enum MovementState { idle, running }

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Joystick _stick;
    [SerializeField] private float _moveSpeed;

    private Rigidbody _rigidbody;
    private Animator _animator;
    private MovementState _state = MovementState.idle;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning("Không tìm thấy Animator. Vui lòng thêm vào GameObject.");
        }
    }

    private void Update()
    {
        // Lấy giá trị input từ joystick
        float horizontalInput = _stick.Horizontal;
        float verticalInput = _stick.Vertical;

        // Tạo vector di chuyển chỉ theo trục X và Z
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        // Chuẩn hóa vector nếu người chơi đẩy joystick theo đường chéo
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Di chuyển player
        _rigidbody.velocity = new Vector3(movement.x * _moveSpeed, _rigidbody.velocity.y, movement.z * _moveSpeed);

        // Xoay player theo hướng di chuyển (nếu đang di chuyển)
        if (movement != Vector3.zero)
        {
            transform.forward = movement;
        }

        UpdateAnimation(movement);
    }

    private void UpdateAnimation(Vector3 movement)
    {
        if (_animator == null) return;

        // Xác định trạng thái di chuyển
        MovementState state;

        // Nếu người chơi đang di chuyển (magnitude > 0.1 để loại bỏ input nhỏ)
        if (movement.magnitude > 0.1f)
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idle;
        }

        // Cập nhật animation chỉ khi trạng thái thay đổi
        if (state != _state)
        {
            _state = state;
            // Cập nhật tham số trong Animator
            _animator.SetInteger("State", (int)state);
        }
    }
}