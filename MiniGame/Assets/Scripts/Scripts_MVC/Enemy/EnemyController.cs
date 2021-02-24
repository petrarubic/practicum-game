using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    //Behavoiur:
    //Wait after spawn (roam around/ go to waypoint...) -> Idle state
    //if distance from player < 10 -> chaseState
    //if distance from player < 5 -> attackState

    public Transform target;

    public float Speed = 3f;
    //public EnemyType type;

    private int health = 10;
    private float distanceFromTarget;

    private EnemyState state = EnemyState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetState();

        UpdateForState();
    }

    private void SetState()
    {
        distanceFromTarget = Vector3.Distance(transform.position, target.position);

        if (health <= 0) state = EnemyState.Dead;

        else if (distanceFromTarget < 5) state = EnemyState.Attack;

        else if (distanceFromTarget < 10) state = EnemyState.Chase;

        else state = EnemyState.Idle;
    }

    private void UpdateForState()
    {
        switch (state)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Chase:
                ChaseState();
                break;
            case EnemyState.Attack:
                AttackState();
                break;
            case EnemyState.Dead:
                DeadState();
                break;
            default:
                break;
        }
    }

    private void IdleState()
    {
        transform.LookAt(target);
    }

    private void ChaseState()
    {
        transform.LookAt(target);
        var direction = target.position - transform.position;
        var movementVector = direction.normalized * Time.deltaTime * Speed;

        transform.position += movementVector;
    }

    private void AttackState()
    {
        transform.LookAt(target);
        Shoot();
    }

    private void DeadState()
    {
        //Clean up... 
    }

    public void Shoot()
    {
        var bullet = AssetProvider.GetAsset(GameAsset.Bullet);
        //bullet.GetComponent<BulletController>().Activate(transform);
    }

    public void Activate(Vector3 position, Transform target)
    {
        this.target = target;
        transform.position = position;
    }
}

public enum EnemyState
{
    Idle, Chase, Attack, Dead
}
