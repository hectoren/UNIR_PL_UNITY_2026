using UnityEngine;

public class ParallaxLoop : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Parallax")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactor = 0.5f;

    private float spriteWidth;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraPosition = cameraTransform.position;

        var sr = GetComponentInChildren<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    private void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(delta.x * parallaxFactor, 0f, 0f);
        lastCameraPosition = cameraTransform.position;

        // Loop: si un hijo queda completamente atrás de la cámara, lo mandamos al frente
        foreach (Transform child in transform)
        {
            if (cameraTransform.position.x - child.position.x > spriteWidth)
            {
                child.position += Vector3.right * spriteWidth * 2f;
            }
        }
    }
}
