using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.Rendering.Universal;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(CompositeShadowCaster2D))]
public class ShadowCaster2DFromComposite : MonoBehaviour
{
    public bool castsShadows = true;
    public bool selfShadows = true;

    static private GameObject FloorMap;

    static readonly FieldInfo _meshField;
    static readonly FieldInfo _shapePathField;
    static readonly MethodInfo _generateShadowMeshMethod;

    ShadowCaster2D[] _shadowCasters;

    Tilemap _tilemap;
    CompositeCollider2D _compositeCollider;
    List<Vector2> _compositeVerts = new List<Vector2>();

    /// <summary>
    /// Using reflection to expose required properties in ShadowCaster2D
    /// </summary>
    static ShadowCaster2DFromComposite()
    {
        _meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
        _shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);

        _generateShadowMeshMethod = typeof(ShadowCaster2D)
                                    .Assembly
                                    .GetType("UnityEngine.Experimental.Rendering.Universal.ShadowUtility")
                                    .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);
    }

    /// <summary>
    /// Rebuilds ShadowCaster2Ds for all ShadowCaster2DFromComposite in scene
    /// </summary>
    [MenuItem("2DLights/Rebuild Tilemap")]
    public static void RebuildAll()
    {
        foreach (var item in FindObjectsOfType<ShadowCaster2DFromComposite>())
        {
            item.Rebuild();
        }
        FloorMap = GameObject.Find("FloorMap");
        //GameObject Border1 = FloorMap.transform.GetChild(0).gameObject;
        
        Destroy(FloorMap.GetComponent<Transform>().GetChild(0).gameObject);
        Destroy(FloorMap.GetComponent<Transform>().GetChild(1).gameObject);


    }

    private void OnEnable()
    {
        _tilemap = GetComponent<Tilemap>();
        Tilemap.tilemapTileChanged += this.RebuildOnTilePlacement;
    }

    private void OnDisable()
    {
        Tilemap.tilemapTileChanged -= this.RebuildOnTilePlacement;
    }

    private void RebuildOnTilePlacement(Tilemap arg1, Tilemap.SyncTile[] arg2)
    {
        if (arg1 == _tilemap)
            Rebuild();
    }

    private void Start()
    {
        Rebuild();
    }

    /// <summary>
    /// Rebuilds this specific ShadowCaster2DFromComposite
    /// </summary>
    private void Rebuild()
    {
        _compositeCollider = GetComponent<CompositeCollider2D>();
        CreateShadowGameObjects();
        _shadowCasters = GetComponentsInChildren<ShadowCaster2D>();
        for (int i = 0; i < _compositeCollider.pathCount; i++)
        {
            GetCompositeVerts(i);
        }
    }

    /// <summary>
    /// Automatically creates holder gameobjects for each needed ShadowCaster2D, depending on complexity of tilemap
    /// </summary>
    private void CreateShadowGameObjects()
    {
        //Delete all old objects
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).name.Contains("ShadowCaster"))
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
        //Generate new ones
        for (int i = 0; i < _compositeCollider.pathCount; i++)
        {
            GameObject newShadowCaster = new GameObject("ShadowCaster");
            newShadowCaster.transform.parent = transform;
            newShadowCaster.AddComponent<ShadowCaster2D>();
              
        }
    }

    /// <summary>
    /// Gathers all the verts of a given path shape in a CompositeCollider2D
    /// </summary>
    /// <param name="path">The path index to fetch verts from</param>
    private void GetCompositeVerts(int path)
    {
        _compositeVerts = new List<Vector2>();

        Vector2[] pathVerts = new Vector2[_compositeCollider.GetPathPointCount(path)];
        _compositeCollider.GetPath(path, pathVerts);
        _compositeVerts.AddRange(pathVerts);
        UpdateCompositeShadow(_shadowCasters[path]);
    }

    /// <summary>
    /// Sets the verts of each ShadowCaster2D to match their corresponding
    /// verts in CompositeCollider2D and then generates the mesh
    /// </summary>
    /// <param name="caster"></param>
    private void UpdateCompositeShadow(ShadowCaster2D caster)
    {
        caster.castsShadows = castsShadows;
        caster.selfShadows = selfShadows;

        Vector2[] points = _compositeVerts.ToArray();
        var threes = ConvertArray(points);

        _shapePathField.SetValue(caster, threes);
        _meshField.SetValue(caster, new Mesh());
        _generateShadowMeshMethod.Invoke(caster,
            new object[] { _meshField.GetValue(caster),
                _shapePathField.GetValue(caster) });
    }

    //Quick method for converting a Vector2 array into a Vector3 array
    Vector3[] ConvertArray(Vector2[] v2)
    {
        Vector3[] v3 = new Vector3[v2.Length];
        for (int i = 0; i < v3.Length; i++)
        {
            Vector2 tempV2 = v2[i];
            v3[i] = new Vector3(tempV2.x, tempV2.y);
        }
        return v3;
    }


}