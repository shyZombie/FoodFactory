using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject selectedBuildingPrefab;
    
    private GameObject previewBuilding;
    private GridPosition previewGridPosition;

    private int rotationSteps = 0;
    private Dictionary<SpriteRenderer, Color> previewOriginalColors =
    new Dictionary<SpriteRenderer, Color>();

    public void SelectBuilding(GameObject buildingPrefab)
    {
        selectedBuildingPrefab = buildingPrefab;
        rotationSteps = 0;

        CreatePreview();

        Debug.Log(
            $"Selected Building: {selectedBuildingPrefab.name}"
        );
    }

    private void CreatePreview()
    {
        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
            previewBuilding = null;
        }

        if (selectedBuildingPrefab == null)
            return;

        previewBuilding = Instantiate(
            selectedBuildingPrefab
        );

        previewBuilding.name =
            selectedBuildingPrefab.name + "_Preview";

        previewOriginalColors.Clear();

        SpriteRenderer[] spriteRenderers =
            previewBuilding.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            previewOriginalColors[spriteRenderer] =
                spriteRenderer.color;
        }

        DisablePreviewComponents();
        SetPreviewTransparency();
        SetPreviewSortingOrder();
    }

    private void SetPreviewSortingOrder()
    {
        if (previewBuilding == null)
            return;

        SpriteRenderer[] spriteRenderers =
            previewBuilding.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingOrder = 100;
        }
    }

    private void DisablePreviewComponents()
    {
        MonoBehaviour[] behaviours =
            previewBuilding.GetComponentsInChildren<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            behaviour.enabled = false;
        }

        Collider2D[] colliders =
            previewBuilding.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void SetPreviewTransparency()
    {
        SpriteRenderer[] spriteRenderers =
            previewBuilding.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;
        }
    }

    private void Update()
    {
        HandleCancel();
        HandleRotation();
        HandlePreview();
        HandlePlacement();
    }

    private void HandleCancel()
    {
        if (!Keyboard.current.escapeKey.wasPressedThisFrame &&
            !Mouse.current.rightButton.wasPressedThisFrame)
            return;

        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
            previewBuilding = null;
        }

        selectedBuildingPrefab = null;
        rotationSteps = 0;

        Debug.Log("Building placement cancelled.");
    }

    private void HandlePreview()
    {
        if (previewBuilding == null)
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            previewBuilding.SetActive(false);
            return;
        }

        previewBuilding.SetActive(true);

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    -Camera.main.transform.position.z
                )
            );

        GridPosition gridPosition =
            gridManager.WorldToGridPosition(
                mouseWorldPosition
            );

        previewGridPosition = gridPosition;

        Vector3 worldPosition =
            gridManager.GridToWorldPosition(
                gridPosition
            );

        previewBuilding.transform.position =
            worldPosition;

        UpdatePreviewValidity(gridPosition);
    }

    private void UpdatePreviewRotation()
    {
        if (previewBuilding == null)
            return;

        previewBuilding.transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                -rotationSteps * 90f
            );
    }

    private bool CanPlacePreview()
    {
        if (previewBuilding == null)
            return false;

        GridObject gridObject =
            previewBuilding.GetComponent<GridObject>();

        if (gridObject == null)
            return false;

        return gridManager.CanPlaceGridObject(
            previewGridPosition,
            gridObject
        );
    }

    private void UpdatePreviewValidity(
        GridPosition gridPosition)
    {
        if (previewBuilding == null)
            return;

        GridObject gridObject =
            previewBuilding.GetComponent<GridObject>();

        if (gridObject == null)
            return;

        bool canPlace =
            gridManager.CanPlaceGridObject(
                gridPosition,
                gridObject
            );

        SpriteRenderer[] spriteRenderers =
            previewBuilding.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Color color;

            if (previewOriginalColors.ContainsKey(
                    spriteRenderer))
            {
                color =
                    previewOriginalColors[spriteRenderer];
            }
            else
            {
                color = Color.white;
            }

            if (!canPlace)
            {
                color.r = 1f;
                color.g = 0.3f;
                color.b = 0.3f;
            }

            color.a = 0.5f;

            spriteRenderer.color = color;
        }
    }

    private void HandleRotation()
    {
        if (!Keyboard.current.rKey.wasPressedThisFrame)
            return;

        GridObject gridObject = GetGridObjectUnderMouse();

        if (gridObject != null)
        {
            ConveyorBelt conveyorBelt =
                gridObject.GetComponent<ConveyorBelt>();

            if (conveyorBelt != null)
            {
                conveyorBelt.RotateClockwise();

                switch (conveyorBelt.GetDirection())
                {
                    case ConveyorBelt.Direction.Right:
                        rotationSteps = 0;
                        break;

                    case ConveyorBelt.Direction.Down:
                        rotationSteps = 1;
                        break;

                    case ConveyorBelt.Direction.Left:
                        rotationSteps = 2;
                        break;

                    case ConveyorBelt.Direction.Up:
                        rotationSteps = 3;
                        break;
                }

                UpdatePreviewRotation();
                return;
            }

            Machine machine =
                gridObject.GetComponent<Machine>();

            if (machine != null)
            {
                machine.RotateClockwise();

                switch (machine.GetDirection())
                {
                    case Machine.Direction.Right:
                        rotationSteps = 0;
                        break;

                    case Machine.Direction.Down:
                        rotationSteps = 1;
                        break;

                    case Machine.Direction.Left:
                        rotationSteps = 2;
                        break;

                    case Machine.Direction.Up:
                        rotationSteps = 3;
                        break;
                }

                UpdatePreviewRotation();
                return;
            }
        }

        rotationSteps++;

        if (rotationSteps >= 4)
        {
            rotationSteps = 0;
        }

        UpdatePreviewRotation();

        Debug.Log(
            $"Next Building Rotation: " +
            $"{rotationSteps * 90} degrees"
        );
    }

    private GridObject GetGridObjectUnderMouse()
    {
        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    -Camera.main.transform.position.z
                )
            );

        GridPosition gridPosition =
            gridManager.WorldToGridPosition(
                mouseWorldPosition
            );

        return gridManager.GetGridObject(
            gridPosition
        );
    }

    private void HandlePlacement()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        PlaceBuilding();
    }

    private void PlaceBuilding()
    {
        if (selectedBuildingPrefab == null)
            return;

        GridObject previewGridObject =
            selectedBuildingPrefab.GetComponent<GridObject>();

        if (previewGridObject == null)
        {
            Debug.LogError(
                $"Selected building {selectedBuildingPrefab.name} " +
                $"does not have a GridObject component."
            );
            return;
        }

        if (!gridManager.CanPlaceGridObject(
                previewGridPosition,
                previewGridObject))
        {
            Debug.Log(
                $"Cannot place {selectedBuildingPrefab.name} " +
                $"at {previewGridPosition}."
            );
            return;
        }

        Vector3 worldPosition =
            gridManager.GridToWorldPosition(
                previewGridPosition
            );

        GameObject newBuilding =
            Instantiate(
                selectedBuildingPrefab,
                worldPosition,
                Quaternion.identity
            );

        GridObject gridObject =
            newBuilding.GetComponent<GridObject>();

        if (gridObject == null)
        {
            Debug.LogError(
                $"Placed object {newBuilding.name} " +
                $"does not have a GridObject component."
            );

            Destroy(newBuilding);
            return;
        }

        if (!gridManager.TryAddGridObject(
            previewGridPosition,
            gridObject))
        {
            Debug.LogWarning(
                $"Failed to register {newBuilding.name} " +
                $"at {previewGridPosition}."
            );

            Destroy(newBuilding);
            return;
        }

        Extractor extractor =
            newBuilding.GetComponent<Extractor>();

        if (extractor != null)
        {
            GridObject gridObjectUnderExtractor =
                gridManager.GetGridObject(
                    previewGridPosition
                );

            if (gridObjectUnderExtractor != null)
            {
                FoodSpawner foodSpawner =
                    gridObjectUnderExtractor
                        .GetComponent<FoodSpawner>();

                if (foodSpawner != null)
                {
                    foodSpawner.SetExtractor(extractor);

                    Debug.Log(
                        $"{foodSpawner.name} linked to " +
                        $"Extractor {extractor.name}."
                    );
                }
            }
        }

        ConveyorBelt conveyorBelt =
            newBuilding.GetComponent<ConveyorBelt>();

        if (conveyorBelt != null)
        {
            for (int i = 0; i < rotationSteps; i++)
            {
                conveyorBelt.RotateClockwise();
            }
        }

        Machine machine =
            newBuilding.GetComponent<Machine>();

        if (machine != null)
        {
            for (int i = 0; i < rotationSteps; i++)
            {
                machine.RotateClockwise();
            }
        }

        Debug.Log(
            $"Placed {selectedBuildingPrefab.name} " +
            $"at {previewGridPosition}."
        );
    }
}