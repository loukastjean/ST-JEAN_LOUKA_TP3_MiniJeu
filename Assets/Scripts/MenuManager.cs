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
    PlayerInputReader inputReader;
    
    [SerializeField] Button btnPlay, btnSettings, btnQuit, btnStartGame;

    [SerializeField] GameObject MainMenu, CharacterSelectionMenu;
    
    [SerializeField] List<Button> player1Characters, player2Characters;
    
    List<Button> buttons;
    
    // L'index des personnages selectionnes par les joueurs
    int player1IndexSelectedCharacter, player2IndexSelectedCharacter;
    
    // Start is called before the first frame update
    void Start()
    {
        inputReader = GetComponent<PlayerInputReader>();
        eventSystem = EventSystem.current;
        
        buttons = player1Characters.Concat(player2Characters).ToList();
        
        LoadMainMenu();

        SetButtonEvents();

        SetInputs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Update_CharacterSelection()
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


    void SetInputs()
    {
        // Menu (Pause)
        inputReader.Menu.callback += Menu_OnClicked;
    }

    void LoadCharacterSelectionMenu()
    {
        // Selectionner le joueur bleu par defaut
        buttons[0].Select();
        
        // Juste pour faire sur, desactiver playerselectionet enable MainMenu
        CharacterSelectionMenu.SetActive(true);
        MainMenu.SetActive(false);
    }
    
    void LoadMainMenu()
    {
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
        SceneManager.LoadScene("MiniJeu");
    }

    void SelectCharacter1()
    {
        Button btnCurrentlySelectedCharacter = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player1IndexSelectedCharacter = player1Characters.IndexOf(btnCurrentlySelectedCharacter);
        
        ResetButtonColors();
        
        btnCurrentlySelectedCharacter.image.color = Color.white;
        
        Debug.Log("Player 1 selected: " + player1IndexSelectedCharacter);
    }
    
    void SelectCharacter2()
    {
        Button btnCurrentlySelectedCharacter = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player2IndexSelectedCharacter = player2Characters.IndexOf(btnCurrentlySelectedCharacter);

        ResetButtonColors();
        
        btnCurrentlySelectedCharacter.image.color = Color.white;
        
        Debug.Log("Player 2 selected: " + player2IndexSelectedCharacter);
    }

    void ResetButtonColors()
    {
        foreach (Button button in buttons)
        {
            //button.image.color = new Color(171,171,171);
            button.image.color = new Color(0, 0, 0);
            //button.image.color = new Color(0, 0, 0);
            Debug.Log("Vient de remettre bouton a gris");
        }
    }
    
}
