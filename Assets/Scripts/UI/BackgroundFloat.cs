using UnityEngine;

public class BackgroundFloat : MonoBehaviour
{
    public float speed = 0.2f;
    public float amount = 0.15f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * amount;
        transform.position = startPos + new Vector3(0f, y, 0f);
    }
}