using UnityEngine;
using UnityEngine.Tilemaps;

public class FlipWater : MonoBehaviour
{
    public float secondsPerFlip = 0.5f;

    Tilemap tilemap;
    float timer;
    bool flipped;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < secondsPerFlip) return;

        timer -= secondsPerFlip;
        flipped = !flipped;

        transform.localScale = new Vector3(flipped ? -1 : 1, 1, 1);
    }
}