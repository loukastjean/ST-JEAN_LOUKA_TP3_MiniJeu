using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    // UI
    [SerializeField] private TMP_Text player1Damage, player2Damage, player1Lives, player2Lives, timer, gagnant;
    [SerializeField] private GameObject inGameUI, pauseUI, gameOverUI;
    [SerializeField] private Button btnMainMenu;

    // Les deux personnages, pour les differencier
    private Personnage player1, player2;

    private float startingTime, lastTimePaused;

    // Start is called before the first frame update
    private void Start()
    {
        startingTime = Time.time;
        lastTimePaused = -99f;
    }

    // Update is called once per frame
    private void Update()
    {
        // Pendant que le jeu n'est pas commence, ne pas continuer dans l'update
        if (!inGameUI.activeSelf)
            return;

        // Si aucun des personnages n'a perdu toutes ses vies
        // Si la partie n'a pas ete finie avant
        if (!VerifyGameOver())
        {
            Update_Timer();
            Update_Damage();
            Update_Lives();
        }
    }

    public void SetPlayers()
    {
        var players = FindObjectsOfType<Personnage>();

        // Le joueur 1 est toujours a gauche et le joueur 2 a droite
        if (players[0].transform.position.x < players[1].transform.position.x)
        {
            player1 = players[0];
            player2 = players[1];
        }
        else
        {
            player1 = players[1];
            player2 = players[0];
        }
    }

    private void Update_Timer()
    {
        // Calculer les minutes et secondes écoulées depuis le début du jeu
        var minutes = Mathf.FloorToInt((Time.time - startingTime) / 60);
        var secondes = Mathf.FloorToInt((Time.time - startingTime) % 60);

        // Afficher le timer dans le format MM:SS
        timer.text = $"{minutes:D2}:{secondes:D2}";
    }

    private void Update_Damage()
    {
        // Affiche les dommages des joueurs
        player1Damage.text = $"{player1.damage}%";
        player2Damage.text = $"{player2.damage}%";
    }

    private void Update_Lives()
    {
        // Affiche les vies des joueurs
        player1Lives.text = $"{player1.lives} vies";
        player2Lives.text = $"{player2.lives} vies";
    }

    private bool VerifyGameOver()
    {
        // Vérifier si un des 2 joueurs a perdu ET
        // que c'est la premiere fois que c'est gameover
        if ((player1.lives <= 0 || player2.lives <= 0) && !gameOverUI.activeSelf)
        {
            //InGameUI.SetActive(false); // Désactiver l'UI de jeu
            gameOverUI.SetActive(true); // Activer l'UI de gameover

            gagnant.text = "Gagnant: Joueur ";

            // Afficher le gagnant avec sa couleur de joueur
            if (player2.lives <= 0)
            {
                gagnant.text += "1";
                gagnant.color = new Color(0f, 0.2666667f, 0.9647059f);
            }
            else
            {
                gagnant.text += "2";
                gagnant.color = new Color(0.772549f, 0f, 0f);
            }


            // Update une derniere fois le UI, sinon on dirait que le joueur avait encore une vie
            Update_Damage();
            Update_Lives();

            // S'abonner au "event" d'appuyer sur le menu principal
            btnMainMenu.onClick.AddListener(ReloadScene);
        }

        // Si un des joueurs a perdu
        return player1.lives <= 0 || player2.lives <= 0;
    }


    private void ReloadScene()
    {
        // Retourne au menu principal en faisant un "reload" de scene
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }


    public void Pause()
    {
        Debug.Log(Time.timeScale); // DEBUG

        // Essaie de faire que les deux joueurs ne pausent pas en meme temps, keyboard fix
        if (Time.time < lastTimePaused + 0.2f)
            return;

        lastTimePaused = Time.time;

        // Si en pause et donc veut "unpause"
        if (Time.timeScale == 0)
        {
            pauseUI.SetActive(false); // Desactiver l'UI de pause
            Debug.Log("Unpause");
            Time.timeScale = 1;
        }
        // Si il veut pauser
        else
        {
            pauseUI.SetActive(false); // Activer l'UI de pause
            Debug.Log("Unpause");
            Time.timeScale = 0;
        }
    }
}