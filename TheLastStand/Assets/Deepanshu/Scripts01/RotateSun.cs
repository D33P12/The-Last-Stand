using UnityEngine;

public class RotateSun : MonoBehaviour
{
    [SerializeField] private float minRotationSpeed = 10f; 
    [SerializeField] private float maxRotationSpeed = 50f; 
    [SerializeField] private float changeSpeedFrequency = 1f; 

    private Vector3 rotationSpeeds;
    private float timer = 0f;
    void Start()
    {
        rotationSpeeds = new Vector3(
            Random.Range(minRotationSpeed, maxRotationSpeed),
            Random.Range(minRotationSpeed, maxRotationSpeed),
            Random.Range(minRotationSpeed, maxRotationSpeed)
        );
    }
    void Update()
    {
        transform.Rotate(rotationSpeeds * Time.deltaTime);
        timer += Time.deltaTime;
        if (timer >= changeSpeedFrequency)
        {
            rotationSpeeds = new Vector3(
                Random.Range(minRotationSpeed, maxRotationSpeed),
                Random.Range(minRotationSpeed, maxRotationSpeed),
                Random.Range(minRotationSpeed, maxRotationSpeed)
            );
            timer = 0f;
        }
    }
}
