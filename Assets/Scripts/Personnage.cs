using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personnage : MonoBehaviour
{
    // Référence au PlayerInputReader
    PlayerInputReader inputReader;
    Animator animator;
    
    Vector2 mouvement;

    float vitesse;

    float jumpForce;
    int numberJumps;

    float damage;
    int lives;
    
    Rigidbody2D rb;
    SpriteRenderer sr;

    Vector2 feetPosition = new(-0.75f, -2.56f);
    float feetWidth = 1.5f;
    
    Vector2 respawnPosition = new(0, 10f);

    public GameObject prefabBullet;

    bool previouslyGrounded = false;
    
    // Button south
    void BS_onClicked()
    {
        Jump();
    }

    // Left stick
    void LS_moved(Vector2 direction)
    {
        Move(direction);
    }
    
    // Right stick
    void RS_moved(Vector2 direction)
    {
        Debug.Log(direction);
    }
    
    // Right trigger (in the future)
    void RT_moved(float idk)
    {
        Debug.Log($"LS moved {idk}");
    }

    // Button east
    void BE_onClicked()
    {
        Debug.Log("BE on clicked");
    }

    // Start is called before the first frame update
    void Start()
    {
        SetAttributes();
        
        // Assigner le PlayerInputReader
        inputReader = GetComponent<PlayerInputReader>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // S'abonner aux inputs
        SetInputs();
    }

    void SetInputs()
    {
        // Button south/W
        inputReader.BS.callback += BS_onClicked;
        // Left stick/WASD
        inputReader.LS_m.callback += LS_moved;
        // Right trigger/NOTHING
        inputReader.RT.callback += RT_moved;
        // TEMPORARY
        inputReader.BE.callback += BE_onClicked;
        // Right stick/
        inputReader.RS_m.callback += RS_moved;
    }

    void SetAttributes()
    {
        mouvement = Vector2.zero;
        vitesse = 20;
        jumpForce = 35;
        
        damage = 0;
        lives = 5;
    }

    void Update()
    {
        if (ExitedField())
        {
            Respawn();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsGrounded() && rb.velocity.y == 0)
        {
            // Si il est au sol, il reprend ses 2 sauts
            numberJumps = 0;
            
            // Si il n'etait pas deja a terre
            if (!previouslyGrounded)
            {
                animator.SetTrigger("endJumping");
            }
        }
            
        
        transform.Translate(mouvement * (vitesse * Time.deltaTime));
        //rb.MovePosition(rb.position + mouvement * vitesse);
        
        previouslyGrounded = IsGrounded();
        
    }

    void Jump()
    {
        // Si il a deja saute 2 fois, ne pas le laisser sauter plus
        if (numberJumps >= 2)
            return;
        
        // Incrementer le nombre de jumps qu'il fait
        numberJumps++;
        
        animator.SetTrigger("startJumping");
        
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

        // Si il bouge en X, le faire marcher
        if (Mathf.Abs(direction.x) > 0f)
        {
            animator.SetBool("isWalking", true);
            
            // Si la direction est < 0, donc a guche, flip le sprite
            sr.flipX = direction.x < 0f;
        }
        else
            animator.SetBool("isWalking", false);
        
        mouvement = direction;
    }

    void Respawn()
    {
        rb.velocity = Vector2.zero;
        transform.position = respawnPosition;
    }

    public void SubirDegats(float degats, Vector2 direction)
    {
        damage -= degats;
        
        rb.AddForce(direction.normalized * (degats * 0.1f), ForceMode2D.Impulse);
    }

    bool ExitedField()
    {
        // Si il part trop d'un bord ou de l'autre du terrain en X OU Si il tombe trop bas
        if (Mathf.Abs(transform.position.x) > 40f || transform.position.y < -20f)
        {
            lives--;
            return true;
        }
        
        return false;
        
    }

    bool IsGrounded()
    {
        // Fait une ligne en dessous des pieds et regarde si elle est en contact avec le sol
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x + feetPosition.x, transform.position.y + feetPosition.y), Vector2.right, feetWidth, LayerMask.GetMask("Ground"));
        
        // Hitbox de pieds pour isgrounded DEBUG
        Debug.DrawRay(new Vector3(transform.position.x + feetPosition.x, transform.position.y + feetPosition.y),
            Vector3.right * feetWidth, Color.red, 5f);

        return hit;
    }
}