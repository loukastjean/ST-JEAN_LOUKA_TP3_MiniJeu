using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    #region Unity Methods

    private void Update()
    {
        // Si la partie n'a pas encore commencée
        if (!inGameMenu.activeSelf || gameOverMenu.activeSelf)
            return;

        // Si la partie est terminée
        if (!CheckGameOver())
        {
            UpdateTimer();
            UpdateDamageUI();
            UpdateLivesUI();
        }
    }

    #endregion

    #region Serialized Fields

    [Header("UI Elements")] [SerializeField]
    private TMP_Text player1Damage;

    [SerializeField] private TMP_Text player2Damage;
    [SerializeField] private TMP_Text player1Lives;
    [SerializeField] private TMP_Text player2Lives;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TMP_Text winnerText;

    [Header("Menus")] [SerializeField] private GameObject inGameMenu;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;

    [SerializeField] private Button btnMainMenu;

    #endregion

    #region Private Fields

    private Personnage player1;
    private Personnage player2;
    private float gameStartTime;

    #endregion

    #region Initialization

    // Set les informations au début du match
    public void Creation(Personnage _player1, Personnage _player2)
    {
        gameStartTime = Time.time;

        inGameMenu.SetActive(true);

        player1 = _player1;
        player2 = _player2;

        InputSchemeAssigner.AssignSchemes();

        player1.Creation();
        player2.Creation();

        // Fait que le joueur 1, vu qu'il est a gauche, vise a droite
        player1.aim = Vector2.right;

        // Fait que le joueur 2, vu qu'il est a droite, vise a gauche et est inversé
        player2.aim = Vector2.left;
        player2.sr.flipX = true;

        AssignLineRenderers();
    }

    private void AssignLineRenderers()
    {
        player1.lineRenderer = player1.GetComponent<LineRenderer>();
        player2.lineRenderer = player2.GetComponent<LineRenderer>();

        // Assigne la couleur de la ligne de visée aux joueurs en fonction de leur coté
        player1.lineRenderer.startColor = Color.blue;
        player1.lineRenderer.endColor = Color.blue;

        player2.lineRenderer.startColor = Color.red;
        player2.lineRenderer.endColor = Color.red;
    }

    #endregion

    #region UI Updates

    // Affiche et update le timer
    private void UpdateTimer()
    {
        var elapsed = Time.time - gameStartTime;
        var minutes = Mathf.FloorToInt(elapsed / 60);
        var seconds = Mathf.FloorToInt(elapsed % 60);
        timer.text = $"{minutes:D2}:{seconds:D2}";
    }

    // Update l'information sur le UI au sujet des dégats subis
    private void UpdateDamageUI()
    {
        player1Damage.text = $"{player1.damage}%";
        player2Damage.text = $"{player2.damage}%";
    }

    // Update l'information sur le UI au sujet des vies perdues
    private void UpdateLivesUI()
    {
        player1Lives.text = $"{player1.lives} vies";
        player2Lives.text = $"{player2.lives} vies";
    }

    #endregion

    #region Game State

    // Verifie si c'est la fin de la partie
    private bool CheckGameOver()
    {
        // Si c'est la premiere frame qu'un joueur est a 0 vies
        if (player1.lives <= 0 || player2.lives <= 0)
        {
            ShowGameOverMenu();
            return true;
        }

        return false;
    }

    private void ShowGameOverMenu()
    {
        gameOverMenu.SetActive(true);

        // Pour etre sur que les vies et dommages sur le UI montrent 0 vies 0 dommages, pas ce qu'il y avait juste avant le gameover
        UpdateDamageUI();
        UpdateLivesUI();

        winnerText.text = "Gagnant: Joueur ";

        if (player2.lives <= 0)
        {
            winnerText.text += "1";
            winnerText.color = new Color(0f, 0.27f, 0.96f);
        }
        else
        {
            winnerText.text += "2";
            winnerText.color = new Color(0.77f, 0f, 0f);
        }

        btnMainMenu.onClick.AddListener(ReloadScene);

        btnMainMenu.Select();
    }

    private void ReloadScene()
    {
        // Empeche les joueurs de bouger apres la fin de la partie, afin qu'ils puissent selectionner le bouton Menu Principal
        player1.RemoveInputs();
        player2.RemoveInputs();

        // Rejoue la scene du jeu pour revenir au menu principal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause()
    {
        // Si le jeu est pausé, unpause, sinon pause
        var isPaused = Time.timeScale == 0;
        pauseMenu.SetActive(!isPaused);
        Time.timeScale = isPaused ? 1 : 0;
    }

    #endregion
}