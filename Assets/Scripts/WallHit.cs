using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class WallHit : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

    }
    [BurstCompile]
    struct DotHitWallTrigger : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> allEnemies;
        [ReadOnly] public ComponentDataFromEntity<WallTag> wall;
        [WriteOnly] public EntityCommandBuffer command;

        public void Execute(CollisionEvent collisionEvent)
        {
            if ((allEnemies.Exists(collisionEvent.EntityA) && wall.Exists(collisionEvent.EntityB)) || (allEnemies.Exists(collisionEvent.EntityB) && wall.Exists(collisionEvent.EntityA)))
            {
                Entity enemy;
                if (allEnemies.Exists(collisionEvent.EntityA))
                {
                    enemy = collisionEvent.EntityA;
                }
                else
                {
                    enemy = collisionEvent.EntityB;
                }
            }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DotHitWallTrigger();
        job.allEnemies = GetComponentDataFromEntity<EnemyTag>();
        job.wall = GetComponentDataFromEntity<WallTag>();
        job.command = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }
}
