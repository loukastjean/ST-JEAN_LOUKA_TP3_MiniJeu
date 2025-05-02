using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

// Attention, ce système ne peut pas gérer plus de 2 joueurs sur le même clavier.

public static class InputSchemeAssigner
{
    // Nombre de joueurs
    static int playerCount = 0;

    // Nombre de gamepads
    static int gamePadCount = 0;

    // La classe a été initialisé?
    static bool isInitialized = false;

    /// <summary>
    /// Assigne un appareil à chacun des joueurs (manettes ou clavier)
    /// </summary>
    /// <param name="invertPlayers">Inverse le joueur de gauche et de droite</param>
    public static void AssignSchemes(bool invertPlayers = false)
    {
        if (isInitialized)
        { 
            Debug.LogWarning("Le InputSchemeAssigner a déjà été initialisé!");
            //return;
        }

        // Assigner les valeurs
        playerCount = PlayerInput.all.Count;
        gamePadCount = Gamepad.all.Count;

        Debug.Log($"Partie de {playerCount} joueurs avec {gamePadCount} GamePads");

        AssignScheme(invertPlayers);
    }

    // Assigne les scheme aux différents joueurs
    static void AssignScheme(bool invertPlayers = false)
    {
        // Nombre de joueurs restants à assigner un contrôleur
        int remainingPlayers = playerCount;

        // Empêcher l'assignation s'il y a 0 joueurs
        if (remainingPlayers == 0)
        {
            Debug.LogError("Il n'y a aucun joueur dans la scène");
            return;
        }

        // Assigner les gamepads
        for (int i = 0; i < gamePadCount; i++)
        {
            PlayerInput.all[remainingPlayers - 1].SwitchCurrentControlScheme("Gamepad", Gamepad.all[i]);

            remainingPlayers--;

            if (remainingPlayers == 0)
            {
                // Initialisation terminée
                isInitialized = true;
                return;
            }
        }

        string schemeLeft =     "Keyboard Left";
        string schemeRight =    "Keyboard Right";

        // Assigner les côtés de clavier        
        for (int i = remainingPlayers; i > 0; i--)
        {
            switch (i)
            {
                case 1:
                    if (!invertPlayers)
                        PlayerInput.all[remainingPlayers - 1].SwitchCurrentControlScheme(schemeLeft, Keyboard.current);
                    else
                        PlayerInput.all[remainingPlayers - 1].SwitchCurrentControlScheme(schemeRight, Keyboard.current);
                    break;
                case 2:
                    if (!invertPlayers)
                        PlayerInput.all[remainingPlayers - 1].SwitchCurrentControlScheme(schemeRight, Keyboard.current);
                    else
                        PlayerInput.all[remainingPlayers - 1].SwitchCurrentControlScheme(schemeLeft, Keyboard.current);
                    break;
                default:
                    break;
            }
            remainingPlayers--;
        }
        

        // Initialisation terminée
        isInitialized = true;
    }

    public static void SwapPlayers()
    {
        if (!isInitialized)
        {
            Debug.LogError("Pour échanger les joueurs, le InputSchemeAssigner doit avoir été initialisé");
            return;
        }

        if (playerCount != 2)
        {
            Debug.LogWarning($"On ne peut changer de personnage que lorsqu'il y a 2 joueurs seulement! (Il y en a présentement {playerCount})");
            return;
        }

        // Conserver le scheme et device du joueur 1
        string scheme1 = PlayerInput.all[0].currentControlScheme;
        InputDevice device1 = PlayerInput.all[0].devices[0];

        // Conserver le scheme et device du joueur 2
        string scheme2 = PlayerInput.all[1].currentControlScheme;
        InputDevice device2 = PlayerInput.all[1].devices[0];

        // Échanger les valeurs
        PlayerInput.all[0].SwitchCurrentControlScheme(scheme2, device2);
        PlayerInput.all[1].SwitchCurrentControlScheme(scheme1, device1);

        Debug.Log("Swap des deux joueurs complété!");
    }
}