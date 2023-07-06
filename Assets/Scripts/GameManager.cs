using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public HashSet<IBoid> allBoids = new HashSet<IBoid>();

    public static GameManager instance;

    [SerializeField]
    private int _blueTeam;

    [SerializeField]
    private int _redTeam;

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private GameObject _panel;

    private bool _simulation = true;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddBoid(IBoid b)
    {
        if (!allBoids.Contains(b))
            allBoids.Add(b);
    }

    public void RemoveAgent(TeamEnum team)
    {
        switch (team)
        {
            case TeamEnum.RedTeam:
                _redTeam--;
                break;
            case TeamEnum.BlueTeam:
                _blueTeam--;
                break;
        }

        if(_redTeam <= 0 || _blueTeam <= 0)
        {
            _simulation = false;

            Time.timeScale = 0;

            _panel.SetActive(true);

            if (_redTeam <= 0)
                _text.text = "Blue team wins";
            else if (_blueTeam <= 0)
                _text.text = "Red team wins";
        }

    }

    public void BTN_Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BTN_Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public bool SimulationOn() => _simulation;
}
