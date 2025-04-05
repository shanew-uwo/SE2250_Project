using UnityEngine;

public class TextForLevel3 : MonoBehaviour
{
    private Transform trans;
    private Vector3 offset = new Vector3(0, -180, 0);

    void Start()
    {
        trans = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Update()
    {
        transform.LookAt(trans);
        transform.Rotate(offset);
    }

}
