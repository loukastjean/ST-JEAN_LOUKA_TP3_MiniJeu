using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    EventSystem eventSystem;
    
    [SerializeField] Button btnPlay, btnSettings, btnQuit, btnStartGame;

    [SerializeField] GameObject MainMenu, CharacterSelectionMenu, InGameMenu;
    
    [SerializeField] List<Button> player1Characters, player2Characters;
    
    List<Button> buttons;

    [SerializeField] List<GameObject> prefabs;
    
    // L'index des personnages selectionnes par les joueurs
    int player1IndexSelectedCharacter, player2IndexSelectedCharacter;
    
    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        
        LoadMainMenu();

        SetButtonEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void SetButtonEvents()
    {
        // Ajouter un listener sur btnJouer
        btnPlay.onClick.AddListener(Play);
        // Ajouter un listener sur btnParametres
        btnSettings.onClick.AddListener(Settings);
        // Ajouter un listener sur btnQuitter
        btnQuit.onClick.AddListener(Quit);

        foreach (Button btnCharacter in player1Characters)
        {
            btnCharacter.onClick.AddListener(SelectCharacter1);
        }
        
        foreach (Button btnCharacter in player2Characters)
        {
            btnCharacter.onClick.AddListener(SelectCharacter2);
        }
        
        btnStartGame.onClick.AddListener(StartGame);
    }

    void LoadCharacterSelectionMenu()
    {
        buttons = player1Characters.Concat(player2Characters).ToList();
        
        // Selectionner le joueur bleu par defaut
        buttons[0].Select();
        
        // Juste pour faire sur, desactiver playerselectionet enable MainMenu
        CharacterSelectionMenu.SetActive(true);
        MainMenu.SetActive(false);
        
        SetButtonColors();
    }
    
    void LoadMainMenu()
    {
        buttons = new List<Button> {btnPlay, btnSettings, btnQuit};
        
        buttons[0].Select();
        
        // Juste pour faire sur, desactiver playerselectionet enable MainMenu
        CharacterSelectionMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    void Menu_OnClicked()
    {
        if (CharacterSelectionMenu.activeSelf)
            LoadMainMenu();
    }


    void Play()
    {
        LoadCharacterSelectionMenu();
    }

    void Settings()
    {
        
    }

    void Quit()
    {
        Application.Quit();
    }

    void StartGame()
    {
        // Instancier joueur 1
        Personnage player1 = Instantiate(
            prefabs[player1IndexSelectedCharacter],
            new Vector2(-8, 1),
            Quaternion.identity
        ).GetComponent<Personnage>();
        
        // Instancier joueur 2
        Personnage player2 = Instantiate(
            prefabs[player2IndexSelectedCharacter],
            new Vector2(8, 1),
            Quaternion.identity
        ).GetComponent<Personnage>();

        player1.Creation();
        player2.Creation();
        
        // Assigner les inputs
        InputSchemeAssigner.AssignSchemes();
        
        InGameMenu.SetActive(true);
        CharacterSelectionMenu.SetActive(false);
        
        // Set les joueurs dans le UI In Game
        FindObjectOfType<InGameManager>().SetPlayers();
    }

    void SelectCharacter1()
    {
        Button btnCurrentlySelectedCharacter1 = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player1IndexSelectedCharacter = player1Characters.IndexOf(btnCurrentlySelectedCharacter1);
        
        SetButtonColors();
        
        Debug.Log("Player 1 selected: " + player1IndexSelectedCharacter);
    }
    
    void SelectCharacter2()
    {
        Button btnCurrentlySelectedCharacter2 = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player2IndexSelectedCharacter = player2Characters.IndexOf(btnCurrentlySelectedCharacter2);
        
        SetButtonColors();
        
        Debug.Log("Player 2 selected: " + player2IndexSelectedCharacter);
    }

    void SetButtonColors()
    {
        foreach (Button button in buttons)
        {
            if (button == player1Characters[player1IndexSelectedCharacter] ||
                button == player2Characters[player2IndexSelectedCharacter])
            {
                var colors = button.colors;
                colors.normalColor = new Color(1f, 1f , 1f);
                button.colors = colors;
            }
            else
            {
                var colors = button.colors;
                colors.normalColor = new Color(0.66f, 0.66f , 0.66f);
                button.colors = colors;
            }
        }
    }
}
