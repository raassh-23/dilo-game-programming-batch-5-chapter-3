using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
{
    [Header("Templates")]
    public List<TerrainTemplateController> terrainTemplates;
    public float terrainTemplateWidth;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffset;
    public float areaEndOffset;

    private const float debugLineLength = 10.0f;

    private List<GameObject> spawnedTerrains;

    [Header("Force Early Template")]
    public List<TerrainTemplateController> earlyTerrainTemplates;

    private float lastGeneratedPositionX;
    private float lastRemovedPositionX;

    private Dictionary<string, List<GameObject>> pool;

    private void Start() {
        pool = new Dictionary<string, List<GameObject>>();
        spawnedTerrains = new List<GameObject>();

        lastGeneratedPositionX = GetHorizontalPositionStart();
        lastRemovedPositionX = lastGeneratedPositionX - terrainTemplateWidth;

        foreach (TerrainTemplateController terrain in earlyTerrainTemplates) {
            GenerateTerrain(lastGeneratedPositionX, terrain);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while (lastGeneratedPositionX < GetHorizontalPositionEnd()) {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
    }

    private void Update() {
        while (lastGeneratedPositionX < GetHorizontalPositionEnd()) {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }

        while (lastRemovedPositionX + terrainTemplateWidth < GetHorizontalPositionStart()) {
            lastRemovedPositionX += terrainTemplateWidth;
            RemoveTerrain(lastRemovedPositionX);
        }
    }

    private GameObject GenerateFromPool(GameObject item, Transform parent) {
        if (pool.ContainsKey(item.name)) {
            if(pool[item.name].Count > 0) {
                GameObject newItemFromPool = pool[item.name][0];
                pool[item.name].Remove(newItemFromPool);
                newItemFromPool.SetActive(true);
                return newItemFromPool;
            }
        } else {
            pool.Add(item.name, new List<GameObject>());
        }

        GameObject newItem = Instantiate(item, parent);
        newItem.name = item.name;
        return newItem;
    }

    private void ReturnToPool(GameObject item)
    {
        if(!pool.ContainsKey(item.name)) {
            Debug.LogError("Invalid pool item. Pool does not contain item: " + item.name);
        }

        pool[item.name].Add(item);
        item.SetActive(false);
    }

    private void GenerateTerrain(float posX, TerrainTemplateController forceterrain = null) {
        GameObject newTerain;

        if(forceterrain == null) {
            newTerain = GenerateFromPool(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);
        } else {
            newTerain = GenerateFromPool(forceterrain.gameObject, transform);;
        }

        newTerain.transform.position = new Vector2(posX, -4.3f);

        spawnedTerrains.Add(newTerain);
    }

    private void RemoveTerrain(float posX) {
        GameObject terrainToRemove = null;

        foreach (GameObject terrain in spawnedTerrains) {
            if (terrain.transform.position.x == posX) {
                terrainToRemove = terrain;
                break;
            }
        }

        if (terrainToRemove != null) {
            spawnedTerrains.Remove(terrainToRemove);
            ReturnToPool(terrainToRemove);
        }
    }

    private float GetHorizontalPositionStart() {
        return gameCamera.ViewportToWorldPoint(new Vector2(0.0f, 0.0f)).x + areaStartOffset;
    }

    private float GetHorizontalPositionEnd() {
        return gameCamera.ViewportToWorldPoint(new Vector2(1.0f, 0.0f)).x + areaEndOffset;
    }

    private void OnDrawGizmos() {
        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        Debug.DrawLine(areaStartPosition + Vector3.up * debugLineLength / 2, areaStartPosition + Vector3.down * debugLineLength / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * debugLineLength / 2, areaEndPosition + Vector3.down * debugLineLength / 2, Color.red);
    }
}
