using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputflick : MonoBehaviour
{
    public static Inputflick Instance { set; get; }

    private float[] touchStartPos;
    private float[] touchEndPos;
    private string direction;


    public void Start()
    {
        Instance = this;
    }

    public string Getdirection()
    {
        return direction;
    }

    public float[] GettouchStartPos()
    {
        return touchStartPos;
    }

    public void Flick()
    {

        direction = null;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            touchStartPos = BoardManager.Instance.Getselection();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            touchEndPos = BoardManager.Instance.Getselection();
            GetDirection();
        }
    }

    private void GetDirection()
    {
        float directionX = touchEndPos[0] - touchStartPos[0];
        float directionY = touchEndPos[1] - touchStartPos[1];

        if (Mathf.Abs(directionY) < Mathf.Abs(directionX))
        {
            if (0.1 < directionX)
            {
                //右向きにフリック
                direction = "right";
            }
            else if (-0.1 > directionX)
            {
                //左向きにフリック
                direction = "left";
            }
        }
        else if (Mathf.Abs(directionX) < Mathf.Abs(directionY))
        {
            if (0.1 < directionY) {
                //上向きにフリック
                direction = "up";
            } else if (-0.1 > directionY) {
                //下向きのフリック
                direction = "down";
            }
        } else {
            //タッチを検出
            direction = "touch";
        }

        //Debug.Log(direction);
    }
}
