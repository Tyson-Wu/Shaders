using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class WaterMeshBuilder : MonoBehaviour
{
    [SerializeField] bool toBuild = false;
    [SerializeField] float width = 10;
    [SerializeField] float length = 10;
    [SerializeField] float unit = 1;
    [SerializeField] MultiGerstnerWavesSetting setting;
    void build()
    {
        if (!toBuild) return;
        if (setting == null) return;
        toBuild = false;
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = meshFilter.sharedMesh;
        mesh.Clear();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        int x_count = (int)(length / unit);
        int z_count = (int)(width / unit);
        float half_lenght = length / 2;
        float half_width = width / 2;
        float x_pos = 0;
        float z_pos = 0;
        x_pos = -half_lenght;
        // generate vertices
        for (int x = 0; x < x_count; ++x, x_pos+= unit)
        {
            z_pos = -half_width;
            for (int z = 0; z < z_count; ++z, z_pos+=unit)
            {
                vertices.Add(new Vector3(x_pos, 0, z_pos));
                if (x == 0 || z == 0) continue;

                int index = x * z_count + z;
                int i_1_1= index;
                int i_0_1 = i_1_1 - z_count;
                int i_0_0 = i_0_1 - 1;
                int i_1_0 = i_0_0 + z_count;

                triangles.Add(i_1_1);
                triangles.Add(i_0_0);
                triangles.Add(i_0_1);

                triangles.Add(i_0_0);
                triangles.Add(i_1_1);
                triangles.Add(i_1_0);

            }
        }
        
        mesh.vertices = vertices.ToArray();
        mesh.SetIndices(triangles.ToArray(), MeshTopology.Triangles, 0);
        meshFilter.mesh = mesh;
        meshRenderer.material = setting.GetMaterial();
    }
    private void Update()
    {
        build();
    }
}
