using Unity.Entities;
[GenerateAuthoringComponent]

public struct TurretTag : IComponentData
{
    public float shotTimer;
    public bool hasTarget;
    public Entity target;
    public Entity bulletPrefab;
}