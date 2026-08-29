using UnityEngine;

public static class RecipeDiscoverySaveSerializer
{
    public static string Serialize(
        RecipeDiscoverySaveData saveData)
    {
        if (saveData == null)
        {
            return string.Empty;
        }

        return JsonUtility.ToJson(saveData);
    }

    public static RecipeDiscoverySaveData Deserialize(
        string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonUtility.FromJson<
            RecipeDiscoverySaveData
        >(json);
    }
}
