using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personnage : MonoBehaviour
{
    // Référence au PlayerInputReader
    PlayerInputReader inputReader;
    
    Vector2 mouvement;

    float vitesse;

    float jumpForce;
    int numberJumps;
    
    Rigidbody2D rb;

    Vector2 positionFeet = new(-0.8f, -2.6f);
    float longueurFeet = 1.5f;

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
        jumpForce = 35;
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
    void FixedUpdate()
    {
        if (IsGrounded() && rb.velocity.magnitude < 0.1f)
            numberJumps = 0;
        
        transform.Translate(mouvement * (vitesse * Time.deltaTime));
        //rb.MovePosition(rb.position + mouvement * vitesse);
    }

    void Jump()
    {
        // Si il a deja saute 2 fois, ne pas le laisser sauter plus
        if (numberJumps >= 2)
            return;
        
        // Incrementer le nombre de jumps qu'il fait
        numberJumps++;
        
        Debug.Log(numberJumps);
        
        // Faire que peu importe sa velocite, il saute la meme hauteur
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
            rb.gravityScale = 12f;
        }

        // Juste laisser le fastfall
        if (direction.y < 0 && !IsGrounded())
            direction.y *= 1.3f;
        else
            direction.y = 0;
        
        
        mouvement = direction;
    }

    bool IsGrounded()
    {
        // Fait une ligne en dessous des pieds et regarde si elle est en contact avec le sol
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + positionFeet.x, transform.position.y + positionFeet.y), Vector2.right, longueurFeet, LayerMask.GetMask("Ground"));
        
        // Hitbox de pieds pour isgrounded
        Debug.DrawRay(new Vector3(transform.position.x + positionFeet.x, transform.position.y + positionFeet.y),
            Vector3.right * longueurFeet, Color.red, 5f);
        
        if (hit)
        {
            return true;
        }
        
        return false;
    }
}