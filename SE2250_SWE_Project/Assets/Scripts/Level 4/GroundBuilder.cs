using UnityEngine;

public class GroundBuilder : MonoBehaviour
{
    public GameObject groundCubePrefab;
    public int width = 10;
    public int depth = 10;
    public float cubeSize = 1f;

    public Vector3 offsetPosition = new Vector3(-10, 0, -10); // ✅ Shift the whole grid

    void Start()
    {
        BuildFlatGround();
    }

    void BuildFlatGround()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 position = new Vector3(x * cubeSize, -0.25f, z * cubeSize) + offsetPosition;
                GameObject cube = Instantiate(groundCubePrefab, position, Quaternion.identity, transform);
                cube.transform.localScale = new Vector3(cubeSize, 0.5f, cubeSize);
                cube.name = $"GroundCube_{x}_{z}";
            }
        }
    }
}