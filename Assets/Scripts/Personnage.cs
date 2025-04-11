using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personnage : MonoBehaviour
{

    // Référence au PlayerInputReader
    PlayerInputReader inputReader;
    
    Vector2 mouvement;

    float vitesse;

    float jumpForce = 35;
    
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        SetAttributes();
        
        // Assigner le PlayerInputReader
        inputReader = GetComponent<PlayerInputReader>();
        rb = GetComponent<Rigidbody2D>();
        
        // S'abonner au input du clickage de bouton sud
        inputReader.BS.callback += BS_onClicked;

        inputReader.LS_m.callback += LS_moved;
    }

    void SetAttributes()
    {
        mouvement = Vector2.zero;
        vitesse = 20;
        //jumpForce = 30;
    }

    void BS_onClicked()
    {
        Jump();
    }

    void LS_moved(Vector2 direction)
    {
        Move(direction);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(mouvement * (vitesse * Time.deltaTime));
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Move(Vector2 direction)
    {
        if (rb.velocity.y > 0)
        {
            rb.gravityScale = 7f;
        }
        else
        {
            rb.gravityScale = 10f;
        }
        
        if (direction.y > 0)
            direction.y = 0;
        
        mouvement = direction;
    }

    void isGrounded()
    {
        
    }
    
}
