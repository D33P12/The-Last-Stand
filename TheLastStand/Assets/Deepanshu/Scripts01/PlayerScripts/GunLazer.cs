using UnityEngine;

public class GunLazer : MonoBehaviour
{
    public Transform shootPoint;
    public float laserRange = 100f;
    public Camera primaryCamera;
    public Camera coverCamera;
    [SerializeField]
    private LineRenderer laserLine;
    [SerializeField]
    private Color laserColor = Color.blue; 
    [SerializeField]
    private float startWidth = 0.1f; 
    [SerializeField]
    private float endWidth = 0.1f; 
    private Camera activeCamera;
    private PlayerController playerController;
    void Start()
    {
        laserLine = gameObject.AddComponent<LineRenderer>();
        laserLine.positionCount = 2;
        laserLine.startWidth = startWidth;
        laserLine.endWidth = endWidth;
        laserLine.useWorldSpace = true;
        laserLine.enabled = true;

        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(laserColor, 0.0f);
        colorKeys[1] = new GradientColorKey(laserColor, 1.0f);
        gradient.colorKeys = colorKeys;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);
        gradient.alphaKeys = alphaKeys;

        laserLine.colorGradient = gradient;
        playerController = GetComponent<PlayerController>();
    }
    void Update()
    {
        laserLine.enabled = playerController._coverState != CoverState.InCover;
        if (laserLine.enabled)
        {
            if (playerController._coverState == CoverState.InCoverColliding)
            {
                activeCamera = coverCamera;
            }
            else
            {
                activeCamera = primaryCamera;
            }
            Vector3 shootDir = activeCamera.transform.forward;
            Ray ray = new Ray(shootPoint.position, shootDir);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, laserRange))
            {
                LaserTarget laserTarget = hit.collider.GetComponent<LaserTarget>();
                if (laserTarget != null)
                {
                    laserLine.SetPosition(0, shootPoint.position);
                    laserLine.SetPosition(1, hit.point);

                    laserTarget.OnHitByLaser();
                    return;
                }
            }
            laserLine.SetPosition(0, shootPoint.position);
            laserLine.SetPosition(1, shootPoint.position + shootDir * laserRange);
        }
    }
}
