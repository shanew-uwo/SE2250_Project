using UnityEngine;
using System.Collections;

public class BossAOEVisual : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int points = 50;         // Number of points in the circle
    public float radius = 0.1f;       // Initial radius
    public float expandSpeed = 5f;  // How fast the circle expands
    public float maxRadius = 7f;    // Maximum expansion radius
    
    void Start()
    {
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = points + 1; // Close the loop
        lineRenderer.loop = true;
        DrawCircle(radius);
    }

    public void StartAOEEffect()
    {
        StopAllCoroutines(); // Stop any previous effect before starting a new one
        lineRenderer.enabled = true; // Make sure it's visible again
        StartCoroutine(ExpandCircle());
    }

    private IEnumerator ExpandCircle()
    {
        float currentRadius = radius;

        while (currentRadius < maxRadius)
        {
            currentRadius += expandSpeed * Time.deltaTime;
            DrawCircle(currentRadius);
            yield return null;
        }

        lineRenderer.enabled = false; // Hide after expansion
    }

    private void DrawCircle(float r)
    {
        float angleStep = 360f / points;
        for (int i = 0; i <= points; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * r, 0.1f, Mathf.Sin(angle) * r);
            lineRenderer.SetPosition(i, transform.position + pos);
        }
    }
}