# Wheel Prefab Generator for Unity

A simple Unity Editor tool that automates extracting wheel meshes from a single FBX file and generates prefabs with physics components and two variant scripts.

## Features

* **Single-FBX Extraction**: Drag and drop an FBX containing multiple wheels, and the script will auto-detect all wheel meshes.
* **Prefab Variants**: For each wheel, create three prefabs:

  1. **Base** – default setup with MeshCollider and Rigidbody.
  2. **Installed** – adds your custom `ClassA` component.
  3. **Detached** – adds your custom `ClassB` component.
* **Automatic Naming & Organization**:

  * Prefab names follow the pattern: `<BaseName>_XX[_installed|_detached]`, where `XX` is a two-digit index.
  * Generated prefabs are saved under `Assets/Resources/Prefabs/{Base, Installed, Detached}`.
* **Inspector Configuration**: Set `Base Name`, `Wheel FBX`, and custom suffixes (`installed`/`detached`) directly in the Editor window.
* **Clean-up Utility**: One click to clear all generated prefabs from the output folders.

## Prerequisites

* Unity 2020.3 LTS or newer.
* Place the script inside any folder named **Editor** (e.g., `Assets/Editor/WheelPrefabGenerator.cs`).
* Create two MonoBehaviour scripts `ClassA.cs` and `ClassB.cs` (in a non-Editor folder) to define your installed/detached behaviors.

```csharp
// ClassA.cs
using UnityEngine;
public class ClassA : MonoBehaviour { /* Your installed logic */ }

// ClassB.cs
using UnityEngine;
public class ClassB : MonoBehaviour { /* Your detached logic */ }
```

## Installation

1. Copy `WheelPrefabGenerator.cs` to an `Editor` folder in your Unity project.
2. Create `ClassA.cs` and `ClassB.cs` in a runtime folder (e.g., `Assets/Scripts/`).
3. Save your FBX file containing wheel meshes anywhere under `Assets`.

## Usage

1. In Unity, go to `Tools → Wheel Prefab Generator`.
2. In the window that appears:

   * **Base Name**: Enter the prefix for prefab names (e.g., `wheel`).
   * **Wheel FBX**: Drag your FBX file into this field.
   * **Installed Suffix / Detached Suffix**: (Optional) customize variant names.
3. Click **Generate Prefabs**. The tool will:

   * Instantiate the FBX in a hidden scene.
   * Extract each mesh into its own prefab with physics.
   * Save Base, Installed, and Detached prefabs in the corresponding folders.
   * Clean up temporary objects.
4. Find your prefabs under `Assets/Resources/Prefabs/Base`, `.../Installed`, and `.../Detached`.
5. To clear all generated prefabs, click **Clear Output Folders** in the same window.

## Folder Structure

```
Assets/
├─ Editor/
│  └─ WheelPrefabGenerator.cs
├─ Resources/
│  └─ Prefabs/
│     ├─ Base/
│     ├─ Installed/
│     └─ Detached/
└─ Scripts/
   ├─ ClassA.cs
   └─ ClassB.cs
```

## Tips & Troubleshooting

* **Missing Script**: Ensure `ClassA.cs` and `ClassB.cs` are in a non-Editor folder and each file name matches the class name.
* **Mesh Not Found**: The FBX must contain child `MeshFilter` components; nested empties without meshes are ignored.
* **Collider Settings**: Adjust the `MeshCollider` properties on generated prefabs if you need convex or specific physics settings.

## License

MIT License. Feel free to modify and adapt for your project.
