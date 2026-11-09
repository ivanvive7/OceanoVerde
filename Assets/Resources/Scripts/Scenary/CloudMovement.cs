using UnityEngine;
using UnityEngine.Tilemaps;

public class CloudMovement : MonoBehaviour
{
    public float secondsPerMove = 0.5f;
    private static float unitsPerPixel = 0.0625f;
    float timer;

    void Start()
    {
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < (secondsPerMove + Random.Range(-0.2f,0.2f) ) ) return;

        timer -= secondsPerMove;

        transform.position += Vector3.left * (unitsPerPixel * Random.Range(1,3));

        if (transform.position.x < -10)
        {
            transform.position = new Vector3(10, transform.position.y + (unitsPerPixel * Random.Range(-10,10)), transform.position.z);
        }
    }
}
