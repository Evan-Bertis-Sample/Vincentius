using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BackgroundObjectSpawner : MonoBehaviour
{
    class BackgroundObject
    {
        public Vector2 spawnOffset;
        public int spawnsSinceUse;
        public Sprite sprite;

        public BackgroundObject(Sprite sprite)
        {
            this.sprite = sprite;
            spawnOffset = CalculateSpawnOffset();
            spawnsSinceUse = 0;
        }
        
        private Vector2 CalculateSpawnOffset()
        {
            return new Vector2(0, -sprite.bounds.min.y);
        }
    }

    [Header("Scene GUI Settings")]
    public Color handleColor = Color.red;
    public float handleSize = 0.5f;
    
    [Header("Object Settings")]
    public Sprite[] objectSprites;
    public float objectDensity;
    public Material objectMaterial;
    public string spriteLayer;
    public int orderInLayer = 0;
    public Color spriteColor = Color.white;
    public bool randomlyFlipSprites;

    [Header("Spawn Settings")]
    public AnimationCurve spawnCurve;
    public int minIterationsUntilRepeat = 2;
    public float minXSpacing = 0.5f;
    public Vector3 globalSpawnOffset = new Vector3(0, -0.1f, 0);
    public bool attachToGround = true;
    public LayerMask groundMask;

    private List<BackgroundObject> spawnableObjects = new List<BackgroundObject>();
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Vector3> spawnPoints = new List<Vector3>();

    [Header("Handles")]
    public Vector3 leftBound;
    public Vector3 rightBound;

    public void GenerateSpawnableObjects()
    {
        spawnableObjects = new List<BackgroundObject>();
        for(int i = 0; i < objectSprites.Length; i++)
        {
            spawnableObjects.Add(new BackgroundObject(objectSprites[i]));
        }
    }

    public void SpawnObjects()
    {
        float distance = rightBound.x - leftBound.x;
        int numObjects = Mathf.RoundToInt(objectDensity * distance);

        spawnPoints = GenerateSpawnPoints(numObjects);

        for (int i = 0; i < numObjects; i++)
        {
            GameObject newObject = new GameObject($"Object: {spawnedObjects.Count}");
            SpriteRenderer newSr = newObject.AddComponent<SpriteRenderer>();
            BackgroundObject spriteObj = ChoseSpriteObject();
            
            newSr.sprite = spriteObj.sprite;
            newSr.sortingLayerName = spriteLayer;
            newSr.sortingOrder = orderInLayer;
            newSr.material = objectMaterial;
            newSr.color = spriteColor;
            if (randomlyFlipSprites) newSr.flipX = (Random.Range(0f, 1f) > 0.5f);

            newObject.transform.position = (Vector3)spriteObj.spawnOffset + spawnPoints[i] + globalSpawnOffset;
            newObject.transform.parent = transform;

            spawnedObjects.Add(newObject);
            
        }
    }

    public void ClearObjects()
    {
        List<GameObject> toDestroy = new List<GameObject>();
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("Object: "))
            {
                toDestroy.Add(transform.GetChild(i).gameObject);
            }
        }

        foreach(GameObject g in toDestroy)
        {
            DestroyImmediate(g);
        }

        spawnedObjects.Clear();
    }

    private List<Vector3> GenerateSpawnPoints(int numPoints)
    {
        List<Vector3> spawnPoints = new List<Vector3>();

        for (int i = 0; i < numPoints; i++)
        {
            bool found = false;
            float spawnT = 0;
            while(found == false)
            {
                float t = Random.Range(0f, 1f);
                float curveValue = spawnCurve.Evaluate(t);
                float compareValue = Random.Range(0f, 1f);

                if (curveValue >= compareValue)
                {
                    found = true;
                    spawnT = t;
                }
            }

            Vector3 originalPoint =  Vector3.Lerp(leftBound, rightBound, spawnT);

            spawnPoints.Add(originalPoint);
        }

        //Sort through points
        spawnPoints.Sort((a, b) => a.x.CompareTo(b.x));

        //Spread Points
        for (int k = 0; k < 20; k++)
        {
            for(int i = 1; i < spawnPoints.Count; i++)
            {
                Vector3 left = spawnPoints[i - 1];
                Vector3 current = spawnPoints[i];

                float distance = current.x - left.x;

                if (distance < minXSpacing)
                {
                    float midPoint = Vector3.Lerp(left, current, 0.5f).x;
                    left.x = midPoint - (minXSpacing / 2);
                    current.x = midPoint + (minXSpacing / 2);
                }

                if(left.x < leftBound.x)
                {
                    left.x = leftBound.x;
                }

                if (current.x > rightBound.x)
                {
                    current.x = rightBound.x;
                }

                spawnPoints[i - 1] = left;
                spawnPoints[i] = current;
            }
        }
        if (attachToGround)
        {
            for(int i = 0; i < spawnPoints.Count ; i ++)
            {
                RaycastHit2D groundRay = Physics2D.Raycast(spawnPoints[i], Vector2.down, 5 , groundMask);
                spawnPoints[i] = groundRay.point;
            }
        }

        return spawnPoints;
    }

    private BackgroundObject ChoseSpriteObject()
    {
        List<BackgroundObject> possibleObjects = new List<BackgroundObject>();

        foreach(BackgroundObject s in spawnableObjects)
        {
            if (s.spawnsSinceUse >= minIterationsUntilRepeat)
            {
                possibleObjects.Add(s);
            }
        }

        BackgroundObject chosen = null;

        if(possibleObjects.Count == 0)
        {
            //There were no objects that fit the min iterations count
            //Choose a random one
            chosen = spawnableObjects[Random.Range(0, spawnableObjects.Count - 1)];
        }
        else 
        {
            //There were objects that fit the requirement
            chosen =  possibleObjects[Random.Range(0, spawnableObjects.Count - 1)];
        }

        //Update spawnSinceUseValues
        foreach(BackgroundObject s in spawnableObjects)
        {
            if (s == chosen) s.spawnsSinceUse = 0;;
            s.spawnsSinceUse++;
        }

        return chosen;
    }
}
