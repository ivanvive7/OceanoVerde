using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Vector3 targetPos = new(0.5f, 0.5f, 0);
    [SerializeField] int timeBeforeSpawn = 2;
    [SerializeField] int moveSpeed = 2;
    [SerializeField] int throwInterval = 5;

    [Header("Throwing")]
    [SerializeField] GameObject trashPrefab;
    [SerializeField] float riseHeight = 2f;
    [SerializeField] float riseTime = 0.4f;
    [SerializeField] float throwTime = 0.8f;
    [SerializeField] float minDistance = 1f;
    [SerializeField] float maxDistance = 5f;
    [SerializeField] Vector3 throwDirection = Vector3.left;
    [SerializeField] AudioClip sfx;
    [SerializeField, Range(0f, 1f)] float volume = 1f;

    [Header("Ground")]
    [SerializeField] LayerMask groundMask;      
    [SerializeField] float gravity = 20f;       

    private float timer;
    private float throwTimer;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        throwTimer = throwInterval - 0.5f; // first throw happens on arrival
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < timeBeforeSpawn) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (transform.position != targetPos) return;

        throwTimer += Time.deltaTime;
        if (throwTimer < throwInterval) return;
        throwTimer -= throwInterval;
        if (GameManager.Instancia != null && GameManager.Instancia.NivelTerminado) return;
        StartCoroutine(ThrowTrash());
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //die if touching player
        if (!other.CompareTag("Player")) return;
        Destroy(gameObject);
    }

    IEnumerator ThrowTrash()
    {
        if (animator != null)
            animator.SetTrigger("Throw");

        if (sfx != null)
            Sfx.Play(sfx, volume);

        Vector3 start = targetPos;
        Vector3 apex = start + Vector3.up * riseHeight;
        float landX = start.x + throwDirection.normalized.x * Random.Range(minDistance, maxDistance);

        GameObject obj = Instantiate(trashPrefab, start, Quaternion.identity);

        if (GameManager.Instancia != null)
            GameManager.Instancia.RegistrarBasura();

        TrashPickup pickup = obj.GetComponent<TrashPickup>();
        pickup.enabled = false;
        pickup.skipDestroyRoll = true;

       
        float groundY = start.y; // fallback if nothing is hit
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(landX, apex.y), Vector2.down, 100f, groundMask);
        if (hit.collider != null)
        {
            // Offset by half the trash's height so it rests ON the ground 
            float halfHeight = obj.GetComponent<Collider2D>().bounds.extents.y;
            groundY = hit.point.y + halfHeight;
        }
        Vector3 landing = new Vector3(landX, groundY, start.z);

        //  rise
        for (float t = 0; t < 1f; t += Time.deltaTime / riseTime)
        {
            if (obj == null) yield break;
            obj.transform.position = Vector3.Lerp(start, apex, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // fall
        float drop = Mathf.Max(0.01f, apex.y - landing.y);
        float fallTime = Mathf.Sqrt(2f * drop / gravity);

        for (float t = 0; t < 1f; t += Time.deltaTime / fallTime)
        {
            if (obj == null) yield break;
            Vector3 pos = obj.transform.position;
            pos.x = Mathf.Lerp(apex.x, landing.x, t);
            pos.y = Mathf.Lerp(apex.y, landing.y, t * t); // accelerating fall
            obj.transform.position = pos;
            yield return null;
        }

        if (obj == null) yield break;
        obj.transform.position = landing;
        pickup.enabled = true;
    }
}