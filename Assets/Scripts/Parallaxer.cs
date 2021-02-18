using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Parallaxer : MonoBehaviour
{
   class PoolObject
    {
        public Transform transform;
        public bool inUse;                                      //determine if it's in use or not
        public PoolObject(Transform t) { transform = t; }       //constructor
        public void Use() { inUse = true; }                     
        public void Dispose() { inUse = false; }
    }

    [System.Serializable]
    public struct YspawnRange
    {
        public float minY;
        public float maxY;
    }

    public GameObject Prefab;                                   //what time of prefab we're spawning
    public int poolSize;                                        //what size of pool, how many objects should we spawn
    public float shiftSpeed;                                    //how fast they're moving 
    public float spawnRate;                                     //how fast objects are spawning

    public YspawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate;                                 //have clouds and stars immediately spawn (particle prewarm)
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;
    GameManager game;

    void Awake()
    {
        Configure();
    }

    void Start()
    {
        game = GameManager.Instance;
    }

    void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;

    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;

    }

    void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        Configure();
        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Update()
    {
        if (game.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure()                                                //determines target aspect during game play (10:16 ratio)
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();   
        }
    }

    void Spawn()
    {
        Transform t = GetPoolObject();
        if (t == null) return;          //if true, this indicates that poolSize is too small
        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        t.position = pos;
    }

    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null) return;          //if true, this indicates that poolSize is too small
        Vector3 pos = Vector3.zero;
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
        t.position = pos;
        Spawn();
    }

    void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.localPosition += Vector3.right * shiftSpeed * Time.deltaTime;      //shifts object right to left
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)              //check if parallax object is off screen
    {
        if(poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect) 
        {
            poolObject.Dispose();                               //in use is now false
            poolObject.transform.position = Vector3.one * 1000; //put object way off screen
        }
    }

    Transform GetPoolObject()                                   //get available pool object
    {
        for (int i = 0; i < poolObjects.Length; i++)            //get first available
        {
            if (!poolObjects[i].inUse)                          //if not in use
            {
                poolObjects[i].Use();                           //marks object as used
                return poolObjects[i].transform;                          //return pool object
            }
        }
        return null;
    }




}

