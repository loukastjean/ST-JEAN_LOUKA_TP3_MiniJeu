using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button btnPlay, btnSettings, btnQuit, btnStartGame;
    [SerializeField] private GameObject MainMenu, CharacterSelectionMenu, InGameMenu;
    [SerializeField] private List<Button> player1Characters, player2Characters;
    [SerializeField] private List<GameObject> prefabs;

    private List<Button> buttons;
    private EventSystem eventSystem;

    // L'index des boutons personnages selectionnes par les joueurs
    private int player1IndexSelectedCharacter, player2IndexSelectedCharacter;

    // Start is called before the first frame update
    private void Start()
    {
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
        CharacterSelectionMenu.SetActive(true);
        MainMenu.SetActive(false);

        SetButtonColors();
    }

    private void LoadMainMenu()
    {
        buttons = new List<Button> { btnPlay, btnSettings, btnQuit };

        buttons[0].Select();

        // desactiver characterselection et active MainMenu
        CharacterSelectionMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    private void Menu_OnClicked()
    {
        if (CharacterSelectionMenu.activeSelf)
            LoadMainMenu();
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
        var player1 = Instantiate(
            prefabs[player1IndexSelectedCharacter],
            new Vector2(-8, 1),
            Quaternion.identity
        ).GetComponent<Personnage>();

        // Instancier joueur 2
        var player2 = Instantiate(
            prefabs[player2IndexSelectedCharacter],
            new Vector2(8, 1),
            Quaternion.identity
        ).GetComponent<Personnage>();

        player1.Creation();
        player2.Creation();

        // Assigner les inputs pour que les deux puissent controler leur personnage
        InputSchemeAssigner.AssignSchemes();

        InGameMenu.SetActive(true);
        CharacterSelectionMenu.SetActive(false);

        // Add a LineRenderer component
        player1.lineRenderer = player1.GetComponent<LineRenderer>();

        // Set the color
        player1.lineRenderer.startColor = Color.blue;
        player1.lineRenderer.endColor = Color.blue;

        // Add a LineRenderer component
        player2.lineRenderer = player2.GetComponent<LineRenderer>();

        // Set the color
        player2.lineRenderer.startColor = Color.red;
        player2.lineRenderer.endColor = Color.red;

        // Set les joueurs dans le UI In Game
        FindObjectOfType<InGameManager>().SetPlayers();
    }

    private void SelectCharacter1()
    {
        var btnCurrentlySelectedCharacter1 = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player1IndexSelectedCharacter = player1Characters.IndexOf(btnCurrentlySelectedCharacter1);

        SetButtonColors();

        Debug.Log("Player 1 selected: " + player1IndexSelectedCharacter);
    }

    private void SelectCharacter2()
    {
        var btnCurrentlySelectedCharacter2 = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player2IndexSelectedCharacter = player2Characters.IndexOf(btnCurrentlySelectedCharacter2);

        SetButtonColors();

        Debug.Log("Player 2 selected: " + player2IndexSelectedCharacter);
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