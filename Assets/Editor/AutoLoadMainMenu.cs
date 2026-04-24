using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoLoadMainMenu
{
    static AutoLoadMainMenu()
    {
        EditorSceneManager.playModeStartScene =
            AssetDatabase.LoadAssetAtPath<SceneAsset>(
                "Assets/Scenes/MainMenu.unity"
            );
    }
}