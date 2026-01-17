using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float attackTime;
    [SerializeField] private float attackDamage;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(AttackRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            anim.SetTrigger("fireball");
            yield return new WaitForSeconds(attackTime);
        }
    }

    private void ThrowBall()
    {
        Instantiate(ball, spawnPoint.position, transform.rotation);
    }
}
