using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    #region Serialized Fields

    [Header("Buttons")]
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnInfos;
    [SerializeField] private Button btnQuit;
    [SerializeField] private Button btnStartGame;
    [SerializeField] private Button btnCloseInfos;

    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject characterSelectionMenu;
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private GameObject infosMenu;

    [Header("Character Selection")]
    [SerializeField] private List<Button> player1Characters;
    [SerializeField] private List<Button> player2Characters;
    [SerializeField] private List<GameObject> prefabs;

    #endregion

    #region Private Fields

    private List<Button> allCharacterButtons;
    private EventSystem eventSystem;

    private int player1SelectedIndex;
    private int player2SelectedIndex;

    #endregion

    #region Unity Methods

    private void Start()
    {
        // Eventsystem pour savoir sur quel bouton est "highlighted"
        eventSystem = EventSystem.current;

        LoadMainMenu();
        SetButtonCallbacks();
    }

    #endregion

    #region Menu Loading

    private void LoadMainMenu()
    {
        mainMenu.SetActive(true);
        characterSelectionMenu.SetActive(false);
        btnPlay.Select();
    }

    private void LoadCharacterSelectionMenu()
    {
        allCharacterButtons = player1Characters.Concat(player2Characters).ToList();

        mainMenu.SetActive(false);
        characterSelectionMenu.SetActive(true);
        // Selectionne le character bleu du joueur 1
        allCharacterButtons[0].Select();

        UpdateCharacterButtonColors();
    }

    #endregion

    #region Button Setup

    // S'abonne au onclick event de tous les boutons du menu
    private void SetButtonCallbacks()
    {
        btnPlay.onClick.AddListener(LoadCharacterSelectionMenu);
        btnInfos.onClick.AddListener(OpenInfos);
        btnQuit.onClick.AddListener(QuitGame);
        btnCloseInfos.onClick.AddListener(CloseInfos);
        btnStartGame.onClick.AddListener(StartGame);

        foreach (var btn in player1Characters)
            btn.onClick.AddListener(SelectCharacter1);

        foreach (var btn in player2Characters)
            btn.onClick.AddListener(SelectCharacter2);
    }

    #endregion

    #region Button Actions

    private void OpenInfos()
    {
        infosMenu.SetActive(true);
        SetMainMenuButtonsInteractable(false);
        btnCloseInfos.Select();
    }

    private void CloseInfos()
    {
        infosMenu.SetActive(false);
        SetMainMenuButtonsInteractable(true);
        btnInfos.Select();
    }

    // Si les boutons du mainMenu doivent être interactable (selon si le menu info est ouvert)
    private void SetMainMenuButtonsInteractable(bool state)
    {
        btnPlay.interactable = state;
        btnInfos.interactable = state;
        btnQuit.interactable = state;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void StartGame()
    {
        // Instantiate Player 1
        Personnage player1 = Instantiate(prefabs[player1SelectedIndex], new Vector2(-8, 1), Quaternion.identity)
            .GetComponent<Personnage>();

        // Instantiate Player 2
        Personnage player2 = Instantiate(prefabs[player2SelectedIndex], new Vector2(8, 1), Quaternion.identity)
            .GetComponent<Personnage>();

        // Initialize game UI
        FindObjectOfType<InGameManager>().Creation(player1, player2);

        characterSelectionMenu.SetActive(false);
    }

    #endregion

    #region Character Selection

    private void SelectCharacter1()
    {
        var selectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player1SelectedIndex = player1Characters.IndexOf(selectedButton);
        UpdateCharacterButtonColors();
    }

    private void SelectCharacter2()
    {
        var selectedButton = eventSystem.currentSelectedGameObject.GetComponent<Button>();
        player2SelectedIndex = player2Characters.IndexOf(selectedButton);
        UpdateCharacterButtonColors();
    }

    private void UpdateCharacterButtonColors()
    {
        foreach (var button in allCharacterButtons)
        {
            var colors = button.colors;

            bool isSelected =
                button == player1Characters[player1SelectedIndex] ||
                button == player2Characters[player2SelectedIndex];

            // Si le character est selectionné, le mettre blanc, sinon grisé
            colors.normalColor = isSelected
                ? Color.white
                : new Color(0.66f, 0.66f, 0.66f);

            button.colors = colors;
        }
    }

    #endregion
}