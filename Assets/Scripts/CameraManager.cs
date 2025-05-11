using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private GameObject inGameMenu;

    private readonly float cameraSpeed = 5f;
    private bool hasJustAppeared = true;

    private Personnage[] players;

    #region Unity Methods

    private void Start()
    {
        hasJustAppeared = true;
    }

    private void Update()
    {
        // Si la partie vient de commencer, trouver les joueurs et les assigner pour faire le mouvement entre les personnages
        if (hasJustAppeared && inGameMenu.activeSelf)
        {
            FindPlayers();
            hasJustAppeared = false;
        }
    }

    private void FixedUpdate()
    {
        // Verifie si la partie n'est pas encore commencée
        if (!inGameMenu.activeSelf)
            return;

        // Suit les joueurs pour viser entre eux
        FollowPlayers();
    }

    #endregion

    #region Camera Logic

    private void FindPlayers()
    {
        players = FindObjectsOfType<Personnage>();
    }

    private void FollowPlayers()
    {
        // Calcule le point milieu entre les deux joueurs
        Vector2 midpoint = (players[0].transform.position + players[1].transform.position) / 2f;
        // Donne la position qu'il faut que la caméra doit atteindre
        Vector3 targetPosition = new(midpoint.x, midpoint.y, transform.position.z);

        // Trouve le mouvement que la caméra doit faire pour arriver entre les joueurs
        var offset = targetPosition - transform.position;

        // Bouge en X pour arriver au point, et fait sur que la caméra ne dépasse pas ses limites
        if ((offset.x < 0 && transform.position.x > -15f) ||
            (offset.x > 0 && transform.position.x < 15f))
            transform.position += new Vector3(offset.x, 0, 0) * (cameraSpeed * Time.fixedDeltaTime);

        // Bouge en Y pour arriver au point, et fait sur que la caméra ne dépasse pas ses limites
        if ((offset.y < 0 && transform.position.y > -10f) ||
            (offset.y > 0 && transform.position.y < 5f))
            transform.position += new Vector3(0, offset.y, 0) * (cameraSpeed * Time.fixedDeltaTime);
    }

    #endregion
}