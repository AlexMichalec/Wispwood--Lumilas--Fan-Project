using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
    public List<Vector3> targetPositions;
    public List<Quaternion> targetRotations;
    public int targetIndex;
    public int steps = 100;
    public float fullTime = 2.0f;
    public bool moving = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            changePosition();
        }
    }

    public void changePosition()
    {
        if (moving) return;
        moving = true;
        targetIndex = (targetIndex + 1) % targetPositions.Count;
        StartCoroutine(moveCamera());

    }

    IEnumerator moveCamera()
    {
        Vector3 distance = transform.position - targetPositions[targetIndex];
        for (int i = 0; i < steps; ++i)
        {
            yield return new WaitForSeconds(fullTime / steps);
            transform.position -= distance / steps;
        }
        moving = false;
    }
}
