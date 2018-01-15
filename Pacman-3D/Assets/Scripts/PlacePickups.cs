using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePickups : MonoBehaviour
{
    public GameObject pickup;

    public float minX;
    public float maxX;

    public float minY;
    public float maxY;

    public int amountX;
    public int amountY;

    List<Vector3> places;

    [ContextMenu("Calculate and place stuff")]
    public void Place()
    {
        ClearPickups();
        places = new List<Vector3>();
        float stepX = (maxX - minX) / amountX;
        float stepY = (maxY - minY) / amountY;

        for (float y = minY + 0.5f * stepY; y <= maxY; y += stepY)
        {
            for (float x = minX + 0.5f * stepX; x <= maxX; x += stepX)
            {
                Vector3 pos = new Vector3(x, 0, y);
                if (IsSpaceEmpty(pos))
                    places.Add(pos);
            }
        }

        foreach (Vector3 position in places)
        {
            Instantiate(pickup, position, Quaternion.identity, transform);
        }
    }

    [ContextMenu("Place stuff in saved positions")]
    public void Place2()
    {
        ClearPickups();

        foreach (Vector3 position in SavedPositions)
        {
#if UNITY_EDITOR
            GameObject go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(pickup);
#else
            GameObject go = Instantiate(pickup);
#endif
            go.transform.position = position;
            go.transform.parent = transform;
            //Instantiate(pickup, position, Quaternion.identity, transform);
        }
    }

    public bool IsSpaceEmpty(Vector3 position)
    {
        Ray ray = new Ray(new Vector3(position.x, 10f, position.z), Vector3.down);
        return !Physics.SphereCast(ray, 0.1f);
    }

    [ContextMenu("Clear stuff")]
    public void ClearPickups()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public List<Vector3> SavedPositions;

    [ContextMenu("Save positions")]
    public void SavePositions()
    {
        SavedPositions = new List<Vector3>();

        for (int i = 0; i < transform.childCount; i++)
        {
            SavedPositions.Add(transform.GetChild(i).position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //if (places.Count > 0)
        //{
        //    foreach (Vector3 pos in places)
        //    {
        //        Gizmos.DrawSphere(pos, 0.3f);
        //    }
        //}
    }
}
