using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotSpeed=4;

    private Entity bulletEntity;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;
    private GameObjectConversionSettings gameObjectConversionSettings;
    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        gameObjectConversionSettings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        bulletEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, gameObjectConversionSettings);
    }
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 destination = mousePos;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (Input.GetMouseButtonDown(0))
        {
            Shoot(mousePos.normalized);

        }
    }


    void Shoot(Vector3 destination)
    {
        //Vector3 speed = (destination - transform.position).normalized;
        destination *= shotSpeed;
        Entity bullet = entityManager.Instantiate(bulletEntity);
        entityManager.SetComponentData(bullet, new PhysicsVelocity
        {
            Linear = new float3(destination.x, destination.y, 0)
        });
        entityManager.SetComponentData(bullet, new Translation
        {
            Value = new float3(transform.position.x, transform.position.y, 0)
        });
        entityManager.SetComponentData(bullet, new Rotation { Value = transform.rotation });
    }

    private void OnDisable()
    {
        blobAssetStore.Dispose();
    }
}
