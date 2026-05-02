using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
    [Header("General")]
    public List<Vector3> targetPositions;
    public List<Quaternion> targetRotations;
    public int targetIndex;

    [UnityEngine.Range(0f, 3f)]
    public float fullTime = 2.0f;

    [HideInInspector]
    public bool moving = false;

    [Header("Navigation")]
    public UI userInterface;
    
 


    public void changePosition()
    {
        if (moving) return;
        moving = true;
        targetIndex = (targetIndex + 1) % targetPositions.Count;
        StartCoroutine(moveCamera());

    }

    IEnumerator moveCamera()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPostion = targetPositions[targetIndex];
        float counter = 0;
        while(counter < fullTime)
        {
            yield return null;
            transform.position = Vector3.Lerp(startPosition,endPostion, counter/fullTime);
            counter += Time.deltaTime;
        }
        transform.position = endPostion;
        moving = false;

    }


}
