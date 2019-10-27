using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSlide : MonoBehaviour
{
    private float speed = 2.0f;
    public void moveAnimation(GameObject go, Vector3 target)
    {
        go.transform.position = Vector3.MoveTowards(go.transform.position, target, speed);
    }
}
