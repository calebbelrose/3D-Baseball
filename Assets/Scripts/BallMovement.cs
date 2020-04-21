using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BallMovement : MonoBehaviour
{
    public Rigidbody rb;
    public NavMeshAgent[] fielders; int shortestIndex = 0;
    Vector3 prevPosition = Vector3.zero;
    bool noCollision = true;
    bool notCaught = true;

    // Start is called before the first frame update
    void Start()
    {
        Hit();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log((fielders[shortestIndex].transform.position - prevPosition) / Time.deltaTime);
        //prevPosition = fielders[shortestIndex].transform.position;
    }


    void Hit()
    {
        Debug.Log("1");
        Vector3 originalLocation = transform.position;
        Quaternion originalRotation = transform.rotation;
        Vector3 hitDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
        float time = 0f;

        Physics.autoSimulation = false;
        rb.AddForce(hitDirection * 1000f);

        while (noCollision)
            Physics.Simulate(0.01f);

        
        float shortestDistance = Vector3.Distance(fielders[0].transform.position, transform.position);

        for (int i = 1; i < fielders.Length; i++)
        {
            float distance = Vector3.Distance(fielders[i].transform.position, transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                shortestIndex = i;
            }
        }

        ResetHit(hitDirection, originalLocation, originalRotation);

        while (notCaught)
        {
            time += 0.01f;
            Physics.Simulate(0.01f);
            Vector3 newPosition = Vector3.MoveTowards(fielders[shortestIndex].transform.position, transform.position, time);
            if (Vector3.Distance(newPosition, transform.position) <= 0.5f)
                notCaught = false;
        }

        fielders[shortestIndex].SetDestination(transform.position);
        ResetHit(hitDirection, originalLocation, originalRotation);
        Physics.autoSimulation = true;
    }

    void ResetHit(Vector3 hitDirection, Vector3 originalLocation, Quaternion originalRotation)
    {
        rb.Sleep();
        rb.AddForce(hitDirection * 1000f);
        transform.position = originalLocation;
        transform.rotation = originalRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Field")
            noCollision = false;
        else if (collision.gameObject.tag == "Fielder")
        {
            transform.SetParent(collision.gameObject.transform);
            rb.useGravity = false;
        }
    }
}
