using System.Collections.Generic;
using UnityEngine;

public class Personnage : MonoBehaviour
{
    #region Destruction

    public void RemoveInputs()
    {
        inputReader.LS_m.callback -= LS_moved;
        inputReader.RT.callback -= RT_moved;
        inputReader.BE.callback -= BE_onClicked;
        inputReader.RS_m.callback -= RS_moved;
        inputReader.Menu.callback -= Menu_OnClicked;
    }

    #endregion

    #region Serialized Fields

    [SerializeField] private GameObject prefabBullet;
    [SerializeField] private AudioClip clipWalk, clipLand, clipJump, clipDash, clipShoot, clipHurt;

    #endregion

    #region Components

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private PlayerInputReader inputReader;
    private InGameManager ingameManager;

    #endregion

    #region Public

    public LineRenderer lineRenderer;
    public SpriteRenderer sr;
    public Vector2 aim;
    public float damage { get; private set; }
    public int lives { get; private set; }

    #endregion

    #region Movement Variables

    private float speed;
    private float jumpForce;
    private float dashForce;

    private Vector2 movement;

    private int numberJumps;
    private bool canJumpWithStick;
    private bool previouslyGrounded;
    private bool wantsToShoot;

    private float lastDashTime;
    private float lastAttackTime;

    #endregion

    #region Constants

    private readonly Vector2 feetPosition = new(-0.75f, -2.56f);
    private readonly float feetWidth = 1.5f;
    private readonly Vector2 respawnPosition = new(0, 10f);

    #endregion

    #region Unity

    private void Update()
    {
        HandleOutOfBounds();
        UpdateLineRenderer();
        HandleShooting();
        HandleGrounding();
        HandlePlatforms();
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        AddDirectionalMomentum();
    }

    #endregion

    #region Initialization

    public void Creation()
    {
        inputReader = GetComponent<PlayerInputReader>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        ingameManager = FindObjectOfType<InGameManager>();
        audioSource = GetComponent<AudioSource>();

        SetInputs();
        SetAttributes();
    }

    private void SetInputs()
    {
        inputReader.LS_m.callback += LS_moved;
        inputReader.RT.callback += RT_moved;
        inputReader.BE.callback += BE_onClicked;
        inputReader.BN.callback += BN_onClicked;
        inputReader.RS_m.callback += RS_moved;
        inputReader.Menu.callback += Menu_OnClicked;
    }

    // Set les attributs de début de partie
    private void SetAttributes()
    {
        speed = 20f;
        jumpForce = 35f;
        dashForce = 30f;

        lives = 5;

        SpawnAttributes();
    }

    // Set les attributs de respawn
    private void SpawnAttributes()
    {
        lastDashTime = -99f;
        lastAttackTime = -99f;
        numberJumps = 0;
        canJumpWithStick = true;
        wantsToShoot = false;
        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
        damage = 0f;
    }

    #endregion

    #region Input Callbacks

    // Left stick / WASD
    private void LS_moved(Vector2 direction)
    {
        Move(direction);
    }

    // Right stick / NOTHING
    private void RS_moved(Vector2 direction)
    {
        if (direction.magnitude > 0.1f) aim = direction;
    }

    // Right trigger / NOTHING
    private void RT_moved(float press)
    {
        wantsToShoot = press > 0.4f;
    }

    // Button east / E
    private void BE_onClicked()
    {
        Dash();
    }

    // Button north / S
    private void BN_onClicked()
    {
        Roll();
    }

    // Menu / P
    private void Menu_OnClicked()
    {
        ingameManager.Pause();
    }

    #endregion

    #region Movement

    private void Move(Vector2 direction)
    {
        // Si le stick est assez redescendu par rapport au dernier saut avec stick
        if (!canJumpWithStick && direction.y < 0.3f) canJumpWithStick = true;

        // Appliquer une plus grande gravité si en descendant pour rendre le mouvement plus rapide
        rb.gravityScale = rb.velocity.y > 0 ? 7f : 12f;

        // Verifie sie le stick est descendu avant de resauter
        if (direction.y > 0.7f && canJumpWithStick)
        {
            canJumpWithStick = false;
            Jump();
        }

        // Fastfall
        if (direction.y < 0f && !IsGrounded())
            direction.y *= 1.3f;
        else
            direction.y = 0;

        // Si on tente de bouger à l'horizontal, mettre l'animation de walking
        if (Mathf.Abs(direction.x) > 0f)
        {
            animator.SetBool("isWalking", true);
            sr.flipX = direction.x < 0f;
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        movement = direction;
    }

    private void Jump()
    {
        // Si on a déjà dépensé les 2 sauts dans les airs
        if (numberJumps >= 2) return;

        numberJumps++;
        animator.SetTrigger("startJumping");

        // Applique la force au rigidbody pour sauter, reset le mouvement en Y mais pas celui en X
        rb.velocity = new Vector2(rb.velocity.x, 0);
        // Sauter avec un petit boost vers l'endroit ou il veut aller
        rb.AddForce((Vector2.up + movement * 0.1f).normalized * jumpForce, ForceMode2D.Impulse);
    }

    private void Dash()
    {
        // Si ca fait pas encore 3 secondes depuis le dernier dash
        if (Time.time < lastDashTime + 3f) return;

        audioSource.PlayOneShot(clipDash);

        // Donner une force vers ou on veut aller
        var direction = sr.flipX ? Vector2.left : Vector2.right;
        rb.AddForce((direction + movement * 0.1f).normalized * dashForce, ForceMode2D.Impulse);

        lastDashTime = Time.time;
    }

    private void MoveCharacter()
    {
        transform.Translate(movement * (speed * Time.fixedDeltaTime));
    }

    // Avoir plus de facilité à contrer la vélocité
    private void AddDirectionalMomentum()
    {
        if ((movement.x > 0 && rb.velocity.x < 0) || (movement.x < 0 && rb.velocity.x > 0))
            rb.velocity += new Vector2(movement.x * speed * Time.fixedDeltaTime * 5f, 0);
    }

    #endregion

    #region Combat

    private void Shoot()
    {
        // Empecher de tirer trop rapidement
        if (Time.time - lastAttackTime < 0.8f) return;

        audioSource.PlayOneShot(clipShoot);

        var bullet = Instantiate(prefabBullet, GetComponent<Collider2D>().bounds.center, Quaternion.identity)
            .GetComponent<Bullet>();

        bullet.SetAttributes(aim, rb.worldCenterOfMass, this);

        lastAttackTime = Time.time;
    }

    private void Roll()
    {
        // Empecher de tirer trop rapidement
        if (Time.time - lastAttackTime < 0.8f) return;

        //audioSource.PlayOneShot(); // TODO ajoute son pour attaque

        // Afflige du dommage et du knockback sur tous les ennemis qui sont dans le rayon d'attaque
        foreach (var enemy in GetEnemiesInRange())
        {
            Vector2 differenceBetweenPlayers = enemy.transform.position - transform.position;
            enemy.TakeDamage(20f, differenceBetweenPlayers);
            Debug.Log(enemy.damage);
        }


        animator.SetTrigger("roll");

        lastAttackTime = Time.time;
    }

    public void TakeDamage(float damage, Vector2 direction)
    {
        this.damage += damage;

        direction = direction.normalized;

        // Donner plus de recul quand on est sur le sol, car la friction fait que le projectile impacte moins
        if (direction.y < 0f && IsGrounded()) direction.x *= 1.2f;

        audioSource.PlayOneShot(clipHurt);

        // Inflige une force dans le meme sens que la trajectoire du projectile
        rb.AddForce(direction * (damage / 3 + 40f), ForceMode2D.Impulse);
    }

    #endregion

    #region Utility

    // Recupere les ennemis qui touche au projectile
    private List<Personnage> GetEnemiesInRange()
    {
        var enemies = new List<Personnage>();

        // Donne un peu de range a l'attaque
        Debug.Log(GetComponent<Collider2D>().bounds.extents);
        var radius = GetComponent<Collider2D>().bounds.extents.x * 3f;
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var col in colliders)
            if (col.TryGetComponent(out Personnage character) && character != this)
                enemies.Add(character);

        return enemies;
    }

    private void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (collisionInfo.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            // Dessine une ligne sous les pieds du personnage et regarde si ca entre en contact avec le sol
            var hitPlatform = Physics2D.Raycast(
                new Vector2(transform.position.x, transform.position.y + feetPosition.y),
                Vector2.up,
                4f,
                LayerMask.GetMask("Platforms")
            );

            Debug.DrawRay(
                new Vector3(transform.position.x, transform.position.y + feetPosition.y),
                Vector2.up * 4f,
                Color.green,
                2f
            ); // DEBUG // TODO

            if (hitPlatform)
            {
                Debug.Log("PlayersThroughPlatforms");
                gameObject.layer = LayerMask.NameToLayer("PlayersThroughPlatforms");
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(GetComponent<Collider2D>().bounds.center, GetComponent<Collider2D>().bounds.extents);
    }


    private void HandlePlatforms()
    {
        // Change la layer en fonction de si il est dans une platforme
        gameObject.layer = IsInPlatform()
            ? LayerMask.NameToLayer("PlayersThroughPlatforms")
            : LayerMask.NameToLayer("Players");
    }


    private bool IsInPlatform()
    {
        // Dessine une ligne qui passe a travers le personnage et regarde si elle touche une plateforme 
        var hitPlatform = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y + feetPosition.y),
            Vector2.up,
            4f,
            LayerMask.GetMask("Platforms")
        );

        Debug.DrawRay(
            new Vector3(transform.position.x, transform.position.y + feetPosition.y),
            Vector2.up * 4f,
            Color.green,
            2f
        ); // DEBUG // TODO

        return hitPlatform;
    }

    private void HandleOutOfBounds()
    {
        // Si il est parti trop loin, il respawn
        if (ExitedField())
            Respawn();
    }

    // Dessine la ligne de visée du personnage
    private void UpdateLineRenderer()
    {
        lineRenderer.SetPosition(0, rb.worldCenterOfMass);
        lineRenderer.SetPosition(1, rb.worldCenterOfMass + new Vector2(aim.x, aim.y).normalized * 100f);
    }

    private void HandleShooting()
    {
        if (wantsToShoot)
            Shoot();
    }

    private void HandleGrounding()
    {
        // Si le personnage tombe, commener l'animation de isFalling
        animator.SetBool("isFalling", rb.velocity.y < -15f);

        // Si il est sur le sol, reset les sauts
        if (IsGrounded() && rb.velocity.y <= 5f)
        {
            numberJumps = 0;

            // Si c'est la première frame qu'il tombe au sol
            if (!previouslyGrounded)
            {
                lastDashTime = -99f;
                animator.SetTrigger("endJumping");
                audioSource.PlayOneShot(clipLand);
            }
        }

        previouslyGrounded = IsGrounded();
    }

    private bool IsGrounded()
    {
        // TODO: Avoir une boite au lieu d'une ligne

        // Dessine une ligne sous les pieds du personnage et regarde si ca entre en contact avec le sol
        var hitGround = Physics2D.Raycast(
            new Vector2(transform.position.x + feetPosition.x, transform.position.y + feetPosition.y),
            Vector2.right,
            feetWidth,
            LayerMask.GetMask("Ground")
        );

        // Dessine une ligne sous les pieds du personnage et regarde si ca entre en contact avec le sol
        var hitPlatform = Physics2D.Raycast(
            new Vector2(transform.position.x + feetPosition.x, transform.position.y + feetPosition.y),
            Vector2.right,
            feetWidth,
            LayerMask.GetMask("Platforms")
        );

        Debug.DrawRay(
            new Vector3(transform.position.x + feetPosition.x, transform.position.y + feetPosition.y),
            Vector3.right * feetWidth,
            Color.red,
            2f
        ); // DEBUG // TODO

        return hitGround || hitPlatform;
    }

    private bool ExitedField()
    {
        // Les limites du terrain
        return Mathf.Abs(transform.position.x) > 70f || Mathf.Abs(transform.position.y) > 30f;
    }

    private void Respawn()
    {
        // Lui enleve une vie et remet au spawn point
        SpawnAttributes();
        lives--;
        transform.position = respawnPosition;
    }

    #endregion
}