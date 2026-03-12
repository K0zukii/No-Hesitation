using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Variable")]
    private float score;
    public float speedMultiplier = 1f;
    private const float difficultyIncreaseRate = 0.01f;
    private const float maxMultiplier = 3f;
    public bool isPlaying = true;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuUI;
    public bool isPaused = false;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScore;

    [Header("Motor Audio")]
    [SerializeField] private AudioSource engineAudio;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 2.5f;
    [SerializeField] private float maxSpeedForAudio = 3f;

    private AudioSource musicSource;

    void Start()
    {
        Time.timeScale = 1;    //Make sure the game is running
        AudioListener.pause = false;  // Make sure the game audio is not paused

        if(MusicManager.Instance != null)
        {
            musicSource = MusicManager.Instance.GetComponent<AudioSource>();

            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    void Update()
    {
        //if player press escape, show the pause menu
        if (Input.GetKeyDown(KeyCode.Escape) && isPlaying)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (isPlaying && !isPaused )
        {
            score += 5 * Time.deltaTime;

            //Make the engine audio's pitch higher accordingly to the game speed !
            engineAudio.UnPause();
            float speedPercent = speedMultiplier / maxSpeedForAudio;
            speedPercent = Mathf.Clamp01(speedPercent);
            engineAudio.pitch = minPitch + ( speedPercent * (maxPitch - minPitch));
            
            //Verify that we've not reached the maximum difficulty
            if (speedMultiplier < maxMultiplier)
            {
                //The difficulty increases with each frame!
                speedMultiplier += difficultyIncreaseRate * Time.deltaTime;
            }

            scoreText.text = "Score : " + Mathf.RoundToInt(score).ToString();
        }
        else
        {
            engineAudio.Pause();
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        AudioListener.pause = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;     //Freezes time, movements and physics.
        isPaused = true;
        AudioListener.pause = true;
    }

    public void GameOver()
    {
        isPlaying = false;
        gameOverPanel.SetActive(true);
        finalScore.text = "Final Score : " + Mathf.RoundToInt(score).ToString();
        Time.timeScale = 0f;

        if (musicSource != null) musicSource.Stop();
        
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;

        AudioListener.pause = false;
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.Play();
        } 

        SceneManager.LoadScene(0);   //Load the main menu
    }

    public void RestartGame()
    {
        AudioListener.pause = false;

        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.Play();
        }

        //Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
