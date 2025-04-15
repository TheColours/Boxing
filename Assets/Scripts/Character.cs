
using UnityEngine;

public class Character : MonoBehaviour
{
    protected Rigidbody _rigidbody;
    protected Animator _animator;
    protected Collider _collider;
    public MovementState State;
    public int HP;

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        HP = 100;
    }
    public void Victory()
    {
        State = MovementState.Victory;
        _animator.SetInteger("State", (int)State);
    }
}
