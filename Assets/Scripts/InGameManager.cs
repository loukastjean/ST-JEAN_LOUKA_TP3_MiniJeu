using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // UI
    [SerializeField] TMP_Text player1Damage, player2Damage, player1Lives, player2Lives, timer, gagnant;
    [SerializeField] GameObject InGameUI, PauseUI,GameOverUI;
    
    Personnage player1, player2;

    MenuManager menu;

    bool justAppeared;
    
    // Start is called before the first frame update
    void Start()
    {
        justAppeared = false;
    }

    void SetPlayers()
    {
        Personnage[] players = FindObjectsOfType<Personnage>();

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

    // Update is called once per frame
    void Update()
    {
        if (!InGameUI.activeSelf)
            return;

        if (!justAppeared)
        {
            justAppeared = true;
            SetPlayers();
        }
        
        if (!VerifyGameOver())
        {
            Update_Timer();
            Update_Damage();
            Update_Lives();
        }
    }

    void Update_Timer()
    {
        // Calculer les minutes et secondes écoulées depuis le début du jeu
        var minutes = Mathf.FloorToInt(Time.time / 60);
        var secondes = Mathf.FloorToInt(Time.time % 60);

        // Afficher le timer dans le format MM:SS
        timer.text = $"{minutes:D2}:{secondes:D2}";
    }

    void Update_Damage()
    {
        player1Damage.text = $"{player1.damage}%";
        player2Damage.text = $"{player2.damage}%";
    }

    void Update_Lives()
    {
        player1Lives.text = $"{player1.lives} vies";
        player2Lives.text = $"{player2.lives} vies";
    }

    bool VerifyGameOver()
    {
        // Vérifier si un des 2 joueurs a perdu
        if (player1.lives <= 0 || player2.lives <= 0)
        {
            //InGameUI.SetActive(false); // Désactiver l'UI en jeu
            GameOverUI.SetActive(true); // Activer l'UI de fin de jeu
            
            gagnant.text = "Gagnant: Joueur de ";

            // Afficher le gagnant et le perdant pour chaque équipe
            if (player1.lives <= 0)
                gagnant.text += "Droite";
            else
                gagnant.text += "Gauche";

            Update_Damage();
            Update_Lives();
        }

        return player1.lives <= 0 || player2.lives <= 0;
    }


    public void Pause()
    {
        Debug.Log(Time.timeScale);
        
        
        // Si en pause
        if (Time.timeScale == 0)
        {
            PauseUI.SetActive(true); // Activer l'UI de pause
            Debug.Log("Pause");
            Time.timeScale = 1;
        }
        else
        {
            PauseUI.SetActive(false); // Desactiver l'UI de pause
            Debug.Log("Unpause");
            Time.timeScale = 0;
        }
    }
}
