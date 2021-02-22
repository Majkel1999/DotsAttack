using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

public class SlotControlller : MonoBehaviour
{
    [SerializeField] private GameObject turretPrefab;

    private Entity turretEntity;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;
    private GameObjectConversionSettings gameObjectConversionSettings;
    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        gameObjectConversionSettings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        turretEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(turretPrefab, gameObjectConversionSettings);
        Input.multiTouchEnabled = false;
    }
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            Physics.Raycast(ray, out hit);
            if (hit.collider.CompareTag("Slot") && !hit.collider.gameObject.GetComponent<SlotData>().hasTurret)
            {
                Debug.Log("Turret spawned");
                hit.collider.gameObject.GetComponent<SlotData>().hasTurret = true;
                Entity turret = entityManager.Instantiate(turretEntity);
                entityManager.SetComponentData(turret, new Translation
                {
                    Value = new Unity.Mathematics.float3(hit.point.x, hit.point.y, 0)
                });
            }

        }
    }
    private void OnDisable()
    {
        blobAssetStore.Dispose();
    }
}
