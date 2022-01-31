using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // UI
    GameObject uiTitle;
    GameObject controller;
    GameObject uiGameOver;
    GameObject uiPlay;

    // score
    int score;
    int scoreTop;
    Text textScore;
    Text textScoreTop;

    // stage
    int stage;
    readonly int stageMax = 3;
    Text textStage;
    public float enemyPowerFactor;
    readonly float enemyPowerFactorIncrement = 0.1f;

    // player
    PlayerMove player;

    public enum State
    {
        Title,
        Play,
        GameOver
    }

    public State state;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log(this + "GameObject is already exist!");
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        state = State.Title;

        // UI
        uiGameOver = GameObject.Find("UI GameOver");
        uiGameOver.SetActive(false);
        uiTitle = GameObject.Find("UI Title");
        uiTitle.SetActive(true);
        uiPlay = GameObject.Find("UI Play");
        textScore = GameObject.Find("Score Current").GetComponent<Text>();
        textScoreTop = GameObject.Find("Score Top").GetComponent<Text>();
        textStage = GameObject.Find("Stage Count").GetComponent<Text>();
        uiPlay.SetActive(false);
        controller = GameObject.Find("Controller");
        controller.SetActive(false);

        // score
        score = 0;
        //PlayerPrefs.SetInt("ScoreTop", 0);  // Top score reset
        scoreTop = PlayerPrefs.GetInt("ScoreTop", 0);
        textScoreTop.text = "top : " + scoreTop;

        // stage
        stage = 1;
        enemyPowerFactor = 1f;

        player = FindObjectOfType<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }    

    public void StartGame()
    {
        uiTitle.SetActive(false);
        uiPlay.SetActive(true);
        uiGameOver.SetActive(false);
        controller.SetActive(true);
        state = State.Play;

        enemyPowerFactor = 1f;

        player.Init();
        player.UpdateLevel();
        player.UpdateHpBar();
        player.UpdateExpBar();
        player.UpdateCoin();    
    }

    public void GameOver()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.audioGameOver);
        state = State.GameOver;
        uiGameOver.SetActive(true);
        controller.SetActive(false);

        // 가지고 있는 돈을 점수에 더함
        PlayerMove player = FindObjectOfType<PlayerMove>();
        AddScore(player.money);

        // 최고 점수 확인
        if (score > scoreTop)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.audioFanfare, 1f);
            scoreTop = score;
            PlayerPrefs.SetInt("ScoreTop", scoreTop);
            textScoreTop.text = "top : " + scoreTop;
        }
    }

    public void ContinueGame()
    {
        if (stage == 1)
        {
            ButtonRestart buttonRestart = FindObjectOfType<ButtonRestart>();
            buttonRestart.PressButton();
        }
        else
        {
            SceneManager.LoadScene("scene" + stage);

            // score = 0
            score = 0;
            textScore.text = "" + score;

            player.animator.SetTrigger("Revive");

            StartGame();

            InitController();
        }
    }

    void InitController()
    {
        ButtonAttack buttonAttack = FindObjectOfType<ButtonAttack>();
        buttonAttack.PressUp();

        Joystick joystick = FindObjectOfType<Joystick>();
        joystick.PointUp();
    }

    public void LoadTheNextStage()
    {
        if (stage < stageMax)
        {
            stage++;
        }
        else
        {
            stage = stageMax;
            enemyPowerFactor += enemyPowerFactorIncrement;
        }
        textStage.text = "stage " + stage;
        SceneManager.LoadScene("scene" + stage);
        InitController();
    }

    public void AddScore(int point)
    {
        score += point;
        textScore.text = "" + score;
    }

}
