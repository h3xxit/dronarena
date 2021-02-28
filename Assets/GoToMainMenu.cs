using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToMainMenu : MonoBehaviour
{
    public void mainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
