using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;

    private Dictionary<GridPosition, GridObject> gridObjects =
        new Dictionary<GridPosition, GridObject>();

    private Dictionary<GridPosition, Extractor> extractors =
        new Dictionary<GridPosition, Extractor>();

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

            if (!TryAddGridObject(
                    gridPosition,
                    gridObject))
            {
                Debug.LogWarning(
                    $"Could not register {gridObject.name} " +
                    $"at {gridPosition}."
                );
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

    private GridPosition GetFootprintCell(
    GridPosition origin,
    GridObject gridObject,
    int x,
    int y)
    {
        int offsetX =
            x - gridObject.Width / 2;

        int offsetY =
            y - gridObject.Height / 2;

        return new GridPosition(
            origin.x + offsetX,
            origin.y + offsetY
        );
    }

    private bool CanExtractorOverlapSpawner(
        GridPosition origin,
        GridObject gridObject)
    {
        Extractor extractor =
            gridObject.GetComponent<Extractor>();

        if (extractor == null)
            return false;

        if (gridObject.Width != 3 ||
            gridObject.Height != 3)
            return false;

        GridObject centerObject =
            GetGridObject(origin);

        if (centerObject == null)
            return false;

        FoodSpawner foodSpawner =
            centerObject.GetComponent<FoodSpawner>();

        if (foodSpawner == null)
            return false;

        if (foodSpawner.Width != 3 ||
            foodSpawner.Height != 3)
            return false;

        return true;
    }

    public bool CanPlaceGridObject(
        GridPosition origin,
        GridObject gridObject)
    {
        if (gridObject == null)
            return false;

        bool extractorCanOverlapSpawner =
            CanExtractorOverlapSpawner(
                origin,
                gridObject
            );

        for (int x = 0; x < gridObject.Width; x++)
        {
            for (int y = 0; y < gridObject.Height; y++)
            {
                GridPosition cellPosition =
                    GetFootprintCell(
                        origin,
                        gridObject,
                        x,
                        y
                    );

                if (!IsCellOccupied(cellPosition))
                    continue;

                if (extractorCanOverlapSpawner)
                    continue;

                return false;
            }
        }

        return true;
    }

    private void RegisterExtractor(
        GridPosition origin,
        Extractor extractor)
    {
        for (int x = 0; x < extractor.Width; x++)
        {
            for (int y = 0; y < extractor.Height; y++)
            {
                GridPosition cellPosition =
                    GetFootprintCell(
                        origin,
                        extractor,
                        x,
                        y
                    );

                extractors[cellPosition] = extractor;
            }
        }

        extractor.SetGridPosition(origin);
    }

    public bool TryAddGridObject(
        GridPosition origin,
        GridObject gridObject)
    {
        if (!CanPlaceGridObject(
                origin,
                gridObject))
        {
            return false;
        }

        Extractor extractor =
            FindFirstObjectByType<Extractor>(
                FindObjectsInactive.Include
            );
        if (extractor != null)
        {
            extractor.Initialize(this);
        }

        if (extractor != null &&
            CanExtractorOverlapSpawner(
                origin,
                gridObject))
        {
            RegisterExtractor(
                origin,
                extractor
            );

            return true;
        }

        for (int x = 0; x < gridObject.Width; x++)
        {
            for (int y = 0; y < gridObject.Height; y++)
            {
                GridPosition cellPosition =
                    GetFootprintCell(
                        origin,
                        gridObject,
                        x,
                        y
                    );

                gridObjects.Add(
                    cellPosition,
                    gridObject
                );
            }
        }

        gridObject.SetGridPosition(origin);

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
        GridPosition origin)
    {
        GridObject gridObject =
            GetGridObject(origin);

        if (gridObject == null)
            return;

        for (int x = 0; x < gridObject.Width; x++)
        {
            for (int y = 0; y < gridObject.Height; y++)
            {
                GridPosition cellPosition =
                    GetFootprintCell(
                        origin,
                        gridObject,
                        x,
                        y
                    );

                gridObjects.Remove(cellPosition);
            }
        }
    }
    public bool CanPlaceExtractorOnSpawner(
        GridPosition origin,
        GridObject extractor)
    {
        if (extractor == null)
            return false;

        if (extractor.GetComponent<Extractor>() == null)
            return false;

        if (extractor.Width != 3 || extractor.Height != 3)
            return false;

        GridObject centerObject =
            GetGridObject(origin);

        if (centerObject == null)
            return false;

        FoodSpawner spawner =
            centerObject.GetComponent<FoodSpawner>();

        if (spawner == null)
            return false;

        if (spawner.Width != 3 || spawner.Height != 3)
            return false;

        if (spawner.GridPosition.x != origin.x ||
            spawner.GridPosition.y != origin.y)
            return false;

        Extractor[] existingExtractors =
            FindObjectsByType<Extractor>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Extractor existingExtractor in existingExtractors)
        {
            if (existingExtractor.GridPosition.x == origin.x &&
                existingExtractor.GridPosition.y == origin.y)
            {
                return false;
            }
        }

        return true;
    }
}