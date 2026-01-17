using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private float inputH;
    private Animator anim;

    [Header("Movement System")]
    [SerializeField] private Transform feet;
    [SerializeField] private float movementVelocity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float distanceDetectionGround;
    [SerializeField] private LayerMask jumpedLayer;

    [Header("Combat System")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackDamage;
    [SerializeField] private LayerMask damageableLayer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Jump();

        LaunchAttack_1();
    }

    private void LaunchAttack_1()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("attack_1");
        }
    }

    private void Attack()
    {
        Collider2D[] touchedColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, damageableLayer);
        foreach (Collider2D item in touchedColliders)
        {
            HealthSystem healthSystem = item.gameObject.GetComponent<HealthSystem>();
            healthSystem.ReceivedDamage(attackDamage);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && OnGround())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("jump");
        }
    }

    private bool OnGround()
    {
        return Physics2D.Raycast(feet.position, Vector3.down, distanceDetectionGround, jumpedLayer);        
    }

    private void Movement()
    {
        inputH = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(inputH * movementVelocity, rb.velocity.y);

        if (inputH != 0)
        {
            anim.SetBool("running", true);
            if (inputH > 0)
            {
                transform.eulerAngles = Vector3.zero;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        else
        {
            anim.SetBool("running", false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(attackPoint.position, attackRadius);
    }
}
