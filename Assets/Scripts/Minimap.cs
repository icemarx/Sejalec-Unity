using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform sejalec;

    void LateUpdate()
    {
        Vector3 newPosition = sejalec.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, sejalec.eulerAngles.y, 0f);
    }
}
