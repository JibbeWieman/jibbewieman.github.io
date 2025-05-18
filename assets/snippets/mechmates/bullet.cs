using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Assignables")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    [Header("Bullet stats")]
    public float bounciness;
    public bool useGravity;

    [Header("Damage")]
    public float explosionRange;
    public float explosionForce;

    [Header("Lifetime")]
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    int collisions;
    PhysicMaterial physics_mat;
    [HideInInspector] public RangedWeapon.statsDamage damageStats;

    private void Awake()
    {
        Setup();
    }

    private void Update()
    {
        //When to explode:
        if (collisions > maxCollisions) Explode();

        //Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }

    private void Explode()
    {
        //Instantiate explosion
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        //Check for enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);

        for (int i = 0; i < enemies.Length; i++)
        {
            //Get component of enemy and call TakeDamage
            enemies[i].GetComponent<EnemyBase>
            ().TakeDamage(damageStats);

            //Add explosion force (if enemy has a rigidbody)
            if (enemies[i].GetComponentInParent<Rigidbody>())
            {
                enemies[i].GetComponentInParent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
            }
        }

        //Add a little delay, just to make sure everything works fine
        Invoke("Delay", 0.05f);
    }
    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug the collision event
        Debug.Log("Collision with: " + collision.gameObject.name);

        //Count up collisions
        collisions++;

        //Deal base damage if bullet hits an enemy directly
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemyScript = collision.gameObject.GetComponent<EnemyBase>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damageStats);
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
        collision.gameObject.layer == LayerMask.NameToLayer("Walls"))
        {
            Destroy(gameObject);
        }

        //Explode if bullet hits an enemy directly and explodeOnTouch is activated
        if (explodeOnTouch && (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")))
        {
            Explode();
        }


    private void Setup()
    {
        //Create a new Physics material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;
        rb.GetComponent<Rigidbody>();

        //Set gravity
        rb.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}