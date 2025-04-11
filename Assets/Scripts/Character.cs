using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected Rigidbody _rigidbody;
    protected Animator _animator;
    public MovementState State;
    public int HP;

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        HP = 100;
    }
}
