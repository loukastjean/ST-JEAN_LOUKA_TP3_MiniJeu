using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    
    PlayerInputReader inputReader;
    EventSystem eventSystem;
    
    [SerializeField] Button btnPlay, btnSettings, btnQuit, btnStartGame;

    [SerializeField] GameObject MainMenu, CharacterSelectionMenu;
    
    [SerializeField] List<Button> buttons;

    bool canSelect;
    
    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        inputReader = GetComponent<PlayerInputReader>();
        
        canSelect = true;
        
        LoadMainMenu();

        SetButtonEvents();

        SetInputs();
    }

    // Update is called once per frame
    void Update()
    {
        Update_CharacterSelection();
    }

    void Update_CharacterSelection()
    {
        Button currentButton;
        
        try
        {
            GameObject currentGameObject = eventSystem.currentSelectedGameObject;
            currentButton = currentGameObject.GetComponent<Button>();
        }
        catch
        {
            return;
        }
        
        if (buttons.Contains(currentButton))
        {
            currentButton.GetComponent<Image>().color = Color.white;

            foreach (var button in buttons)
            {
                // Enlever le gris sur les bonhommes pas selectionnes
                if (button != currentButton)
                {
                    Debug.Log(button.name);
                    button.GetComponent<Image>().color = new Color(171,171,171);
                }
            }
        }
    }


    void SetButtonEvents()
    {
        // Ajouter un listener sur btnJouer
        btnPlay.onClick.AddListener(Play);
        // Ajouter un listener sur btnParametres
        btnSettings.onClick.AddListener(Settings);
        // Ajouter un listener sur btnQuitter
        btnQuit.onClick.AddListener(Quit);
        
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
    
    
}
