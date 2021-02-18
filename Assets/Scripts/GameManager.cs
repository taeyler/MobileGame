using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver 
    }

    int score = 0;
    bool gameOver = true;

    public bool GameOver { get { return gameOver; } }

   void Awake()
    {
        Instance = this;
        
    }

    void OnEnable()                                             //subscribe to methods
    {
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;
    }

    void OnDisable()                                             //unsubscribe from methods
    {
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
    }

    void OnCountdownFinished()                                 //finishes countdown, starts game
    {
        SetPageState(PageState.None);
        OnGameStarted();
        score = 0;
        gameOver = false;
    }

    void OnPlayerScored()                                       //updates player score
    {
        score++;
        scoreText.text = score.ToString();
    }

    void OnPlayerDied()                                         //Player Dies Display 
    {
		gameOver = true;
		//savedScore = 0;                                       //un-comment to set highscore back to zero
		int savedScore = PlayerPrefs.GetInt("HighScore");       //set highscore to savedScore
        PlayerPrefs.SetInt("gameOverScore", score);             //set gameOverScore to score
        scoreText.text = " ";                                   //hide initial score for final display

        if (score > savedScore)                                 //calculates high score
        {
            PlayerPrefs.SetInt("HighScore", score);             //sets high score
        }

        SetPageState(PageState.GameOver);                       //open up page that says game over
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            
        }
    }

    public void ConfirmGameOver()           //replay button is hit
    {
        SetPageState(PageState.Start);      //start over again
        OnGameOverConfirmed();              //event sent to tap controller, game object resets
        scoreText.text = "";               //setting score back to zero

    }

    public void StartGame()                 //play button is hit
    {
         SetPageState(PageState.Countdown); //send to countdown page
    }
}
