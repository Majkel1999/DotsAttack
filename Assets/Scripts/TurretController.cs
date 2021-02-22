using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public class TurretController : SystemBase
{

    protected override void OnUpdate()
    {
        EntityCommandBuffer command = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
        float delta = Time.DeltaTime;
        Entities.ForEach((ref Rotation rotation, ref TurretTag turretTag,ref Translation translation) =>
        {/*
            if (!turretTag.hasTarget)
            {
                float3 position = translation.Value;
                float3 closestPosition = float3.zero;
                Entity closest = Entity.Null;

                Entities.ForEach((Entity e, Translation translationEnemy, ref EnemyTag enemyTag) =>
                {
                    if (closest == Entity.Null)
                    {
                        closest = e;
                        closestPosition = translationEnemy.Value;
                    }
                    else
                    {
                        if (math.distance(position, translationEnemy.Value) < math.distance(position, closestPosition))
                        {
                            closest = e;
                            closestPosition = translationEnemy.Value;
                        }
                    }

                }).Run();
                turretTag.target = closest;
            }*/
            turretTag.shotTimer -= delta;
            /*
            float3 targetPosition = GetComponent<Translation>(turretTag.target).Value;
            float x = translation.Value.x - targetPosition.x;
            float y = translation.Value.y - targetPosition.y;

            float angle = math.atan2(y, x);
            angle = math.degrees(angle);
            rotation.Value = new float3(0, 0, angle);
            */
            //rotation.Value = new quaternion()
            if (turretTag.shotTimer <= 0.0f)
            {
                Entity bullet = command.Instantiate(turretTag.bulletPrefab);
                World.EntityManager.SetComponentData(bullet, new Translation
                {
                    Value = new float3(translation.Value.x, translation.Value.y, 0)
                });
                World.EntityManager.SetComponentData(bullet, new Rotation { Value = rotation.Value });
                World.EntityManager.SetComponentData(bullet, new PhysicsVelocity
                {
                    Linear = new float3(math.cos(rotation.Value.value.z), math.sin(rotation.Value.value.x), 0)
                });
                turretTag.shotTimer = 1.0f;
            }
        }).WithoutBurst().Run();
    }

}
