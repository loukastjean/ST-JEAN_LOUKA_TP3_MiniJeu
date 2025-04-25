using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personnage : MonoBehaviour
{
    // Référence au PlayerInputReader
    PlayerInputReader inputReader;
    Animator animator;
    
    Vector2 mouvement;

    [SerializeField] float vitesse;

    [SerializeField] float jumpForce;
    [SerializeField] int numberJumps;

    [SerializeField] float damage;
    [SerializeField] int lives;
    
    Rigidbody2D rb;
    SpriteRenderer sr;

    Vector2 feetPosition = new(-0.75f, -2.56f);
    float feetWidth = 1.5f;
    
    Vector2 respawnPosition = new(0, 10f);

    public GameObject prefabBullet;

    bool previouslyGrounded = false;
    Vector2 aim = Vector2.zero;
    float lastShotTime;

    [SerializeField] bool canJumpWithStick;
    bool wantsToShoot;
    

    // Left stick
    void LS_moved(Vector2 direction)
    {
        Move(direction);
    }
    
    // Right stick
    void RS_moved(Vector2 direction)
    {
        if (direction != Vector2.zero)
            aim = direction;
    }
    
    // Right trigger (in the future)
    void RT_moved(float press)
    {
        Debug.Log("Clicked");
        
        wantsToShoot = press > 0.4f;
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
        
        lastShotTime = 0f;
        wantsToShoot = false;
        canJumpWithStick = true;
        
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
            Respawn();
        
        if (wantsToShoot)
            Shoot(aim);
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
            
        
        transform.Translate(mouvement * vitesse * Time.fixedDeltaTime);
        
        // Pour ajouter de la velocite au rigidbody seulement quand il veut bouger en sens inverse de ou il va
        // Verifier donc si le decplacement est contraire a la velocite
        if ((mouvement.x > 0 && rb.velocity.x < 0) || (mouvement.x < 0 && rb.velocity.x > 0))
            rb.velocity += new Vector2(mouvement.x * vitesse * Time.fixedDeltaTime * 5f, 0);
        
        Debug.Log(rb.velocity);
        Debug.DrawRay(transform.position, rb.velocity, Color.red, 2f);
        //rb.MovePosition(rb.position + mouvement * vitesse * Time.fixedDeltaTime);
        //rb.AddForce(mouvement * vitesse * Time.fixedDeltaTime, ForceMode2D.Impulse);
        
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
        // Sauter un peu vers la direction du joystick ou on veut aller
        rb.AddForce((Vector2.up + mouvement * 0.1f).normalized * jumpForce, ForceMode2D.Impulse);
    }

    void Move(Vector2 direction)
    {
        // Si il est en dessous du trigger de saut
        if (!canJumpWithStick && direction.y < 0.3f)
            canJumpWithStick = true;
        
        if (rb.velocity.y > 0)
            rb.gravityScale = 7f;
        
        else
            rb.gravityScale = 12f;

        // Si il saute avec le stick
        if (direction.y > 0.7f && canJumpWithStick)
        {
            canJumpWithStick = false;
            Jump();
        }

        // Juste laisser le fastfall
        if (direction.y < 0f && !IsGrounded())
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
        damage = 0f;
        lives--;
        transform.position = respawnPosition;
    }

    public void SubirDegats(float degats, Vector2 direction)
    {
        damage += degats;
        
        if (direction.y < 0f && IsGrounded())
            direction.x *= 2f;
        
        rb.AddForce((direction.normalized * (damage/3 + 40f)), ForceMode2D.Impulse);
    }

    void Shoot(Vector2 direction)
    {
        if (direction == Vector2.zero || Time.time - lastShotTime < 0.8f)
            return;
            
        Bullet bullet = Instantiate(prefabBullet, transform.position, Quaternion.identity).GetComponent<Bullet>();
        
        bullet.SetAttributes(direction, transform.position, this);
        
        lastShotTime = Time.time;
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
        //Debug.DrawRay(new Vector3(transform.position.x + feetPosition.x, transform.position.y + feetPosition.y), Vector3.right * feetWidth, Color.red, 5f);

        return hit;
    }
}