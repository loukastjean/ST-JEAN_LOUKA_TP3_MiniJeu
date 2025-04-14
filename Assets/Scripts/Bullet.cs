using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float vitesse;
    float degats;
    Vector2 destination;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetAttributes(Vector2 _destination)
    {
        vitesse = 4f;
        degats = 10f;
        destination = _destination;
    }

    // Update is called once per frame
    void Update()
    {
        // Mauvaise logique, juste pour tester
        if (Vector2.Distance(transform.position, destination) < vitesse)
        {
            OnContact();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(new Vector2(destination.x - transform.position.x, destination.y - transform.position.y).normalized * vitesse);
    }

    void OnContact()
    {
        // Pour l'instant
        Destroy(gameObject);
    }
    
}
