using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;


[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class BulletHit : JobComponentSystem
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
    struct BulletHitTriggerSystemJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<BulletTag> allBullets;
        [ReadOnly] public ComponentDataFromEntity<PlayerModelTag> allEnemies;
        [WriteOnly] public EntityCommandBuffer command;


        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            if ((allBullets.Exists(entityA) && allEnemies.Exists(entityB)) || (allBullets.Exists(entityB) && allEnemies.Exists(entityA)))
            {
                command.DestroyEntity(entityB);
                command.DestroyEntity(entityA);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new BulletHitTriggerSystemJob();
        job.allBullets = GetComponentDataFromEntity<BulletTag>();
        job.allEnemies = GetComponentDataFromEntity<PlayerModelTag>();
        var m_EndSimECBSys = World
            .GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        job.command = m_EndSimECBSys.CreateCommandBuffer();

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }
}
