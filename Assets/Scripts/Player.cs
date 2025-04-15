using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Hand _rightHand;
    private List<Enemy> _enemiesNearby = new List<Enemy>();
    private Enemy _targetEnemy;
    private Coroutine _hitRecoveryCoroutine;
    private bool _isMovingToEnemy;

    public bool CanControl { get; private set; }

    private new void Start()
    {
        base.Start();
        _rightHand.EnablePunch(false);
        CanControl = true;
    }
    private void Update()
    {
        if (GameManager.Instance.CurrentEnemy.All(enemy => enemy.HP <= 0))
        {
            if (State != MovementState.Victory)
            {
                State = MovementState.Victory;
                _animator.SetInteger("State", (int)State);
            }
            return;
        }
        if (!CanControl) return;

        if (_isMovingToEnemy && _targetEnemy != null && _targetEnemy.HP > 0)
        {
            MoveTowardsEnemy();
        }
    }

    private void MoveTowardsEnemy()
    {
        Vector3 direction = _targetEnemy.transform.position - transform.position;
        direction.y = 0;
        float distanceToEnemy = direction.magnitude;

        if (direction != Vector3.zero)
        {
            // Rotate towards enemy
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        if (distanceToEnemy > stoppingDistance)
        {
            // Move towards enemy
            direction.Normalize();
            Vector3 movement = direction * moveSpeed;
            _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
            UpdateAnimationByMove(movement);
        }
        else
        {
            // Arrived at enemy, stop moving
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            _isMovingToEnemy = false;
            _targetEnemy = null;
            UpdateAnimationByMove(Vector3.zero);

            // Start attack
            State = MovementState.HeadPunch;
            _animator.SetInteger("State", (int)State);
        }
    }
    //Players can cancel attacks by moving, while AI must complete an attack before moving
    public void SetMove() { return; }

    public void EnablePunch() => _rightHand.EnablePunch(true);

    public void DisablePunch() => _rightHand.EnablePunch(false);

    public void Move(Vector2 target)
    {
        if (!CanControl) return;

        // Cancel auto-movement when player takes control
        if (target.magnitude > 0.1f)
        {
            _isMovingToEnemy = false;
        }

        _rigidbody.velocity = new Vector3(target.x, _rigidbody.velocity.y, target.y);
    }

    public void UpdateAnimationByMove(Vector3 movement)
    {
        if (movement.magnitude > 0.1f)
        {
            State = MovementState.running;
        }
        else if (!_isMovingToEnemy)
        {
            State = MovementState.idle;
        }

        _animator.SetInteger("State", (int)State);
    }

    public void DetectEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        _enemiesNearby.Clear();
        foreach (var hitCollider in hitColliders)
        {
            _enemiesNearby.Add(hitCollider.GetComponent<Enemy>());
        }
        _enemiesNearby = _enemiesNearby.Where(enemy => enemy.HP > 0).ToList();

        if (_enemiesNearby.Count > 0)
        {
            if (!_isMovingToEnemy)
            {
                _targetEnemy = _enemiesNearby[Random.Range(0, _enemiesNearby.Count)];
                _isMovingToEnemy = true;
            }
        }
        else
        {
            _targetEnemy = null;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy") || !other.TryGetComponent(out Hand hand) ||
            hand.Main.State != MovementState.HeadPunch)
            return;
        other.enabled = false;
        HandleHit();
    }

    private void HandleHit()
    {
        HP -= 10;
        CanControl = false;

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
                CanControl = true;
                _hitRecoveryCoroutine = null;
            }));
        }
        else
        {
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
            if (_hitRecoveryCoroutine != null)
            {
                StopCoroutine(_hitRecoveryCoroutine);
            }
            State = MovementState.KnockedOut;
            _animator.SetInteger("State", (int)State);
            GameManager.Instance.CheckComplete();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}