using System.Collections.Generic;
using UnityEngine;

public enum MovementState { idle, running, HeadPunch, HeadHit, KnockedOut, Victory }

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Joystick _stick;
    [SerializeField] private float _moveSpeed;
    private List<Player> _allPlayers;
    private Enemy[] _allEnemy;


    public void SetUp(List<Player> player)
    {
        _allPlayers = player;
    }


    private void Update()
    {
        if (_allPlayers == null) return;
        float horizontalInput = _stick.Horizontal;
        float verticalInput = _stick.Vertical;
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        foreach (var player in _allPlayers)
        {
            if (!player.CanControl) continue;
            if (movement != Vector3.zero)
            {
                player.Move(new Vector2(movement.x * _moveSpeed, movement.z * _moveSpeed));
            }
            else
            {
                player.DetectEnemiesInRange();
            }
            player.UpdateAnimationByMove(movement);
            if (movement != Vector3.zero)
            {
                player.transform.forward = movement;
            }
        }
    }
}