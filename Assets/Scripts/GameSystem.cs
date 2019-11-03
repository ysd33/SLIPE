using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    private GameObject CurrentTeam;

    public void StartGame()
    {
        SceneManager.LoadScene("SLIPE");
        
    }

}
