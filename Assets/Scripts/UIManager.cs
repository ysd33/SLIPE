using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance { set; get; }

    public GameObject lightTeamWinImage;
    public GameObject darkTeamWinImage;

    private void Start()
    {
        Instance = this;

        lightTeamWinImage = GameObject.Find("LightTeamWin");
        darkTeamWinImage = GameObject.Find("DarkTeamWin");
    }

    public void StartGameSetup()
    {
        lightTeamWinImage.GetComponent<Image>().enabled = false;
        darkTeamWinImage.GetComponent<Image>().enabled = false;
    }

    public void EndGameDisplay()
    {
        if (BoardManager.Instance.isLightTurn)
            lightTeamWinImage.GetComponent<Image>().enabled = true;
        else
            darkTeamWinImage.GetComponent<Image>().enabled = true;

    }
}
