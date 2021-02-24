using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController3 : MonoBehaviour
{

    //Behavoiur:
    //Wait after spawn (roam around/ go to waypoint...) -> Idle state
    //if distance from player < 10 -> chaseState
    //if distance from player < 5 -> attackState

    public Transform target;

    public float Speed = 3f;
    //public EnemyType type;

    private int health = 10;

    private EnemyStateMachine stateMachine;

    public float initialFirerate = 2f;
    private float shotCooldown;
    private float nextShotTime;

    private bool isFrozen;


    // Start is called before the first frame update
    void Start()
    {
        SetupStateMachine();
        SetFirerate(initialFirerate);
    }

    // Update is called once per frame
    void Update()
    {
        var distanceFromTarget = Vector3.Distance(transform.position, target.position);
        stateMachine.distanceFromTarget = distanceFromTarget;

        stateMachine.health = health;
        stateMachine.isFrozen = isFrozen;

        stateMachine.Update();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            health -= 2;
            Debug.Log("Health: " + health);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            health += 2;
            Debug.Log("Health: " + health);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isFrozen = !isFrozen;
        }
    }

    public void Shoot()
    {
        if (nextShotTime < Time.time)
        {
            var bullet = AssetProvider.GetAsset(GameAsset.Bullet);
            //bullet.GetComponent<BulletController>().Activate(transform);
            nextShotTime = Time.time + shotCooldown;
        }
    }

    public void SetFirerate(float firerate)
    {
        shotCooldown = 1f / firerate;
    }

    private void SetupStateMachine()
    {
        stateMachine = new EnemyStateMachine();
        stateMachine.controller = this;
    }
}
