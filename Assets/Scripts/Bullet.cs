using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float vitesse;
    float degats;
    Vector2 movement;

    Personnage creator;
    
    SpriteRenderer renderer;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void SetAttributes(Vector2 destination, Vector2 position, Personnage personnage)
    {
        vitesse = 25f;
        degats = 10f;
        
        transform.position = position;
        movement = destination.normalized;
        creator = personnage;
    }

    // Update is called once per frame
    void Update()
    {
        // Mauvaise logique, juste pour tester
        if (TouchedEnemy())
        {
            OnContact();
        }
        
        if (ExitedField())
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(movement * (vitesse * Time.fixedDeltaTime));
    }

    bool TouchedEnemy()
    {
        return EnnemiesInContact().Count > 0;
    }

    void OnContact()
    {
        // Faire du dommage à l'ennemi
        foreach (var enemy in EnnemiesInContact())
            enemy.SubirDegats(degats, movement);
        
        // Pour l'instant
        Destroy(gameObject);
    }

    List<Personnage> EnnemiesInContact()
    {
        var personnages = new List<Personnage>();
        
        // Get les colliders qui sont pres 
        var colliders = Physics2D.OverlapCircleAll(transform.position, renderer.bounds.extents.x);
        
        // Vérifie chaque collider pour savoir s'il s'agit d'une unité
        foreach (var collider in colliders)
            if (collider.TryGetComponent(out Personnage personnage))
                if (personnage != creator)
                    personnages.Add(personnage);
        
        return personnages;
    }
    
    bool ExitedField()
    {
        // Si il part trop d'un bord ou de l'autre du terrain en X OU Si il tombe trop bas
        return Mathf.Abs(transform.position.x) > 40f || transform.position.y < -20f;
    }
}
