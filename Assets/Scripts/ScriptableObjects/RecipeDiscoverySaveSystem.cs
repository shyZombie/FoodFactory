using System.IO;
using UnityEngine;

public static class RecipeDiscoverySaveSystem
{
    private const string FileName =
        "recipe_discovery.json";

    private static string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            FileName
        );

    public static void Save(
        RecipeDiscoverySaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        string json =
            RecipeDiscoverySaveSerializer.Serialize(
                saveData
            );

        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        File.WriteAllText(
            SavePath,
            json
        );
    }

    public static RecipeDiscoverySaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            return null;
        }

        string json =
            File.ReadAllText(SavePath);

        return RecipeDiscoverySaveSerializer.Deserialize(
            json
        );
    }
}
