using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button btnPlay, btnSettings, btnQuit, btnStartGame;
    [SerializeField] private GameObject mainMenu, characterSelectionMenu, inGameMenu;
    [SerializeField] private List<Button> player1Characters, player2Characters;
    [SerializeField] private List<GameObject> prefabs;

    private List<Button> buttons;
    private EventSystem eventSystem;

    // L'index des boutons personnages selectionnes par les joueurs
    private int player1IndexSelectedCharacter, player2IndexSelectedCharacter;

    // Start is called before the first frame update
    private void Start()
    {
        // Pour detecter le bouton selectionne en ce moment
        eventSystem = EventSystem.current;

        LoadMainMenu();

        SetButtonEvents();
    }


    private void SetButtonEvents()
    {
        // Ajouter un listener sur btnJouer
        btnPlay.onClick.AddListener(Play);
        // Ajouter un listener sur btnParametres
        btnSettings.onClick.AddListener(Settings);
        // Ajouter un listener sur btnQuitter
        btnQuit.onClick.AddListener(Quit);

        // S'abonner a l'evenement de clic de chaque bouton
        foreach (var btnCharacter in player1Characters) btnCharacter.onClick.AddListener(SelectCharacter1);
        foreach (var btnCharacter in player2Characters) btnCharacter.onClick.AddListener(SelectCharacter2);

        btnStartGame.onClick.AddListener(StartGame);
    }

    private void LoadCharacterSelectionMenu()
    {
        // La liste de boutons comporte les boutons des deux joueurs
        buttons = player1Characters.Concat(player2Characters).ToList();

        // Selectionner le joueur bleu par defaut
        buttons[0].Select();

        // active characterselection et desactive MainMenu
        characterSelectionMenu.SetActive(true);
        mainMenu.SetActive(false);

        SetButtonColors();
    }

    private void LoadMainMenu()
    {
        buttons = new List<Button> { btnPlay, btnSettings, btnQuit };

        // Par defaut selectionne Jouer
        buttons[0].Select();

        // desactiver characterselection et active MainMenu
        characterSelectionMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void Play()
    {
        LoadCharacterSelectionMenu();
    }

    private void Settings()
    {
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void StartGame()
    {
        // Instancier joueur 1
        Instantiate(
            prefabs[player1IndexSelectedCharacter],
            new Vector2(-8, 1),
            Quaternion.identity
        ).GetComponent<Personnage>();

        // Instancier joueur 2
        Instantiate(
            prefabs[player2IndexSelectedCharacter],
            new Vector2(8, 1),
            Quaternion.identity
        ).GetComponent<Personnage>();
        
        // Cree le UI de la partie
        FindObjectOfType<InGameManager>().Creation();
        
        characterSelectionMenu.SetActive(false);
    }

    private void SelectCharacter1()
    {
        var btnCurrentlySelectedCharacter1 = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player1IndexSelectedCharacter = player1Characters.IndexOf(btnCurrentlySelectedCharacter1);

        SetButtonColors();
    }

    private void SelectCharacter2()
    {
        var btnCurrentlySelectedCharacter2 = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player2IndexSelectedCharacter = player2Characters.IndexOf(btnCurrentlySelectedCharacter2);

        SetButtonColors();
    }

    private void SetButtonColors()
    {
        foreach (var button in buttons)
            
            // Si le bouton est selectionne, l'afficher pleinement
            if (button == player1Characters[player1IndexSelectedCharacter] ||
                button == player2Characters[player2IndexSelectedCharacter])
            {
                var colors = button.colors;
                colors.normalColor = new Color(1f, 1f, 1f);
                button.colors = colors;
            }
            // Si il n'est pas selectionne, le griser
            else
            {
                var colors = button.colors;
                colors.normalColor = new Color(0.66f, 0.66f, 0.66f);
                button.colors = colors;
            }
    }
}