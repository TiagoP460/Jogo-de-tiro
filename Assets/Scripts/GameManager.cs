using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text ammoText;
    public TMP_Text gameOverText;
    public GameObject restartButton;

    [Header("Configurações")]
    public int maxAmmo = 30;
    public float reloadTime = 1.5f;
    public float startTime = 60f;
    public int scoreToWin = 4;

    private int score = 0;
    private int currentAmmo;
    private float currentTime;
    private bool gameOver = false;
    private bool isReloading = false;

    public bool IsGameOver
    {
        get { return gameOver; }
    }

    public bool IsReloading
    {
        get { return isReloading; }
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

        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }

        Time.timeScale = 1f;
        UpdateUI();
    }

    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.F))
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
        return !gameOver && !isReloading && currentAmmo > 0;
    }

    public bool TryUseAmmo()
    {
        if (!CanShoot())
        {
            return false;
        }

        currentAmmo--;
        UpdateUI();

        return true;
    }

    public void StartReload()
    {
        if (gameOver)
        {
            return;
        }

        if (isReloading)
        {
            return;
        }

        if (currentAmmo >= maxAmmo)
        {
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        UpdateUI();

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        UpdateUI();
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
            string ammoInfo;

            if (isReloading)
            {
                ammoInfo = "Ammo: Recarregando...";
            }
            else
            {
                ammoInfo = "Ammo: " + currentAmmo + "/" + maxAmmo;

                if (currentAmmo <= 0)
                {
                    ammoInfo += "\nPressione R para recarregar";
                }
            }

            ammoText.text = ammoInfo + "\nTime: " + Mathf.CeilToInt(currentTime) + "s";
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
            gameOverText.text = message + "\n<size=32>Pressione F ou clique no botão para jogar novamente</size>";
            gameOverText.alignment = TextAlignmentOptions.Center;
        }

        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(message);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}