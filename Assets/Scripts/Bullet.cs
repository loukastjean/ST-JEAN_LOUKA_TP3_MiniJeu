using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Personnage creator;
    private Vector2 movement;

    private SpriteRenderer rend;
    private float speed, damage;

    // Start is called before the first frame update
    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Si le projectile entre en contact avec l'ennemi
        if (TouchedEnemy())
            // Il se detruit et inflige du dommage a l'ennemi
            OnContact();

        // Si il est parti plus loin que les bordures de l'arene
        if (ExitedField())
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        // Bouge le projectile a une vitesse fixe
        Move();
    }

    // Set les informations au sujet du projectile
    public void SetAttributes(Vector2 destination, Vector2 position, Personnage personnage)
    {
        speed = 40f;
        damage = 10f;

        // Son point de depart
        transform.position = position;

        // Son mouvement
        movement = destination.normalized;

        // La personne qui l'a tiré, car il doit infliger du dommage aux ennemis seulement
        creator = personnage;
    }

    private void Move()
    {
        transform.Translate(movement * (speed * Time.fixedDeltaTime));
    }

    private bool TouchedEnemy()
    {
        return EnnemiesInContact().Count > 0;
    }

    private bool TouchedGround()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, rend.bounds.extents.x, LayerMask.GetMask("Ground"));
        
        return colliders.Length > 0;
    }

    private void OnContact()
    {
        // Faire du dommage a tous les ennemis avec qui le projectile est en contact
        foreach (var enemy in EnnemiesInContact())
            enemy.SubirDegats(damage, movement);

        // Detruit le projectile
        Destroy(gameObject);
    }

    private List<Personnage> EnnemiesInContact()
    {
        var characters = new List<Personnage>();

        // Get les colliders qui sont pres 
        var colliders = Physics2D.OverlapCircleAll(transform.position, rend.bounds.extents.x);

        // Vérifie chaque collider pour savoir s'il s'agit d'une unité
        foreach (var collider in colliders)
            if (collider.TryGetComponent(out Personnage character))
                // Si c'est un autre que celui qui a cree le projectile
                if (character != creator)
                    characters.Add(character);

        return characters;
    }

    private bool ExitedField()
    {
        // Si il part trop d'un bord ou de l'autre du terrain en X OU Si il tombe trop bas
        return Mathf.Abs(transform.position.x) > 60f || transform.position.y < -40f;
    }
}