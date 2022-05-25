using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joinMesh : MonoBehaviour
{
    public List<MeshFilter> meshFilters = new List<MeshFilter>();
    public LayerMask layer;
    void Start()
    {
        GameObject[] objs = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (var obj in objs)
        {
            if (obj.GetComponent<MeshFilter>() != null)// && obj.layer == layer)
            {
                meshFilters.Add(obj.GetComponent<MeshFilter>());
                Debug.Log(meshFilters.Count);
            }
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Count];

        int i = 0;
        while (i < meshFilters.Count)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
}
