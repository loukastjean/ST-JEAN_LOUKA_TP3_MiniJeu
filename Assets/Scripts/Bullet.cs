using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Personnage creator;
    private float damage;
    private Vector2 movement;
    private float speed;

    private SpriteRenderer spriteRenderer;

    #region Initialization

    // Set les variables importantes du projectile quand il est créé
    public void SetAttributes(Vector2 direction, Vector2 origin, Personnage shooter)
    {
        transform.position = origin;
        movement = direction.normalized;

        speed = 40f;
        damage = 10f;
        creator = shooter;
    }

    #endregion

    #region Unity Methods

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Si il frappe un ennemi
        if (HasHitEnemy() || HasHitGround())
            HandleImpact();

        // Si il a depasse les limites
        if (IsOutOfBounds())
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Move();
    }

    #endregion

    #region Logic

    // Fait bouger le projectile à une vitesse constante
    private void Move()
    {
        transform.Translate(movement * (speed * Time.fixedDeltaTime));
    }

    // Verifie si le projectile touche a un ennemi
    private bool HasHitEnemy()
    {
        return GetEnemiesInRange().Count > 0;
    }

    // Inflige les degats quand le projectile touche un ennemi
    private void HandleImpact()
    {
        foreach (var enemy in GetEnemiesInRange()) enemy.TakeDamage(damage, movement);

        Destroy(gameObject);
    }

    // Si le projectile est à l'exterieur des limites
    private bool IsOutOfBounds()
    {
        return Mathf.Abs(transform.position.x) > 60f || transform.position.y < -40f;
    }

    #endregion

    #region Helpers

    // Recupere les ennemis qui touche au projectile
    private List<Personnage> GetEnemiesInRange()
    {
        var enemies = new List<Personnage>();

        var radius = spriteRenderer.bounds.extents.x;
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var col in colliders)
            if (col.TryGetComponent(out Personnage character) && character != creator)
                enemies.Add(character);

        return enemies;
    }

    // Si le projectile est entre en contact avec le sol
    private bool HasHitGround()
    {
        var radius = spriteRenderer.bounds.extents.x;
        var groundHits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Ground"));

        return groundHits.Length > 0;
    }

    #endregion
}