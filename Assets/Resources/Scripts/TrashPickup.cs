using UnityEngine;

public class TrashPickup : MonoBehaviour
{
    [SerializeField] Sprite[] variations;
    [SerializeField] int destroyPosibility = 5;
    private float secondsPerBob = 0.25f;
    private static float unitsPerPixel = 0.0625f;
    private float timer;
    private bool dir;
    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (variations.Length > 0)
            sr.sprite = variations[Random.Range(0, variations.Length)];

        sr.flipX = Random.value > 0.5f; 

        GetComponent<Collider2D>().isTrigger = true;

        if (Random.Range(0, destroyPosibility) == 0) Destroy(gameObject);

        timer = -(transform.position.x / 5);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < secondsPerBob) return;

        timer -= secondsPerBob;
        dir = !dir;
        transform.position += (dir ? Vector3.up : Vector3.down) * unitsPerPixel*2;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // TODO que haga algo
        Debug.Log("Objeto recogido");

        Destroy(gameObject);
    }
}