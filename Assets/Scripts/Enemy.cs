using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float detectionRadius = 12f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private SphereCollider _rightHand;
    [SerializeField] private float punchDelay = 1.0f;

    private GameObject targetPlayer;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool _canMove;

    private new void Start()
    {
        base.Start();
        _rightHand.enabled = false;
        InvokeRepeating(nameof(DetectPlayer), 0f, 0.5f);
    }

    private void Update()
    {
        if (targetPlayer != null)
        {
            Vector3 direction = targetPlayer.transform.position - transform.position;
            direction.y = 0;
            float distance = direction.magnitude;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }

            // 👉 Chặn di chuyển nếu đang tấn công
            if (!isAttacking && distance > stoppingDistance)
            {
                direction.Normalize();
                Vector3 movement = direction * moveSpeed;
                _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
                UpdateAnimation(movement);
                Debug.Log("di chuyen");
            }
            else
            {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
                UpdateAnimation(Vector3.zero);

                // Nếu không di chuyển, và không đang đấm thì bắt đầu đấm
                if (canAttack && !isAttacking)
                {
                    StartCoroutine(Punch());
                }
            }
        }
        else
        {
            UpdateAnimation(Vector3.zero);
        }
    }


    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            targetPlayer = hits[0].gameObject;
        }
    }

    private IEnumerator Punch()
    {
        isAttacking = true;
        canAttack = false;

        // Chỉ set animation – animation sẽ tự gọi EnablePunch() qua event
        State = MovementState.HeadPunch;
        _animator.SetInteger("State", (int)State);

        yield return new WaitForSeconds(1.9f * 2); // Cooldown giữa các đòn
        canAttack = true;
        isAttacking = false;
    }


    public void EnablePunch()
    {
        _rightHand.enabled = true;
        StartCoroutine(DisablePunchDelay());
    }

    private IEnumerator DisablePunchDelay()
    {
        yield return new WaitForSeconds(0.1f);
        _rightHand.enabled = false;
    }

    public void UpdateAnimation(Vector3 movement)
    {
        if (movement.magnitude > 0.1f)
        {
            State = MovementState.running;
        }
        else if (!isAttacking)
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
