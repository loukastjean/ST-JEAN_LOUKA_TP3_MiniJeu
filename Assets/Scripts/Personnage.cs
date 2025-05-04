using UnityEngine;

public class Personnage : MonoBehaviour
{
    [SerializeField] private GameObject prefabBullet;

    private float vitesse, jumpForce, dashForce;
    private int numberJumps;
    private bool canJumpWithStick;
    
    public LineRenderer lineRenderer;
    private Vector2 aim = Vector2.zero;
    private Animator animator;

    private readonly Vector2 feetPosition = new(-0.75f, -2.56f);
    private readonly float feetWidth = 1.5f;

    private InGameManager ingameManager;

    // Référence au PlayerInputReader
    private PlayerInputReader inputReader;
    private float lastDashTime;
    private float lastShotTime;

    private Vector2 mouvement;

    private bool previouslyGrounded;

    private Rigidbody2D rb;

    private readonly Vector2 respawnPosition = new(0, 10f);
    private SpriteRenderer sr;
    private bool wantsToShoot;

    public float damage { get; private set; }
    public int lives { get; private set; }
    
    private AudioSource audioSource;
    
    [SerializeField] private AudioClip clipWalk, clipLand, clipJump, clipDash, clipShoot, clipHurt;
    

    private void Update()
    {
        if (ExitedField())
            Respawn();

        // Update the point positions of the line renderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + new Vector3(aim.x, aim.y).normalized * 100f);

        if (wantsToShoot)
            Shoot(aim);

        animator.SetBool("isFalling", rb.velocity.y < -15f);

        if (IsGrounded() && rb.velocity.y <= 5f)
        {
            // Si il est au sol, il reprend ses 2 sauts
            numberJumps = 0;

            // Si il n'etait pas deja a terre
            if (!previouslyGrounded)
            {
                lastDashTime = -99f;
                animator.SetTrigger("endJumping");
                
                audioSource.PlayOneShot(clipLand);
            }
        }

        previouslyGrounded = IsGrounded();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Translate(mouvement * vitesse * Time.fixedDeltaTime);

        // Pour ajouter de la velocite au rigidbody seulement quand il veut bouger en sens inverse de ou il va
        // Verifier donc si le decplacement est contraire a la velocite
        if ((mouvement.x > 0 && rb.velocity.x < 0) || (mouvement.x < 0 && rb.velocity.x > 0))
            rb.velocity += new Vector2(mouvement.x * vitesse * Time.fixedDeltaTime * 5f, 0);
    }


    // Left stick
    private void LS_moved(Vector2 direction)
    {
        Move(direction);
    }

    // Right stick
    private void RS_moved(Vector2 direction)
    {
        if (direction.magnitude > 0.1f)
            aim = direction;
    }

    // Right trigger
    private void RT_moved(float press)
    {
        wantsToShoot = press > 0.4f;
    }

    // Button east
    private void BE_onClicked()
    {
        Dash();
    }

    private void Menu_OnClicked()
    {
        ingameManager.Pause();
    }


    public void Creation()
    {
        // Assigner le PlayerInputReader
        inputReader = GetComponent<PlayerInputReader>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        ingameManager = FindObjectOfType<InGameManager>();
        audioSource = GetComponent<AudioSource>();
        
        

        // S'abonner aux inputs
        SetInputs();

        SetAttributes();
    }

    private void SetInputs()
    {
        // Left stick/WASD
        inputReader.LS_m.callback += LS_moved;
        // Right trigger/NOTHING
        inputReader.RT.callback += RT_moved;
        // TEMPORARY
        inputReader.BE.callback += BE_onClicked;
        // Right stick/
        inputReader.RS_m.callback += RS_moved;
        // Menu (Pause)
        inputReader.Menu.callback += Menu_OnClicked;
    }

    private void SpawnAttributes()
    {
        lastShotTime = -99f;
        lastDashTime = -99f;
        wantsToShoot = false;
        canJumpWithStick = true;
        numberJumps = 0;

        mouvement = Vector2.zero;
        rb.velocity = Vector2.zero;

        damage = 0f;
    }

    private void SetAttributes()
    {
        SpawnAttributes();

        vitesse = 20f;
        jumpForce = 35f;
        dashForce = 30f;

        lives = 5;
    }

    private void Jump()
    {
        // Si il a deja saute 2 fois, ne pas le laisser sauter plus
        if (numberJumps >= 2)
            return;

        // Incrementer le nombre de jumps qu'il fait
        numberJumps++;

        animator.SetTrigger("startJumping");
        
        //audioSource.PlayOneShot(clipJump); //TODO

        // Faire que peu importe sa velocite, il saute la meme hauteur
        rb.velocity = new Vector2(rb.velocity.x, 0);
        // Sauter un peu vers la direction du joystick ou on veut aller
        rb.AddForce((Vector2.up + mouvement * 0.1f).normalized * jumpForce, ForceMode2D.Impulse);
    }

    private void Dash()
    {
        if (Time.time < lastDashTime + 3f)
            return;
        
        audioSource.PlayOneShot(clipDash);

        if (sr.flipX)
            rb.AddForce((Vector2.left + mouvement * 0.1f).normalized * dashForce, ForceMode2D.Impulse);
        else
            rb.AddForce((Vector2.right + mouvement * 0.1f).normalized * dashForce, ForceMode2D.Impulse);

        lastDashTime = Time.time;
    }

    private void Move(Vector2 direction)
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
            
            //audioSource.PlayOneShot(clipWalk); //TODO

            // Si la direction est < 0, donc a gauche, flip le sprite
            sr.flipX = direction.x < 0f;
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        mouvement = direction;
    }

    private void Respawn()
    {
        SpawnAttributes();

        lives--;
        transform.position = respawnPosition;
    }

    public void SubirDegats(float degats, Vector2 direction)
    {
        damage += degats;

        if (direction.y < 0f && IsGrounded())
            direction.x *= 2f;
        
        audioSource.PlayOneShot(clipHurt);

        rb.AddForce(direction.normalized * (damage / 3 + 40f), ForceMode2D.Impulse);
    }

    private void Shoot(Vector2 direction)
    {
        if (direction == Vector2.zero || Time.time - lastShotTime < 0.8f)
            return;
        
        audioSource.PlayOneShot(clipShoot);

        var bullet = Instantiate(prefabBullet, transform.position, Quaternion.identity).GetComponent<Bullet>();

        bullet.SetAttributes(direction, transform.position, this);

        lastShotTime = Time.time;
    }

    private bool ExitedField()
    {
        // Si il part trop d'un bord ou de l'autre du terrain en X OU Si il tombe trop bas
        return Mathf.Abs(transform.position.x) > 70f || transform.position.y < -30f;
    }

    private bool IsGrounded()
    {
        // Fait une ligne en dessous des pieds et regarde si elle est en contact avec le sol
        var hit = Physics2D.Raycast(
            new Vector2(transform.position.x + feetPosition.x, transform.position.y + feetPosition.y), Vector2.right,
            feetWidth, LayerMask.GetMask("Ground"));

        // Hitbox de pieds pour isgrounded DEBUG
        //Debug.DrawRay(new Vector3(transform.position.x + feetPosition.x, transform.position.y + feetPosition.y), Vector3.right * feetWidth, Color.red, 5f);

        return hit;
    }
}