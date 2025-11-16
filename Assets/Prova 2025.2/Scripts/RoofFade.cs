using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofFader : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;
    public float fadeSpeed = 5f;
    private float targetAlpha = 1f;

    void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    void Update()
    {
        // Smooth transition
        Color c = tilemapRenderer.material.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        tilemapRenderer.material.color = c;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            targetAlpha = 0.3f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            targetAlpha = 1f;
    }
}
