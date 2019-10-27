using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public static Piece Instance { set; get; }
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public bool isLight;
    public bool isBoss;

    public void Start()
    {
        Instance = this;
    }

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public bool[,] PossibleMove()
    {
        bool[,] r = new bool[5, 5];

        Piece c;
        int i;

        // Right
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 5)
            {
                r[i - 1, CurrentY] = true;
                break;
            }

            c = BoardManager.Instance.Pieces[i, CurrentY];
            if (c != null)
            {
                r[i - 1, CurrentY] = true;
                break;
            }
        }

        // Left
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0)
            {
                r[i + 1, CurrentY] = true;
                break;
            }

            c = BoardManager.Instance.Pieces[i, CurrentY];
            if (c != null)
            {
                r[i + 1, CurrentY] = true;
                break;
            }

        }

        // Up 
        i = CurrentY;
        while (true)
        {
            i++;
            if (i >= 5)
            {
                r[CurrentX, i - 1] = true;
                break;
            }

            c = BoardManager.Instance.Pieces[CurrentX, i];
            if (c != null)
            {
                r[CurrentX, i - 1] = true;
                break;
            }
        }

        // Down 
        i = CurrentY;
        while (true)
        {
            i--;
            if (i < 0)
            {
                r[CurrentX, i + 1] = true;
                break;
            }

            c = BoardManager.Instance.Pieces[CurrentX, i];
            if (c != null)
            {
                r[CurrentX, i + 1] = true;
                break;
            }
        }

        if (!isBoss)
            r[2, 2] = false;

        r[CurrentX, CurrentY] = false;
        return r;
    }

}
