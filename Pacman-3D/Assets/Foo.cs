using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Foo : MonoBehaviour {

    public GameObject prefab;
    List<Vector3> positions = new List<Vector3>();

    [ContextMenu("Save positions")]
    void Save()
    {
        positions.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            positions.Add(transform.GetChild(i).position);
        }
    }

    [ContextMenu("Create assets at positions")]
    void Create()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.transform.position = positions[i];
            go.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
