using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BallMovement : MonoBehaviour
{
    public Rigidbody rb;
    public NavMeshAgent[] fielders;

    int bestFielder = 0;
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

        if (notCaught && Vector3.Distance(fielders[bestFielder].transform.position, transform.position) <= 60f)
        {
            transform.SetParent(fielders[bestFielder].transform, true);
            rb.useGravity = false;
            rb.Sleep();
            Debug.Log("Caught " + Time.time);
            notCaught = false;
        }
        if (fielders[bestFielder].transform.position != prevPosition)
        {
            prevPosition = fielders[bestFielder].transform.position;
        }
    }


    void Hit()
    {
        Vector3 originalLocation = transform.position;
        Quaternion originalRotation = transform.rotation;
        Vector3 hitDirection = new Vector3(Random.Range(0f, 1f), Random.Range(-1f, 1f), Random.Range(0f, 1f));
        float time = 0f;
        Vector3 bestFielderPosition = Vector3.zero;

        rb.AddForce(hitDirection * 10000f);
        Physics.autoSimulation = false;

        float shortestDistance = float.MaxValue;
        float runTime = 0f;
        
        while (notCaught && time < 500f)
        {
            time += 0.02f;
            Physics.Simulate(0.02f);

            if (time == 0.02f)
                runTime += time * time;
            else
                runTime += 0.02f;

            if (transform.position.y < 100f)
            {
                shortestDistance = Vector3.Distance(fielders[0].transform.position, transform.position);

                for (int i = 0; i < fielders.Length; i++)
                {
                    Vector3 fielderPosition = Vector3.MoveTowards(fielders[i].transform.position, new Vector3(transform.position.x, 0.0f, transform.position.z), runTime * 100.0f);
                    float distance = Vector3.Distance(fielderPosition, transform.position);

                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        bestFielder = i;
                        bestFielderPosition = fielderPosition;
                    }
                    else if (distance == shortestDistance && Vector3.Distance(fielders[i].transform.position, transform.position) < Vector3.Distance(fielders[bestFielder].transform.position, transform.position))
                    {
                        bestFielder = i;
                        bestFielderPosition = fielderPosition;
                    }
                }

                if (shortestDistance <= 50f)
                    notCaught = false;
            }
        }
        Debug.Log(time + " " + transform.position.y + " " + shortestDistance);

        notCaught = true;
        Physics.autoSimulation = true;
        ResetHit(hitDirection, originalLocation, originalRotation);
        fielders[bestFielder].SetDestination(bestFielderPosition);
        prevPosition = fielders[bestFielder].transform.position;
        Debug.Log(Time.time);
    }

    void ResetHit(Vector3 hitDirection, Vector3 originalLocation, Quaternion originalRotation)
    {
        rb.Sleep();
        rb.AddForce(hitDirection * 10000f);
        transform.position = originalLocation;
        transform.rotation = originalRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Field")
            noCollision = false;
        else if (collision.gameObject.tag == "Bounds")
            Debug.Log("Bounds");
            
    }
}
