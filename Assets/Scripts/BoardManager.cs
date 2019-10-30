using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { set; get; }

    public Piece[,] Pieces { set; get; }
    private Piece selectedPiece;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private float mousePosX = -1;
    private float mousePosY = -1;
    private int selectionX = -1;
    private int selectionY = -1;
    public float[] Getselection()
    {
        float[]  selection = { mousePosX, mousePosY };
        return selection;
    }


    public List<GameObject> piecePrefabs;
    private List<GameObject> activePiece = new List<GameObject>();

    private Material previousMat;
    public Material selectedMat;

    // To set Prefab
    private float piecePositionY = -0.21f;
    private Quaternion orientation = Quaternion.Euler(-90, 0, 0);

    // Controll
    public bool isLightTurn = true;
    public bool touchingFlag = false;
    string direction = null;

    // Animation
    public bool movingFlag = false;
    public Vector3 target;
    public Vector3 moveVector;



    //==============================
    //  Functions
    //==============================
    private void Start()
    {
        Instance = this;
        SpawnAllPieces();
    }

    private void Update()
    {
        DrawSLIPESBoard();
        UpdateSelection();

        Inputflick.Instance.Flick();
        direction = Inputflick.Instance.Getdirection();

        if (direction != null) {
            if (direction == "touch" && !movingFlag)
            {
                if (selectionX >= 0 && selectionY >= 0)
                {
                    if (selectedPiece == null)
                    {
                        //Select the piece
                        SelectPiece(selectionX, selectionY);
                    }
                    else
                    {
                        //Move the piece
                        MovePiece(selectionX, selectionY);
                    }
                }

                touchingFlag = !touchingFlag;
            }
            else if (direction != "touch" && !movingFlag && !touchingFlag)
            {
                float[] startPos = Inputflick.Instance.GettouchStartPos();
                Debug.Log(startPos[0]);
                Debug.Log(startPos[1]);
                if(startPos[0] >= 0 && startPos[1] >= 0)
                {
                    SelectPiece((int)startPos[0], (int)startPos[1]);

                    if(selectedPiece != null)
                    {
                        int[] destinationPos = GetDestination();
                        MovePiece(destinationPos[0], destinationPos[1]);
                    }
                }
            }
        }
        else if (movingFlag)
        {
            if (target == selectedPiece.transform.position)
            {
                SwitchTurn();
            }
            else
            {
                // Moving Piece
                selectedPiece.transform.position += moveVector * -0.1f;
            }
        }

    }

    private void SelectPiece(int x, int y)
    {
        if (Pieces[x, y] == null)
            return;

        if (Pieces[x, y].isLight != isLightTurn)
            return;


        bool hasAtleastOneMove = false;
        allowedMoves = Pieces[x, y].PossibleMove();
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                if (allowedMoves[i, j])
                    hasAtleastOneMove = true;

        if (!hasAtleastOneMove)
            return;

        selectedPiece = Pieces[x, y];

        SwitchSelectedMaterial();

        BoardHighlights.Instance.HighLightAllowedmoves(allowedMoves);
    }

    private void MovePiece(int x, int y)
    {
        // Change to default Material
        selectedPiece.GetComponent<MeshRenderer>().material = previousMat;

        if (allowedMoves[x, y])
        {
            Pieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;

            //selectedPiece.transform.position = GetTileCenter(x, y);
            moveVector = (selectedPiece.transform.position - GetTileCenter(x, y)).normalized;
            target = GetTileCenter(x, y);
            movingFlag = true;

            selectedPiece.SetPosition(x, y);
            Pieces[x, y] = selectedPiece;

        }
        else
        {
            selectedPiece = null;
        }

        BoardHighlights.Instance.Hidehighlights();
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("SLIPESPlane")))
        {
            //Debug.Log(hit.point);
            mousePosX = hit.point.x;
            mousePosY = hit.point.z;
            selectionX = (int)mousePosX;
            selectionY = (int)mousePosY;
        }
        else
        {
            mousePosX = -1;
            mousePosY = -1;
            selectionX = -1;
            selectionY = -1;
        }

    }

    //------------------------------
    //  SpawnPieces
    //------------------------------
    private void SpawnPiece(int index, int x, int y)
    {
        GameObject go = Instantiate(piecePrefabs[index], GetTileCenter(x, y), orientation) as GameObject;

        go.transform.SetParent(transform);
        Pieces[x, y] = go.GetComponent<Piece>();
        Pieces[x, y].SetPosition(x, y);
        activePiece.Add(go);
    }

    private void SpawnAllPieces()
    {
        activePiece = new List<GameObject>();
        Pieces = new Piece[5, 5];

        //-------------------------
        //  Spawn the light team.
        //-------------------------
        for(int i=0; i<5; i++)
        {
            if(i == 2)
                SpawnPiece(1, i, 0);
            else
                SpawnPiece(0, i, 0);
        }

        //-------------------------
        //  Spawn the dark team.
        //-------------------------
        for(int i=0; i<5; i++)
        {
            if(i == 2)
                SpawnPiece(3, i, 4);
            else
                SpawnPiece(2, i, 4);
        }

    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.y = piecePositionY;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private void DrawSLIPESBoard()
    {
        Vector3 widthLine = Vector3.right * 5;
        Vector3 heightLine = Vector3.forward * 5;

        for (int i=0; i<=5; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start+widthLine);
            for (int j=0; j<=5; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start+heightLine);
            }
        }

        // Draw the selection
        if(selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1)
            );

            Debug.DrawLine(
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1)
            );
        }
    }

    private void SwitchSelectedMaterial()
    {
        // Change Material
        previousMat = selectedPiece.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedPiece.GetComponent<MeshRenderer>().material = selectedMat;
    }

    private int[] GetDestination()
    {
        int i;
        int[] destinationPos = { selectedPiece.CurrentX, selectedPiece.CurrentY };

        switch(direction){
            // Right
            case "right":
                i = selectedPiece.CurrentX + 1;
                while(true)
                {
                    if(i >= 5)
                        break;

                    if (allowedMoves[i,selectedPiece.CurrentY])
                    {
                        destinationPos[0] = i;
                        break;
                    }
                    i++;
                }
                break;

            // Left
            case "left":
                i = selectedPiece.CurrentX - 1;
                while(true)
                {
                    if(i < 0)
                        break;

                    if (allowedMoves[i,selectedPiece.CurrentY])
                    {
                        destinationPos[0] = i;
                        break;
                    }
                    i--;
                }
                break;

            // Up
            case "up":
                i = selectedPiece.CurrentY + 1;
                while(true)
                {
                    if(i >= 5)
                        break;

                    if (allowedMoves[selectedPiece.CurrentX, i])
                    {
                        destinationPos[1] = i;
                        break;
                    }
                    i++;
                }
                break;

            // Down
            case "down":
                i = selectedPiece.CurrentY - 1;
                while(true)
                {
                    if(i < 0)
                        break;

                    if (allowedMoves[selectedPiece.CurrentX, i])
                    {
                        destinationPos[1] = i;
                        break;
                    }
                    i--;
                }
                break;
        }

        return destinationPos;
    }

    private void SwitchTurn()
    {
        movingFlag = false;
        selectedPiece = null;

        // Check End Game
        if (Pieces[2, 2])
        {
            EndGame();
        }

        isLightTurn = !isLightTurn;
    }

    private void EndGame()
    {
        if (isLightTurn)
            Debug.Log("Light team wins");
        else
            Debug.Log("Dark team wins");

        foreach (GameObject go in activePiece)
            Destroy(go);

        isLightTurn = true;
        BoardHighlights.Instance.Hidehighlights();
        SpawnAllPieces();
    }
}
