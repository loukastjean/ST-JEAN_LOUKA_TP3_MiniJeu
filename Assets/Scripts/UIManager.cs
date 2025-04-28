using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // UI
    [SerializeField] TMP_Text player1Damage, player2Damage, player1Lives, player2Lives, timer;
    
    Personnage player1, player2;
    
    // Start is called before the first frame update
    void Start()
    {
        SetPlayers();
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
        Update_Timer();
        Update_Damage();
        Update_Lives();
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
    
    
}
