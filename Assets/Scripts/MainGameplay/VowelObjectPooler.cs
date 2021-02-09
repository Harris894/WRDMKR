using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VowelObjectPooler : MonoBehaviour
{

    #region Singleton
    public static VowelObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public int poolSizes;
    public Transform vowelParent;
    public List<Letters> vowels;
    public Dictionary<char, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<char, Queue<GameObject>>();

        foreach (var vowel in vowels)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolSizes; i++)
            {
                GameObject obj = Instantiate(vowel.icon,vowelParent);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(vowel.letter, objectPool);
        }
    }

    //Method that takes the already instantiated and deactivated letter and places it into the right place and activates it.
    public GameObject SpawnFromPool(char tag, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag" + tag + "doesn't exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetParent(parent);

        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
        poolDictionary[tag].Enqueue(objectToSpawn);

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        return objectToSpawn;
    }
}
