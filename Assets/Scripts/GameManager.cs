using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text ammoText;
    public TMP_Text gameOverText;

    [Header("Configurações")]
    public int maxAmmo = 30;
    public float startTime = 60f;
    public int scoreToWin = 4;

    private int score = 0;
    private int currentAmmo;
    private float currentTime;
    private bool gameOver = false;

    public bool IsGameOver
    {
        get { return gameOver; }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        currentTime = startTime;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "";
        }

        Time.timeScale = 1f;
        UpdateUI();
    }

    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }

            return;
        }

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame("GAME OVER");
        }

        UpdateUI();
    }

    public bool CanShoot()
    {
        return !gameOver && currentAmmo > 0;
    }

    public bool TryUseAmmo()
    {
        if (!CanShoot())
        {
            return false;
        }

        currentAmmo--;

        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            UpdateUI();
            EndGame("GAME OVER");
        }

        UpdateUI();
        return true;
    }

    public void AddScore(int amount)
    {
        if (gameOver)
        {
            return;
        }

        score += amount;

        if (score >= scoreToWin)
        {
            score = scoreToWin;
            UpdateUI();
            EndGame("Você Ganhou!");
            return;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + currentAmmo + "/" + maxAmmo + "\nTime: " + Mathf.CeilToInt(currentTime) + "s";
        }
    }

    void EndGame(string message)
    {
        if (gameOver)
        {
            return;
        }

        gameOver = true;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = message + "\n<size=32>Pressione R para jogar novamente</size>";
            gameOverText.alignment = TextAlignmentOptions.Center;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(message);
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}