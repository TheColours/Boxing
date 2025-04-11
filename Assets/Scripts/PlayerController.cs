using UnityEngine;

public enum MovementState { idle, running, HeadPunch, HeadHit, KnockedOut, Victory }

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Joystick _stick;
    [SerializeField] private float _moveSpeed;
    private Player[] _allPlayers;
    void Awake()
    {
        _allPlayers = GetComponentsInChildren<Player>();
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

        foreach (var player in _allPlayers)
        {
            if (movement != Vector3.zero)
            {
                player.Move(new Vector2(movement.x * _moveSpeed, movement.z * _moveSpeed));
            }
            else
            {
                player.DetectEnemiesInRange();
            }
            player.UpdateAnimation(movement);
            if (movement != Vector3.zero)
            {
                player.transform.forward = movement;
            }
        }
    }
}