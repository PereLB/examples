using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneTemplate;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using MoreMountains.Tools;

public class CreateNewMinigame : EditorWindow
{
    private string minigameName;
    private enum CreationState { None, CreatingAssets, NeedsAttaching, Done}
    CreationState state = CreationState.None;

    [MenuItem("Mini games / Create Minigame")]
    public static void ShowWindow()
    {
        GetWindow<CreateNewMinigame>("Create Minigame").minSize = new Vector2(520f,110f);
    }

    void OnEnable()
    {
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    void OnDisable()
    {
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    public void OnAfterAssemblyReload()
    { 
        if(state == CreationState.CreatingAssets)
            state = CreationState.NeedsAttaching;
        else
            this.Close();
    }


    private void OnGUI()
    {
        switch (state)
        {
            case CreationState.None:
                minigameName = EditorGUILayout.TextField("Minigame Name", minigameName);

                if (GUILayout.Button("Create Minigame"))
                {
                    minigameName = Regex.Replace(minigameName, @"[^a-zA-Z ]", "").Replace(" ","");
                    if (!string.IsNullOrEmpty(minigameName))
                    {
                        state = CreationState.CreatingAssets;
                        minigameName = minigameName[0].ToString().ToUpper() + minigameName.Substring(1);
                        Repaint();
                        StartCreatingMinigame();
                    }
                }
                break;
            case CreationState.CreatingAssets:
                GUILayout.Label("Creating scene and assets, please wait");
                GUILayout.Label("and DON'T CLOSE the window ¬¬");
                break;
            case CreationState.NeedsAttaching:
                GUILayout.Label("Press the button to attach scripts and link them together");
                if (GUILayout.Button("Attach"))
                {                  
                    AttachObjects();
                }
                break;
            case CreationState.Done:
                GUILayout.Label("Done, you can close the window now");
                break;
            default:
                break;
        }
    }


    private void StartCreatingMinigame()
    {
        SceneTemplateAsset levelTemplate = AssetDatabase.LoadAssetAtPath("Assets/_Common/SceneTemplate/MinigameTemplate.scenetemplate", typeof(SceneTemplateAsset)) as SceneTemplateAsset;

        SceneTemplateService.newSceneTemplateInstantiated += CreateAssets;
        SceneTemplateService.Instantiate(levelTemplate, false);
    }

    private void CreateAssets(SceneTemplateAsset sceneTemplateAsset, Scene scene, SceneAsset sceneAsset, bool additiveLoad) {
        SceneTemplateService.newSceneTemplateInstantiated -= CreateAssets;
        scene.name = $"{minigameName}Scene";
        CreateFolderStructure();
        CreateCatAssetsInScene();
        CreateLightningAssetsInScene();
        CreateCanvasAssetsInScene();
        CreateGameManager();
        CreateLoadableScriptableObject();
    }
    

    private void CreateFolderStructure()
    {
        Directory.CreateDirectory(Path.Combine(Application.dataPath, $"Dev/Minigame - {minigameName}"));
        CreateDirectories($"Dev/Minigame - {minigameName}", "2D", "3D", "Animations", "Audio", "Materials", $"{minigameName}Scene", "Prefabs", "ScriptableObjects", "Scripts", "Textures", "UI");
        CreateDirectories($"Dev/Minigame - {minigameName}/UI", "Assets", "Fonts", "Icons", "Prefabs");
        AssetDatabase.Refresh();
    }

    private void CreateDirectories(string root, params string[] dir)
    {
        var fullpath = Path.Combine(Application.dataPath, root);
        foreach (var newDirectory in dir)
        {
            Directory.CreateDirectory(Path.Combine(fullpath, newDirectory));
        }

    }

    private void CreateCatAssetsInScene()
    {
        string localPath = $"Assets/Dev/Minigame - {minigameName}";
        Object _asset = AssetDatabase.LoadAssetAtPath("Assets/pf_Cat_Container.prefab", typeof(Object)) as GameObject;
        GameObject CatContainer = PrefabUtility.InstantiatePrefab(_asset) as GameObject;
        localPath += $"/Prefabs/pf_Cat_Container{minigameName}_variant.prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        CatContainer.transform.SetSiblingIndex(4);

        _asset = AssetDatabase.LoadAssetAtPath("Assets/pf_Cat.prefab", typeof(Object)) as GameObject;
        GameObject Cat = PrefabUtility.InstantiatePrefab(_asset, CatContainer.transform) as GameObject;
        Cat.name = $"pf_Cat_{minigameName}_Variant";
        PrefabUtility.SaveAsPrefabAssetAndConnect(VRCat, localPath, InteractionMode.UserAction);
        EditorUtility.SetDirty(VRCat);
    }

    private void CreateLightningAssetsInScene()
    {
        string localPath = $"Assets/Dev/Minigame - {minigameName}";
        UnityEngine.Object _asset = AssetDatabase.LoadAssetAtPath("Assets/Dev/Minigame/Prefabs/pf_Lighting.prefab", typeof(UnityEngine.Object)) as GameObject;
        GameObject lightning = PrefabUtility.InstantiatePrefab(_asset) as GameObject;
        lightning.name = $"pf_{minigameName}_Lighting";
        lightning.transform.SetSiblingIndex(2);
        localPath += $"/Prefabs/pf_{minigameName}_Lighting.prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        PrefabUtility.UnpackPrefabInstance(lightning, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        PrefabUtility.SaveAsPrefabAssetAndConnect(lightning, localPath, InteractionMode.UserAction);

        VolumeProfile volumeSO = ScriptableObject.CreateInstance<VolumeProfile>();
        string volumePath = $"Assets/Dev/Minigame - {minigameName}/{minigameName}Scene";
        volumePath += $"/{minigameName} Post.asset";
        volumePath = AssetDatabase.GenerateUniqueAssetPath(volumePath);
        AssetDatabase.CreateAsset(volumeSO, volumePath);
    }

    private void CreateCanvasAssetsInScene()
    {
        string localPath = $"Assets/Dev/Minigame - {minigameName}";
        UnityEngine.Object _asset = AssetDatabase.LoadAssetAtPath("Assets/_Common/UI/Prefabs/CommonCanvas.prefab", typeof(UnityEngine.Object)) as GameObject;
        GameObject canvas = PrefabUtility.InstantiatePrefab(_asset) as GameObject;
        canvas.name = $"pf_{minigameName}_Canvas_Variant";
        canvas.transform.SetSiblingIndex(15);
        localPath += $"/UI/Prefabs/pf_{minigameName}_Canvas.prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        PrefabUtility.SaveAsPrefabAssetAndConnect(canvas, localPath, InteractionMode.UserAction);
    }

    private void CreateGameManager()
    {
        string className = minigameName + "_GameManager";

        string contentTemplate = @"using UnityEngine;
using Games.GameFlow;

namespace Games.MiniGame.{1}
{{
    public class {0} : GameManager
    {{
    }}
}}
";
        string localPath = $"Assets/Dev/Minigame - {minigameName}";
        localPath += $"/Scripts/{className}.cs";

        var scriptFile = new StreamWriter(localPath);
        scriptFile.Write(string.Format(contentTemplate, className, minigameName));
        scriptFile.Close();

        AssetDatabase.ImportAsset(localPath, ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.Refresh();
    }

    private void CreateLoadableScriptableObject()
    {
        string localPath = $"Assets/Dev/Minigame - {minigameName}";

        LoadableLevelSO sceneSO = ScriptableObject.CreateInstance<LoadableLevelSO>();
        sceneSO.addressableSceneName = SceneManager.GetActiveScene().name;
        sceneSO.levelName = sceneSO.addressableSceneName.Replace("Scene", "");
        sceneSO.id = char.ToLower(sceneSO.levelName[0]) + sceneSO.levelName[1..];

        localPath += $"/Level_{sceneSO.levelName}.asset";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        AssetDatabase.CreateAsset(sceneSO, localPath);
    }

    private void AttachObjects()
    {
        string localPath = $"Assets/Dev/Minigame - {minigameName}";
        MonoScript _asset = AssetDatabase.LoadAssetAtPath(localPath+$"/Scripts/{minigameName}_GameManager.cs", typeof(UnityEngine.Object)) as MonoScript;
        LoadableLevelSO sceneSO = AssetDatabase.LoadAssetAtPath(localPath + $"/Level_{SceneManager.GetActiveScene().name.Replace("Scene", "")}.asset", typeof(UnityEngine.Object)) as LoadableLevelSO;

        GameObject gameManager = GameObject.Find("Game Manager");
        gameManager.AddComponent(_asset.GetClass());
        gameManager.GetComponent<Games.GameFlow.GameManager>().levelData = sceneSO;

        GameObject Container = GameObject.Find("CatContainer");
        Container.GetComponent<CatBuilder>().m_petReference = GameObject.Find($"pf_Cat_{minigameName}_Variant");
        PrefabUtility.RecordPrefabInstancePropertyModifications(Container.GetComponent<CatBuilder>());

        FindAnyObjectByType<PetInitializer>().pet = Container.GetComponent<CatBuilder>();
        EditorUtility.SetDirty(FindAnyObjectByType<PetInitializer>());

        VolumeProfile volumeSO = AssetDatabase.LoadAssetAtPath(localPath + $"/{minigameName}Scene/{minigameName} Post.asset", typeof(UnityEngine.Object)) as VolumeProfile;
        GameObject lightning = GameObject.Find($"pf_{minigameName}_Lighting");
        lightning.GetComponentInChildren<Volume>().sharedProfile = volumeSO;
        PrefabUtility.RecordPrefabInstancePropertyModifications(lightning.transform.GetChild(1).GetComponent<Volume>());
        PrefabUtility.ApplyPrefabInstance(lightning, InteractionMode.UserAction);


        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), localPath + "/" + SceneManager.GetActiveScene().name + ".unity");
        state = CreationState.Done;
    }
}


