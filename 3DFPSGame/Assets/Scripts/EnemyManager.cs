using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public int enemyHealth = 200;

    //Navmesh
    public NavMeshAgent enemyAgent;
    public Transform player;
    public LayerMask groundLayer;
    public LayerMask playerLayer;

    //Patrolling
    public Vector3 walkPoint;
    public float walkPointRange;
    public bool walkPointSet;

    //state
    public float sightRange, attackRange;
    public bool EnemySightRange, EnemyAttackRange;

    //attack
    public float attackDelay;
    public bool isAttacking;
    public Transform attackPoint;
    public GameObject projectile;
    public float projectileForce = 18f;

    //animasyon
    private Animator enemyAnimator;

    //killcount
    private GameManager gameManager;

    //Particule effect
    public ParticleSystem deadEffect;

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemySightRange = Physics.CheckSphere(transform.position,sightRange,playerLayer);
        EnemyAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if(!EnemySightRange && !EnemyAttackRange)
        {
            //Patrolling koruduðu yerde kordinatlar hesaplayacak
            Patrolling();
            //anim
            enemyAnimator.SetBool("Patrolling",true);
            enemyAnimator.SetBool("PlayerAtacking", false);
            enemyAnimator.SetBool("PlayerDetecting", false);
        }
        else if(EnemySightRange && !EnemyAttackRange)
        {
            //Detecting enemy
            DetectPlayer();
            //anim
            enemyAnimator.SetBool("PlayerDetecting", true);
            enemyAnimator.SetBool("Patrolling", false);
            enemyAnimator.SetBool("PlayerAtacking", false);
            
        }
        else if(EnemySightRange&&EnemyAttackRange)
        {
            //attack player
            AttackPlayer();
            //anim
            enemyAnimator.SetBool("PlayerAtacking", true);
            enemyAnimator.SetBool("PlayerDetecting", false);
            enemyAnimator.SetBool("Patrolling", false);
            
        }
            
    }
    void Patrolling()
    {
        if(walkPointSet==false)
        {
            float randomZPos = Random.Range(-walkPointRange, walkPointRange);
            float randomXPos = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomXPos,transform.position.y,transform.position.z + randomZPos);
            
        }

        //raycast ile ground kontrolü
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
        if (walkPointSet == true)
        {
            enemyAgent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    void DetectPlayer()
    {
        enemyAgent.SetDestination(player.position);
        transform.LookAt(player);
    }
    public void EnemyTakeDamage(int damageAmount)
    {
        enemyHealth -= damageAmount;
        if(enemyHealth<=0)
        {
            EnemyDeath();
        }
    }
    void AttackPlayer()
    {
        if(isAttacking==true)
        {
            enemyAgent.SetDestination(transform.position);
            transform.LookAt(player);
        }
       
        if(isAttacking==false)
        {

            //attack türü
            Rigidbody rbEnemy = Instantiate(projectile, attackPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rbEnemy.AddForce(transform.forward * projectileForce, ForceMode.Impulse);

            isAttacking = true;
            Invoke("ResetAttack", attackDelay);
        }
        
    }
    void ResetAttack()
    {
        isAttacking = false;
    }
    void EnemyDeath()
    {
        Destroy(gameObject);

        //playera puan kazanma mekaniði eklenebilir-yendiðimiz düþmanlarý topla

        gameManager = FindObjectOfType<GameManager>();
        gameManager.AddKill();
        Instantiate(deadEffect, transform.position, Quaternion.identity);

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);

       
    }
}
