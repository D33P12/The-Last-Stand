using UnityEngine;

public class LaserTargetCall :  LaserTarget
{
    public override void OnHitByLaser()
    {
        Debug.Log(" hit by laser");
    }
}
