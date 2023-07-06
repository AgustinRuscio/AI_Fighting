using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _pausePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void BTN_Resume()
    {
        _pausePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
