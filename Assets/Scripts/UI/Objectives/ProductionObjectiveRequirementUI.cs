using TMPro;
using UnityEngine;

public class ProductionObjectiveRequirementUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text categoryText;

    [SerializeField]
    private TMP_Text progressText;

    public void SetProgress(
        ProductionObjectiveProgress progress)
    {
        if (categoryText != null)
        {
            categoryText.text =
                progress.Category.ToString();
        }

        if (progressText != null)
        {
            string completionMarker =
                progress.IsCompleted ? " ✓" : "";

            progressText.text =
                $"{progress.Current}/{progress.Required}" +
                completionMarker;
        }
    }
}