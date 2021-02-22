using Unity.Entities;
[GenerateAuthoringComponent]
public struct EnemyTag : IComponentData
{
    public bool isStopped;
    public bool isTargeted;
}
