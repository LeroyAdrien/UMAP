using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public void doExitGame()
    {
        Application.Quit();
        Debug.Log("Game has quit");
    }
}
