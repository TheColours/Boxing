using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    private List<GameObject> enemiesNearby = new List<GameObject>();
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float stoppingDistance = 2f; 
    [SerializeField] private float moveSpeed = 3f; 
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private SphereCollider _leftHand;
    [SerializeField] private SphereCollider _rightHand;

    private bool _canMove;
    private GameObject targetEnemy;
    private bool _isMovingToEnemy = false;

    private new void Start()
    {
        base.Start();
        _leftHand.enabled = false;
        _rightHand.enabled = false;
        _canMove = true;
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        _rightHand.enabled = false;
    }
    public void EnablePunch()
    {
        _rightHand.enabled = true;
        StartCoroutine(Delay());
    }
    private void Update()
    {
        if (_isMovingToEnemy && targetEnemy != null)
        {
            Vector3 direction = targetEnemy.transform.position - transform.position;
            direction.y = 0;
            float distanceToEnemy = direction.magnitude;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }

            if (distanceToEnemy > stoppingDistance)
            {
                direction.Normalize();

                // Di chuyển
                Vector3 movement = direction * moveSpeed;
                _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);

                // Animation chạy
                UpdateAnimation(movement);
            }
            else
            {
                // Đã đến nơi, không di chuyển nhưng vẫn giữ xoay
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
                _isMovingToEnemy = false;
                targetEnemy = null;

                UpdateAnimation(Vector3.zero);

                Debug.Log("Đã đến vị trí cách enemy " + stoppingDistance + " đơn vị!");
                State = MovementState.HeadPunch;
                _animator.SetInteger("State", (int)State);
            }
        }
    }


    public void Move(Vector2 target)
    {
        if (!_canMove) return;

        // Nếu người chơi đang điều khiển, hủy việc tự động di chuyển đến enemy
        if (target.magnitude > 0.1f)
        {
            _isMovingToEnemy = false;
        }
        _rigidbody.velocity = new Vector3(target.x, _rigidbody.velocity.y, target.y);
    }

    public void UpdateAnimation(Vector3 movement)
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

        enemiesNearby.Clear();

        foreach (var hitCollider in hitColliders)
        {
            enemiesNearby.Add(hitCollider.gameObject);
        }

        if (enemiesNearby.Count > 0 && !_isMovingToEnemy)
        {
            Debug.Log("Có " + enemiesNearby.Count + " enemy ở gần!");

            // Chọn một enemy bất kỳ để đi đến
            targetEnemy = enemiesNearby[Random.Range(0, enemiesNearby.Count)];
            _isMovingToEnemy = true;
            Debug.Log("Đang tiến đến enemy: " + targetEnemy.name);
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