//------------------------------//
//     Made by Agustin Ruscio   //
//------------------------------//


using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void BTN_Play() => SceneManager.LoadScene("Main");

    public void BTN_Quit() => Application.Quit();
}