using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerList : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int poolSize;

    [SerializeField]
    private bool expandable; //Instantiate more objects if initial pool size is too small

    private List<GameObject> freeList;
    private List<GameObject> usedList;


    private void Awake()
    {
        freeList = new List<GameObject>();
        usedList = new List<GameObject>();

        for(int i = 0; i < poolSize; i++)
        {
            GenerateNewObject();
        }
    }

    public GameObject GetObject()
    {
        if (freeList.Count == 0 && !expandable) return null;
        else if (freeList.Count == 0) GenerateNewObject(); 

        int totalFree = freeList.Count;
        GameObject g = freeList[totalFree - 1];
        freeList.RemoveAt(totalFree - 1);
        usedList.Add(g);
        return g;
    }

    // Return an object to the pool
    public void ReturnObject(GameObject obj)
    {
        Debug.Assert(usedList.Contains(obj));
        obj.SetActive(false);
        usedList.Remove(obj);
        freeList.Add(obj);
    }

    // Instantiating GameObject
    private void GenerateNewObject()
    {
        GameObject g = Instantiate(prefab);
        //g.transform.parent = transform;
        g.transform.SetParent(transform);
        g.SetActive(false);
        freeList.Add(g);
    }
}
