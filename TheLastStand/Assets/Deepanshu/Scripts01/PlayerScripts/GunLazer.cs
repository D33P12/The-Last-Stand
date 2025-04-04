using UnityEngine;

public class GunLazer : MonoBehaviour
{
    public Transform shootPoint;
    public float laserRange = 100f;
    public Camera primaryCamera;
    public Camera coverCamera;
    [SerializeField]
    private LineRenderer laserLine;
    private Camera _activeCamera;
    private PlayerController _playerController;
    void Start()
    {
        if (laserLine != null)
        {
            laserLine.positionCount = 2;
          
            laserLine.useWorldSpace = true;
            
        }

        _playerController = GetComponent<PlayerController>();
    }
    void Update()
    {
        if (laserLine == null)
        {
            return;
        }
        bool shouldEnableLaser = _playerController.coverState != CoverState.InCover;
        laserLine.enabled = shouldEnableLaser;
        if (shouldEnableLaser)
        {
            _activeCamera = (_playerController.coverState == CoverState.InCoverColliding) ? coverCamera : primaryCamera;

            Vector3 shootDir = _activeCamera.transform.forward;
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
