using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

// Place this script under an Editor folder
#if UNITY_EDITOR

public class WheelPrefabGenerator : EditorWindow
{
    [Header("Settings")]
    [Tooltip("Core name for your wheel prefabs, without suffix")]
    public string baseName = "Wheel";

    [Tooltip("Reference to a single FBX containing multiple wheel meshes")]
    public GameObject wheelFbx;

    [Tooltip("Suffix for the installed variant")]
    public string installedSuffix = "installed";

    [Tooltip("Suffix for the detached variant")]
    public string detachedSuffix = "detached";

    // Resource subfolders (inside Assets/Resources/Prefabs)
    private const string PrefabRoot = "Assets/Resources/Prefabs";
    private const string BaseFolder = PrefabRoot + "/Base";
    private const string InstalledFolder = PrefabRoot + "/Installed";
    private const string DetachedFolder = PrefabRoot + "/Detached";

    [MenuItem("Tools/Wheel Prefab Generator")]
    public static void ShowWindow()
    {
        GetWindow<WheelPrefabGenerator>("Wheel Prefab Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Wheel Prefab Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        baseName = EditorGUILayout.TextField("Base Name", baseName);
        wheelFbx = (GameObject)EditorGUILayout.ObjectField("Wheel FBX", wheelFbx, typeof(GameObject), false);
        installedSuffix = EditorGUILayout.TextField("Installed Suffix", installedSuffix);
        detachedSuffix = EditorGUILayout.TextField("Detached Suffix", detachedSuffix);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Extracts each wheel mesh from the selected FBX and creates: base prefab + installed and detached variants",
            MessageType.Info);
        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(!ValidateInputs()))
        {
            if (GUILayout.Button("Generate Prefabs", GUILayout.Height(40)))
            {
                GeneratePrefabsFromFbx();
            }
        }

        if (GUILayout.Button("Clear Output Folders", GUILayout.Height(30)))
        {
            ClearOutputFolders();
        }
    }

    bool ValidateInputs()
    {
        return !string.IsNullOrEmpty(baseName) && wheelFbx != null;
    }

    void GeneratePrefabsFromFbx()
    {
        // Ensure output directories
        EnsureFolder(PrefabRoot, "Base");
        EnsureFolder(PrefabRoot, "Installed");
        EnsureFolder(PrefabRoot, "Detached");

        // Instantiate the FBX as a temporary root in-scene
        GameObject root = (GameObject)PrefabUtility.InstantiatePrefab(wheelFbx);
        root.name = wheelFbx.name + "_Root";

        // Find all child meshes (wheels)
        var meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
        int count = 0;

        foreach (var mf in meshFilters)
        {
            // Only process unique wheel meshes
            var wheelObj = mf.gameObject;
            if (wheelObj == root) continue;

            count++;
            string index = count.ToString("D2");
            string wheelName = $"{baseName}_{index}";

            // Create a parent for this wheel
            GameObject wheelGO = new GameObject(wheelName);
            wheelGO.transform.position = wheelObj.transform.position;
            wheelGO.transform.rotation = wheelObj.transform.rotation;

            // Duplicate mesh
            var meshFilter = wheelGO.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mf.sharedMesh;
            var meshRenderer = wheelGO.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = wheelObj.GetComponent<MeshRenderer>().sharedMaterials;

            // Add physics
            AddPhysicsComponents(wheelGO);

            // Save base prefab
            string basePath = Path.Combine(BaseFolder, wheelName + ".prefab");
            var basePrefab = PrefabUtility.SaveAsPrefabAsset(wheelGO, basePath);

            // Create installed variant
            CreateVariant(basePrefab, InstalledFolder, installedSuffix, typeof(ClassA));
            // Create detached variant
            CreateVariant(basePrefab, DetachedFolder, detachedSuffix, typeof(ClassB));

            // Clean up
            GameObject.DestroyImmediate(wheelGO);
        }

        // Clean up temporary root
        GameObject.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Generated {count} wheel prefabs (base + variants).");
    }

    void CreateVariant(GameObject basePrefab, string outFolder, string suffix, System.Type compType)
    {
        string fn = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(basePrefab));
        string variantName = $"{fn}_{suffix}";
        var inst = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
        inst.name = variantName;
        inst.AddComponent(compType);

        string outPath = Path.Combine(outFolder, variantName + ".prefab");
        PrefabUtility.SaveAsPrefabAsset(inst, outPath);
        GameObject.DestroyImmediate(inst);
    }

    void AddPhysicsComponents(GameObject go)
    {
        if (go.GetComponent<MeshCollider>() == null)
            go.AddComponent<MeshCollider>();
        if (go.GetComponent<Rigidbody>() == null)
            go.AddComponent<Rigidbody>();
    }

    void EnsureFolder(string parent, string name)
    {
        string path = Path.Combine(parent, name);
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, name);
    }

    void ClearOutputFolders()
    {
        foreach (var folder in new[] { BaseFolder, InstalledFolder, DetachedFolder })
        {
            if (!Directory.Exists(folder)) continue;
            foreach (var file in Directory.GetFiles(folder, "*.prefab"))
                AssetDatabase.DeleteAsset(file);
        }
        AssetDatabase.Refresh();
        Debug.Log("Cleared generated prefabs.");
    }
}
#endif
