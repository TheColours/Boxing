using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float detectionRadius = 12f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private Hand _rightHand;
    public List<Character> PotentialTargets;
    private Character _targetPlayer;
    private bool _isHit;
    private bool _canMove;
    private Coroutine _hitRecoveryCoroutine;

    private new void Start()
    {
        base.Start();
        _rightHand.EnablePunch(false);
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }
    public void SetMove()
    {
        _canMove = true;
    }

    private void Update()
    {
        if (_targetPlayer == null)
        {
            if (State != MovementState.Victory)
            {
                State = MovementState.Victory;
                _animator.SetInteger("State", (int)State);
            }
            return;
        }
        if (_isHit) return;
        Vector3 direction = _targetPlayer.transform.position - transform.position;
        direction.y = 0f;
        float distance = direction.magnitude;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
        if (distance <= stoppingDistance)
        {
            if (State != MovementState.HeadPunch)
            {
                _canMove = false;
                State = MovementState.HeadPunch;
                _animator.SetInteger("State", (int)State);
            }
        }
        else
        {
            direction.Normalize();
            Vector3 movement = direction * moveSpeed;
            if (_canMove)
            {
                _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
            }
            UpdateAnimationByMove(movement);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !other.TryGetComponent(out Hand hand) ||
        hand.Main.State != MovementState.HeadPunch)
            return;
        other.enabled = false;

        HP -= 10;
        _isHit = true;
        _canMove = false;

        if (HP > 0)
        {
            State = MovementState.HeadHit;
            _animator.SetInteger("State", (int)State);
            if (_hitRecoveryCoroutine != null)
            {
                StopCoroutine(_hitRecoveryCoroutine);
                AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                _animator.Play(stateInfo.fullPathHash, 0, 0f);
            }
            _hitRecoveryCoroutine = StartCoroutine(Common.Delay(1.12f, () =>
            {
                _hitRecoveryCoroutine = null;
                _isHit = false;
            }));
        }
        else
        {
            if (_hitRecoveryCoroutine != null)
            {
                StopCoroutine(_hitRecoveryCoroutine);
            }
            State = MovementState.KnockedOut;
            _animator.SetInteger("State", (int)State);
            GameManager.Instance.CheckComplete();
        }
    }

    private void UpdateTarget()
    {
        float closestDistance = Mathf.Infinity;
        Character closestPlayer = null;
        foreach (var player in PotentialTargets)
        {
            if (player == null || player.HP <= 0) continue;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < detectionRadius && distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        _targetPlayer = closestPlayer;
    }
    private IEnumerator Test(float alpha, Action callback)
    {
        yield return new WaitForSeconds(alpha);
        callback.Invoke();
    }
    public void EnablePunch() => _rightHand.EnablePunch(true);

    public void DisablePunch() => _rightHand.EnablePunch(false);


    private void UpdateAnimationByMove(Vector3 movement)
    {
        if (_isHit) return;
        if (movement.magnitude > 0.1f)
        {
            State = MovementState.running;
        }
        else
        {
            State = MovementState.idle;
        }

        _animator.SetInteger("State", (int)State);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}
