using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;

    private Dictionary<GridPosition, GridObject> gridObjects =
        new Dictionary<GridPosition, GridObject>();

    private void Awake()
    {
        RegisterExistingGridObjects();
    }

    private void RegisterExistingGridObjects()
    {
        GridObject[] existingObjects =
            FindObjectsByType<GridObject>(
                FindObjectsSortMode.None
            );

        foreach (GridObject gridObject in existingObjects)
        {
            GridPosition gridPosition =
                WorldToGridPosition(gridObject.transform.position);

            if (gridObjects.ContainsKey(gridPosition))
            {
                Debug.LogWarning(
                    $"Multiple GridObjects found at {gridPosition}."
                );

                continue;
            }

            gridObjects.Add(
                gridPosition,
                gridObject
            );

            gridObject.SetGridPosition(gridPosition);

            Machine machine =
                gridObject.GetComponent<Machine>();

            if (machine != null)
            {
                machine.Initialize(this);
            }
        }
    }

    public GridPosition WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(
            worldPosition.x / cellSize
        );

        int y = Mathf.FloorToInt(
            worldPosition.y / cellSize
        );

        return new GridPosition(x, y);
    }

    public Vector3 GridToWorldPosition(
        GridPosition gridPosition)
    {
        float x =
            gridPosition.x * cellSize +
            cellSize / 2f;

        float y =
            gridPosition.y * cellSize +
            cellSize / 2f;

        return new Vector3(
            x,
            y,
            0f
        );
    }

    public bool IsCellOccupied(
        GridPosition gridPosition)
    {
        return gridObjects.ContainsKey(
            gridPosition
        );
    }

    public bool TryAddGridObject(
        GridPosition gridPosition,
        GridObject gridObject)
    {
        if (IsCellOccupied(gridPosition))
        {
            return false;
        }

        gridObjects.Add(
            gridPosition,
            gridObject
        );

        gridObject.SetGridPosition(
            gridPosition
        );

        Machine machine =
            gridObject.GetComponent<Machine>();

        if (machine != null)
        {
            machine.Initialize(this);
        }

        return true;
    }

    public GridObject GetGridObject(
        GridPosition gridPosition)
    {
        if (gridObjects.TryGetValue(
                gridPosition,
                out GridObject gridObject))
        {
            return gridObject;
        }

        return null;
    }

    public void RemoveGridObject(
        GridPosition gridPosition)
    {
        gridObjects.Remove(
            gridPosition
        );
    }
}