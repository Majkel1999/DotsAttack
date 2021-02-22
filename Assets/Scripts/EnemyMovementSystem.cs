using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;

public static class Bit
{
    public static bool isPause = false;
}

public class EnemyMovementSystem : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    private Entity enemyEntity;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;
    private GameObjectConversionSettings gameObjectConversionSettings;
    private float timer = 1.0f;
    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        gameObjectConversionSettings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        enemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, gameObjectConversionSettings);
    }

    private void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0.1f;
            //for (int i = 0; i < 5; i++)
            {

                Entity e = entityManager.Instantiate(enemyEntity);
                entityManager.SetComponentData(e, new Translation
                {
                    Value = new float3(-10f, UnityEngine.Random.Range(-4.5f, 4.5f), 0)
                });
            }
        }
    }
    private void OnDisable()
    {
        blobAssetStore.Dispose();
    }
}

public class MoveSystem : SystemBase
{
    //[SerializeField] float moveSpeed = 1f;
    protected override void OnUpdate()
    {
        if (!Bit.isPause)
        {
            EntityCommandBuffer command = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

            Entities.ForEach((Entity e, ref PhysicsVelocity physicsVelocity, ref Translation translation, ref EnemyTag enemyTag) =>
            {
                float3 speed = new float3(8.5f, 0f, 0f) - translation.Value;
                speed = speed / Mathf.Sqrt(Mathf.Pow(speed.x, 2) + Mathf.Pow(speed.y, 2));
                physicsVelocity.Linear = speed;
                if (translation.Value.x > 8.3)
                {
                    command.DestroyEntity(e);
                }
            }).Run();

            Entities.ForEach((Entity e, ref BulletTag bulletTag) =>
            {
                bulletTag.timer -= Time.DeltaTime;
                if (bulletTag.timer <= 0.0f)
                {
                    command.DestroyEntity(e);
                }
            }).WithoutBurst().Run();
        }
        else
        {
            Entities.ForEach((ref PhysicsVelocity physicsVelocity, ref EnemyTag enemyTag) =>
            {
                enemyTag.isStopped = false;
                physicsVelocity.Linear = new float3(0, 0, 0);
            }).Run();
        }

    }
}
