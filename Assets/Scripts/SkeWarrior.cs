using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeWarrior : MonoBehaviour
{
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float speedPatrol;
    private Vector3 currentDestination;
    private int currentIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        currentDestination = wayPoints[currentIndex].position;
        StartCoroutine(Patrol());
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            while (transform.position != currentDestination)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentDestination, speedPatrol * Time.deltaTime);
                yield return null;
            }
            DefineNewDestination();
        }
    }

    private void DefineNewDestination()
    {
        currentIndex++;
        if (currentIndex >= wayPoints.Length)
        {
            currentIndex = 0;
        }
        currentDestination = wayPoints[currentIndex].position;
        FocusDestination();
    }

    private void FocusDestination()
    {
        if (currentDestination.x > transform.position.x)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
