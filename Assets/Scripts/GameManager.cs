using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public HashSet<IBoid> allBoids = new HashSet<IBoid>();

    public static GameManager instance;

    private List<AiAgent> _redTeamList = new List<AiAgent>();
    private List<AiAgent> _blueTeamList = new List<AiAgent>();

    [SerializeField]
    private TextMeshProUGUI _winerTeam;

    [SerializeField]
    private TextMeshProUGUI _winCondition;

    [SerializeField]
    private GameObject _panel;

    private bool _simulation = true;

    [SerializeField]
    private AudioClip[] _winMusic;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();

        EventManager.Subscribe(EventEnum.LeaderDeath, LeaderDeathcondition);
    }

    public void AddBoid(IBoid b)
    {
        if (!allBoids.Contains(b))
            allBoids.Add(b);
    }

    public void AddAgent(TeamEnum team, AiAgent agent)
    {
        switch (team)
        {
            case TeamEnum.RedTeam:
                _redTeamList.Add(agent);
                break;
            case TeamEnum.BlueTeam:
                _blueTeamList.Add(agent);
                break;
        }
    }

    public void RemoveAgent(TeamEnum team, AiAgent agent)
    {
        if(team == TeamEnum.RedTeam)
        {
            if (_redTeamList.Contains(agent))
                _redTeamList.Remove(agent);
        }
        else
        {
            if (_blueTeamList.Contains(agent))
                _blueTeamList.Remove(agent);
        }

        if(_blueTeamList.Count == 0 || _redTeamList.Count == 0)
            Win();
    }

    private void LeaderDeathcondition(params object[] parameters)
    {
        _simulation = false;

        int randomMusicIndex = Random.Range(0, _winMusic.Length - 1);

        _audioSource.clip = _winMusic[randomMusicIndex];

        _audioSource.Play();

        _winCondition.text = "Leader defeated";

        if ((TeamEnum)parameters[0] == TeamEnum.RedTeam)
        {
            _winerTeam.text = "Blue team wins";
            _panel.SetActive(true);

            EventManager.Trigger(EventEnum.Win, TeamEnum.BlueTeam);
        }
        else
        {
            _winerTeam.text = "Red team wins";
            _panel.SetActive(true);

            EventManager.Trigger(EventEnum.Win, TeamEnum.RedTeam);
        }
    }


    private void Win()
    {
        _simulation = false;

        int randomMusicIndex = Random.Range(0, _winMusic.Length-1);

        _audioSource.clip = _winMusic[randomMusicIndex];

        _audioSource.Play();

        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.5f);

        _winCondition.text = "All team defeated";

        if (_redTeamList.Count <= 0)
        {
            _winerTeam.text = "Blue team wins";
            _panel.SetActive(true);

            EventManager.Trigger(EventEnum.Win, TeamEnum.BlueTeam);
        }
        else if (_blueTeamList.Count <= 0)
        {
            _winerTeam.text = "Red team wins";
            _panel.SetActive(true);

            EventManager.Trigger(EventEnum.Win, TeamEnum.RedTeam);
        }
    }

    private void OnDestroy()
    {
        EventManager.Unsubscribe(EventEnum.LeaderDeath, LeaderDeathcondition);

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