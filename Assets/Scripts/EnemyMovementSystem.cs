using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;

public class EnemyMovementSystem : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    private float timer = 1.0f;
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 1.0f;
            Instantiate(enemyPrefab, new Vector3(-10f, UnityEngine.Random.Range(-4.5f, 4.5f), 0), new Quaternion());
        }
    }
}
public class MoveSystem : ComponentSystem
{
    [SerializeField] float moveSpeed = 1f;
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerModelTag>().ForEach((ref PhysicsVelocity physicsVelocity, ref Translation translation) =>
        {
            float3 speed = new float3(8.5f, 0f, 0f) - translation.Value;
            speed = speed / Mathf.Sqrt(Mathf.Pow(speed.x, 2) + Mathf.Pow(speed.y, 2));
            physicsVelocity.Linear = speed;
                //translation.Value.x += moveSpeed * Time.DeltaTime;
            });

    }
}
