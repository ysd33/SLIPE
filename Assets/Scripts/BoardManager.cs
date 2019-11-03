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

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> piecePrefabs;
    private List<GameObject> activePiece = new List<GameObject>();

    public GameObject turnPanel;

    private Material previousMat;
    public Material selectedMat;

    // To set Prefab
    private float piecePositionY = -0.21f;
    private Quaternion orientation = Quaternion.Euler(-90, 0, 0);

    public bool isLightTurn = true;

    // Animation
<<<<<<< Updated upstream
    public bool movingFlag = false;
    public Vector3 target;
    public Vector3 moveVector;

=======
    private const float MOVING_STEP = 0.1f;
    private bool movingFlag = false;
    private Vector3 target;
    private Vector3 moveVector;
    private Vector3 moveDistance;
    private float panelRotateStep;


    //==============================
    //  Functions
    //==============================
>>>>>>> Stashed changes
    private void Start()
    {
        Instance = this;
        SpawnAllPieces();

        UIManager.Instance.StartGameSetup();
    }

    private void Update()
    {
        DrawSLIPEBoard();
        UpdateSelection();

        // タッチ確認
        Inputflick.Instance.Flick();

<<<<<<< Updated upstream
        if (Input.GetMouseButtonDown(0) && !movingFlag)
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedPiece == null)
=======
        // タッチされた
        if (direction != null) {

            // タップだった
            if (direction == "touch" && !movingFlag)
            {
                //ボード内のタッチ
                if (selectionX >= 0 && selectionY >= 0)
>>>>>>> Stashed changes
                {
                    //Select the piece
                    SelectPiece(selectionX, selectionY);
                }
<<<<<<< Updated upstream
                else
                {
                    //Move the piece
                    MovePiece(selectionX, selectionY);
=======

                // 一度タッチしたらもう一度タッチするまでフリック受け付けない
                touchingFlag = !touchingFlag;
            }

            // フリックだった
            else if (direction != "touch" && !movingFlag && !touchingFlag)
            {
                float[] startPos = Inputflick.Instance.GettouchStartPos();

                // フリック開始地点がボード内
                if(startPos[0] >= 0 && startPos[1] >= 0)
                {
                    // フリック開始点のピースを選択
                    SelectPiece((int)startPos[0], (int)startPos[1]);

                    // 有効なピースを選択できた
                    if(selectedPiece != null)
                    {
                        int[] destinationPos = GetDestination();
                        MovePiece(destinationPos[0], destinationPos[1]);
                    }
>>>>>>> Stashed changes
                }
            }
        }
        else if (movingFlag)
        {
            if(target == selectedPiece.transform.position)
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
            else
            {
<<<<<<< Updated upstream
                selectedPiece.transform.position += moveVector * -0.1f;
=======
                // Moving Piece
                selectedPiece.transform.position += moveVector * MOVING_STEP;

                // Rotate TurnPanel
                turnPanel.transform.Rotate(0.0f, 0.0f, panelRotateStep);
>>>>>>> Stashed changes
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

        // Change Material
        previousMat = selectedPiece.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedPiece.GetComponent<MeshRenderer>().material = selectedMat;

        BoardHighlights.Instance.HighLightAllowedmoves(allowedMoves);
    }

    private void MovePiece(int x, int y)
    {
        // Change to default Material
        selectedPiece.GetComponent<MeshRenderer>().material = previousMat;

        if (allowedMoves[x, y])
        {
            // 選ばられたピースのあった場所を開ける
            Pieces[selectedPiece.CurrentX, selectedPiece.CurrentY] = null;

            // データ上のピースの位置更新
            selectedPiece.SetPosition(x, y);
            Pieces[x, y] = selectedPiece;

            // 移動アニメーション関連
            target = GetTileCenter(x, y);                                           // ピースを動かす先の座標

            moveDistance = target - selectedPiece.transform.position;
            moveVector = moveDistance.normalized;                                   // ピースを動かす方向
            panelRotateStep = 180.0f / moveDistance.magnitude * MOVING_STEP;
            movingFlag = true;

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
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("SLIPEPlane")))
        {
            //Debug.Log(hit.point);
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }

    }

    //==============================
    //  SpawnPieces
    //==============================
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

    private void DrawSLIPEBoard()
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


    private void EndGame()
    {
        //if (isLightTurn)
        //    Debug.Log("Light team wins");
        //else
        //    Debug.Log("Dark team wins");

        UIManager.Instance.EndGameDisplay();

        foreach (GameObject go in activePiece)
            Destroy(go);

        isLightTurn = true;
        BoardHighlights.Instance.Hidehighlights();
        SpawnAllPieces();
    }
}
