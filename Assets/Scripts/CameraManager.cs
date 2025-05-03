using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject InGameMenu;

    // Les deux personnages
    private Personnage[] players;
    private float speed;
    
    private bool justAppeared;

    // Start is called before the first frame update
    private void Start()
    {
        speed = 5f;
        justAppeared = true;
    }

    private void FixedUpdate()
    {
        // Si on est pas dans une partie, ne pas faire bouger la camera
        if (!InGameMenu.activeSelf)
            return;

        MoveCamera();
    }

    private void Update()
    {
        // Pour chercher les personnages quand la partie commence
        if (justAppeared && InGameMenu.activeSelf)
        {
            FindPlayers();
            justAppeared = false;
        }
    }

    public void FindPlayers()
    {
        players = FindObjectsOfType<Personnage>();
    }

    private void MoveCamera()
    {
        // Vecteur entre les deux personnages
        Vector2 playersPositionDifference = new(players[0].transform.position.x - players[1].transform.position.x,
            players[0].transform.position.y - players[1].transform.position.y);
        
        // Point milieu entre les deux personnages
        playersPositionDifference /= 2;
        
        // No clue honnetement
        Vector3 destination = new(playersPositionDifference.x + players[1].transform.position.x,
            playersPositionDifference.y + players[1].transform.position.y);
        
        
        Vector3 mouvementCamera = new(destination.x - transform.position.x, destination.y - transform.position.y);

        // Pas depasser limites en X
        if ((mouvementCamera.x < 0 && transform.position.x > -15) ||
            (mouvementCamera.x > 0 && transform.position.x < 15))
            transform.position += new Vector3(mouvementCamera.x, 0, 0) * (speed * Time.fixedDeltaTime);

        // Pas depasser limites en Y
        if ((mouvementCamera.y < 0 && transform.position.y > -10) ||
            (mouvementCamera.y > 0 && transform.position.y < 5))
            transform.position += new Vector3(0, mouvementCamera.y, 0) * (speed * Time.fixedDeltaTime);
    }
}