using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    // UI
    [SerializeField] private TMP_Text player1Damage, player2Damage, player1Lives, player2Lives, timer, gagnant;
    [SerializeField] private GameObject inGameMenu, pauseMenu, gameOverMenu;
    [SerializeField] private Button btnMainMenu;

    // Les deux personnages, pour les differencier
    private Personnage player1, player2;

    private float startingTime;

    // Update is called once per frame
    private void Update()
    {
        // Pendant que le jeu n'est pas commence, ne pas continuer dans l'update
        if (!inGameMenu.activeSelf)
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

    public void Creation()
    {
        startingTime = Time.time;
        
        inGameMenu.SetActive(true);
        
        // Set les joueurs dans le Menu
        SetPlayers();
        
        // Assigner les inputs pour que les deux puissent controler leur personnage
        InputSchemeAssigner.AssignSchemes();
        
        player1.Creation();
        player2.Creation();
        
        player1.lineRenderer = player1.GetComponent<LineRenderer>();
        player2.lineRenderer = player2.GetComponent<LineRenderer>();

        // Set the colors
        player1.lineRenderer.startColor = Color.blue;
        player1.lineRenderer.endColor = Color.blue;
        
        player2.lineRenderer.startColor = Color.red;
        player2.lineRenderer.endColor = Color.red;
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
        if ((player1.lives <= 0 || player2.lives <= 0) && !gameOverMenu.activeSelf)
        {
            //InGameMenu.SetActive(false); // Désactiver l'UI de jeu
            gameOverMenu.SetActive(true); // Activer l'UI de gameover

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
        // Si en pause et donc veut "unpause"
        if (Time.timeScale == 0)
        {
            pauseMenu.SetActive(false); // Desactiver l'UI de pause
            Debug.Log("Unpause");
            Time.timeScale = 1;
        }
        // Si il veut pauser
        else
        {
            pauseMenu.SetActive(true); // Activer l'UI de pause
            Debug.Log("Pause");
            Time.timeScale = 0;
        }
    }
}