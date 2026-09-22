using UnityEngine;

public static class DeliveryTrackerSaveSystem
{
    private const string SaveKey =
        "DeliveryTracker_SaveData";

    public static void Save(
        DeliveryTrackerSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        string json =
            JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString(
            SaveKey,
            json
        );

        PlayerPrefs.Save();
    }

    public static DeliveryTrackerSaveData Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            return null;
        }

        string json =
            PlayerPrefs.GetString(SaveKey);

        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonUtility.FromJson<DeliveryTrackerSaveData>(
            json
        );
    }
}
