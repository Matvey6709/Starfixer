using UnityEditor;
using UnityEngine;

public static class SaveDataMenu
{
    private const string SaveKey = "SaveData";

    [MenuItem("Tools/Reset Save Data")]
    public static void ResetSaveData()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Reset Save Data",
            "Полностью сбросить сохранения?\n\n" +
            "• PlayerPrefs ключ \"SaveData\" будет удалён.\n" +
            "• Флаг introWatched обнулится → видео интро снова покажется при первом клике \"Играть\".\n" +
            "• Если игра сейчас в Play Mode — данные в памяти тоже сбросятся.",
            "Сбросить",
            "Отмена");

        if (!confirm) return;

        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();

        if (Application.isPlaying && DataManager.Instance != null)
        {
            DataManager.Instance.gameData = new GameData();
            Debug.Log("[SaveDataMenu] Память DataManager обнулена.");
        }

        Debug.Log("[SaveDataMenu] Сохранения сброшены. Интро снова покажется при первом нажатии \"Играть\".");
    }
}
