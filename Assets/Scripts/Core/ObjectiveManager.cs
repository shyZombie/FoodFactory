using System.Collections;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField]
    private ProductionObjectiveData startingObjective;
    [SerializeField]
    private ProductionObjectiveData[] objectives;
    [SerializeField]
    private ProductionObjectiveUI objectiveUI;

    private ProductionObjective currentObjective;
    private int currentObjectiveIndex = -1;

    public ProductionObjective CurrentObjective =>
        currentObjective;

    private void Start()
    {
        if (objectives == null || objectives.Length == 0)
            return;

        StartObjectiveAtIndex(0);
    }

    private void StartObjectiveAtIndex(int index)
    {
        if (index < 0 || index >= objectives.Length)
            return;

        currentObjectiveIndex = index;

        StartObjective(objectives[index]);
    }

    public void StartObjective(
        ProductionObjectiveData objectiveData)
    {
        if (objectiveData == null)
            return;

        if (currentObjective != null)
        {
            currentObjective.OnCompleted -=
                HandleObjectiveCompleted;

            Destroy(currentObjective.gameObject);
            currentObjective = null;
        }

        GameObject objectiveObject =
            new GameObject("ProductionObjective");

        currentObjective =
            objectiveObject.AddComponent<ProductionObjective>();

        currentObjective.OnCompleted +=
            HandleObjectiveCompleted;

        currentObjective.Initialize(objectiveData);

        if (objectiveUI != null)
        {
            objectiveUI.Bind(currentObjective);
        }
    }

    private void HandleObjectiveCompleted(
        ProductionObjective objective)
    {
        /*
        Debug.Log(
            $"OBJECTIVE COMPLETED → " +
            $"{objective.ObjectiveData.name}"
        );
        */

        if (objective != currentObjective)
            return;

        StartCoroutine(
            StartNextObjectiveNextFrame()
        );
    }

    private IEnumerator StartNextObjectiveNextFrame()
    {
        yield return null;

        int nextIndex =
            currentObjectiveIndex + 1;

        if (nextIndex >= objectives.Length)
            yield break;

        StartObjectiveAtIndex(nextIndex);
    }
}
