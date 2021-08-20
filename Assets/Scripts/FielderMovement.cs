using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FielderMovement : MonoBehaviour
{
    Vector3 previousPosition;

    private void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        previousPosition = transform.position;
    }
}
