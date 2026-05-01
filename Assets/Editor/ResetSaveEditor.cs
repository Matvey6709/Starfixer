using UnityEditor;
using UnityEngine;

public static class ResetSaveEditor
{
    [MenuItem("Tools/Reset Save Data")]
    public static void ResetSave()
    {
        PlayerPrefs.DeleteKey("SaveData");
        Debug.Log("Save data cleared.");
    }
}
