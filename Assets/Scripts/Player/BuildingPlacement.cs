using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject selectedBuildingPrefab;
    
    private GameObject previewBuilding;
    private GridPosition previewGridPosition;
    private GridObject selectedGridObject;
    private bool isMovingSelectedObject = false;
    private Vector3 selectedOriginalPosition;

    private Button selectedBuildingButton;
    private bool isDraggingConveyor = false;
    private ConveyorBelt lastDragConveyor;
    private GridPosition lastDragGridPosition;


    private int rotationSteps = 0;
    private Dictionary<SpriteRenderer, Color> previewOriginalColors =
    new Dictionary<SpriteRenderer, Color>();
    private Dictionary<SpriteRenderer, Color> selectedOriginalColors =
    new Dictionary<SpriteRenderer, Color>();
    [SerializeField] private Button[] buildingButtons;
    [SerializeField] private Color selectedButtonColor = new Color(1f, 0.85f, 0.2f);

    public void SelectBuilding(GameObject buildingPrefab)
    {
        selectedBuildingPrefab = buildingPrefab;
        rotationSteps = 0;

        UpdateSelectedBuildingButton();

        CreatePreview();

        Debug.Log(
            $"Selected Building: {selectedBuildingPrefab.name}"
        );
    }
    private void UpdateSelectedBuildingButton()
    {
        if (EventSystem.current == null)
            return;

        GameObject selectedObject =
            EventSystem.current.currentSelectedGameObject;

        foreach (Button button in buildingButtons)
        {
            if (button == null)
                continue;

            Image image = button.GetComponent<Image>();

            if (image == null)
                continue;

            if (button.gameObject == selectedObject)
            {
                image.color = selectedButtonColor;
            }
            else
            {
                image.color = Color.white;
            }
        }
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
        HandleConveyorDrag();
        HandlePlacement();
        HandleSelection();
        HandleMove();
        HandleMovePreview();
        HandleDelete();
    }
    private bool IsSelectedConveyor()
    {
        if (selectedBuildingPrefab == null)
            return false;

        return selectedBuildingPrefab.GetComponent<ConveyorBelt>() != null;
    }

    private void UpdateConveyorDragRotation(
        GridPosition currentGridPosition)
    {
        int deltaX =
            currentGridPosition.x - lastDragGridPosition.x;

        int deltaY =
            currentGridPosition.y - lastDragGridPosition.y;

        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            if (deltaX > 0)
            {
                rotationSteps = 0; // Right
            }
            else
            {
                rotationSteps = 2; // Left
            }
        }
        else
        {
            if (deltaY > 0)
            {
                rotationSteps = 3; // Up
            }
            else
            {
                rotationSteps = 1; // Down
            }
        }

        UpdatePreviewRotation();
    }
    private void SetConveyorRotation(
        ConveyorBelt conveyor,
        int targetRotationSteps)
    {
        if (conveyor == null)
            return;

        int currentRotationSteps = 0;

        switch (conveyor.GetDirection())
        {
            case ConveyorBelt.Direction.Right:
                currentRotationSteps = 0;
                break;

            case ConveyorBelt.Direction.Down:
                currentRotationSteps = 1;
                break;

            case ConveyorBelt.Direction.Left:
                currentRotationSteps = 2;
                break;

            case ConveyorBelt.Direction.Up:
                currentRotationSteps = 3;
                break;
        }

        int rotationDifference =
            (targetRotationSteps - currentRotationSteps + 4) % 4;

        for (int i = 0; i < rotationDifference; i++)
        {
            conveyor.RotateClockwise();
        }
    }
    private void HandleConveyorDrag()
    {
        if (!IsSelectedConveyor())
        {
            isDraggingConveyor = false;
            lastDragConveyor = null;
            return;
        }

        if (isMovingSelectedObject)
        {
            isDraggingConveyor = false;
            lastDragConveyor = null;
            return;
        }

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDraggingConveyor = true;

            bool placed = PlaceBuilding();

            if (placed)
            {
                lastDragGridPosition = previewGridPosition;

                lastDragConveyor =
                    GetGridObjectUnderMouse()
                        ?.GetComponent<ConveyorBelt>();
            }
        }

        if (!Mouse.current.leftButton.isPressed)
        {
            isDraggingConveyor = false;
            lastDragConveyor = null;
            return;
        }

        if (!isDraggingConveyor)
            return;

        if (previewGridPosition.x == lastDragGridPosition.x &&
            previewGridPosition.y == lastDragGridPosition.y)
        {
            return;
        }

        UpdateConveyorDragRotation(
            previewGridPosition
        );

        ConveyorBelt previousConveyor =
            lastDragConveyor;

        bool placedNext = PlaceBuilding();

        if (placedNext)
        {
            SetConveyorRotation(
                previousConveyor,
                rotationSteps
            );

            lastDragGridPosition = previewGridPosition;

            lastDragConveyor =
                GetGridObjectUnderMouse()
                    ?.GetComponent<ConveyorBelt>();
        }
        else
        {
            isDraggingConveyor = false;
            lastDragConveyor = null;
        }
    }
    private void HandleSelection()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (isMovingSelectedObject)
            return;

        GridObject gridObject =
            GetGridObjectUnderMouse();

        if (gridObject == null)
        {
            ClearSelection();
            return;
        }

        FoodSpawner spawner =
            gridObject.GetComponent<FoodSpawner>();

        if (spawner != null)
        {
            ClearSelection();
            return;
        }

        ApplySelectionVisual(gridObject);

        Debug.Log(
            $"Selected GridObject: {gridObject.name}"
        );
    }

    private void HandleMove()
    {
        if (!Keyboard.current.mKey.wasPressedThisFrame)
            return;

        if (selectedGridObject == null)
            return;

        if (isMovingSelectedObject)
            return;

        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
            previewBuilding = null;
        }

        selectedBuildingPrefab = null;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        UpdateSelectedBuildingButton();

        rotationSteps = 0;

        selectedOriginalPosition = selectedGridObject.transform.position;
        isMovingSelectedObject = true;

        Debug.Log(
            $"Started moving {selectedGridObject.name} " +
            $"from {selectedGridObject.GridPosition}."
        );
    }

    private void HandleMovePreview()
    {
        if (!isMovingSelectedObject ||
            selectedGridObject == null)
        {
            return;
        }

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                -Camera.main.transform.position.z
            )
        );

        previewGridPosition = gridManager.WorldToGridPosition(mouseWorldPosition);

        Vector3 previewWorldPosition =
            gridManager.GridToWorldPosition(previewGridPosition);

        selectedGridObject.transform.position = previewWorldPosition;

        bool canMove = CanMoveSelectedObject(previewGridPosition);

        foreach (SpriteRenderer spriteRenderer in
                 selectedGridObject.GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = spriteRenderer.color;

            if (canMove)
            {
                color.r = 1f;
                color.g = 1f;
                color.b = 1f;
            }
            else
            {
                color.r = 1f;
                color.g = 0.25f;
                color.b = 0.25f;
            }

            spriteRenderer.color = color;
        }
    }
    private void ConfirmMove()
    {
        Debug.Log(
            $"MOVE DEBUG | " +
            $"Selected: {selectedGridObject.name} | " +
            $"Selected GridPosition: {selectedGridObject.GridPosition} | " +
            $"Preview: {previewGridPosition} | " +
            $"GridManager at Preview: " +
            $"{gridManager.GetGridObject(previewGridPosition)?.name}"
        );

        if (selectedGridObject == null)
            return;

        GridPosition targetGridPosition = previewGridPosition;

        if (!CanMoveSelectedObject(targetGridPosition))
        {
            Debug.Log(
                $"Cannot move {selectedGridObject.name} " +
                $"to {targetGridPosition}."
            );

            return;
        }

        GridPosition originalGridPosition = selectedGridObject.GridPosition;

        Extractor extractor =
            selectedGridObject.GetComponent<Extractor>();

        FoodSpawner oldSpawner = null;
        FoodSpawner newSpawner = null;

        if (extractor != null)
        {
            GridObject oldGridObject =
                gridManager.GetGridObject(originalGridPosition);

            if (oldGridObject != null)
            {
                oldSpawner =
                    oldGridObject.GetComponent<FoodSpawner>();
            }

            GridObject newGridObject =
                gridManager.GetGridObject(targetGridPosition);

            if (newGridObject != null)
            {
                newSpawner =
                    newGridObject.GetComponent<FoodSpawner>();
            }
        }

        bool moved = gridManager.MoveGridObject(
            originalGridPosition,
            targetGridPosition,
            selectedGridObject
        );

        if (!moved)
        {
            Debug.Log(
                $"Failed to move {selectedGridObject.name} " +
                $"to {targetGridPosition}."
            );

            return;
        }

        if (extractor != null)
        {
            if (oldSpawner != null)
            {
                oldSpawner.SetExtractor(null);
            }

            if (newSpawner != null)
            {
                newSpawner.SetExtractor(extractor);
            }
        }

        selectedGridObject.transform.position =
            gridManager.GridToWorldPosition(targetGridPosition);

        isMovingSelectedObject = false;

        Debug.Log(
            $"Moved {selectedGridObject.name} " +
            $"from {originalGridPosition} " +
            $"to {targetGridPosition}."
        );
    }
    private bool CanMoveSelectedObject(GridPosition targetGridPosition)
    {
        if (selectedGridObject == null)
            return false;

        GridPosition currentGridPosition = selectedGridObject.GridPosition;

        if (targetGridPosition.x == currentGridPosition.x &&
            targetGridPosition.y == currentGridPosition.y)
        {
            return true;
        }

        Extractor extractor =
            selectedGridObject.GetComponent<Extractor>();

        if (extractor != null)
        {
            return gridManager.CanPlaceExtractorOnSpawner(
                targetGridPosition,
                selectedGridObject
            );
        }

        return gridManager.CanPlaceGridObject(
            targetGridPosition,
            selectedGridObject
        );
    }

    private void ClearSelection()
    {
        foreach (
            KeyValuePair<SpriteRenderer, Color> pair
            in selectedOriginalColors)
        {
            if (pair.Key != null)
                pair.Key.color = pair.Value;
        }

        selectedOriginalColors.Clear();
        selectedGridObject = null;
    }

    private void ApplySelectionVisual(
        GridObject gridObject)
    {
        ClearSelection();

        selectedGridObject = gridObject;

        SpriteRenderer[] spriteRenderers =
            gridObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            selectedOriginalColors[spriteRenderer] =
                spriteRenderer.color;

            Color selectedColor =
                spriteRenderer.color;

            selectedColor.r = 1f;
            selectedColor.g = 0.85f;
            selectedColor.b = 0.2f;
            selectedColor.a = 0.5f;

            spriteRenderer.color = selectedColor;
        }
    }

    private void HandleCancel()
    {
        if ((Mouse.current.rightButton.wasPressedThisFrame ||
             Keyboard.current.escapeKey.wasPressedThisFrame) &&
            isMovingSelectedObject)
        {
            selectedGridObject.transform.position = selectedOriginalPosition;
            isMovingSelectedObject = false;

            Debug.Log(
                $"Cancelled moving {selectedGridObject.name}. " +
                $"Returned to original position."
            );

            return;
        }

        if (!Keyboard.current.escapeKey.wasPressedThisFrame &&
            !Mouse.current.rightButton.wasPressedThisFrame)
            return;

        if (previewBuilding != null)
        {
            Destroy(previewBuilding);
            previewBuilding = null;
        }

        selectedBuildingPrefab = null;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        UpdateSelectedBuildingButton();

        rotationSteps = 0;

        ClearSelection();

        Debug.Log("Building placement cancelled.");
    }

    private void HandleDelete()
    {
        if (!Keyboard.current.deleteKey.wasPressedThisFrame)
            return;

        if (selectedGridObject == null)
            return;

        GridPosition gridPosition =
            selectedGridObject.GridPosition;

        gridManager.RemoveGridObject(gridPosition);

        Destroy(selectedGridObject.gameObject);

        isMovingSelectedObject = false;

        selectedOriginalColors.Clear();
        selectedGridObject = null;

        Debug.Log(
            $"Deleted GridObject at {gridPosition}."
        );
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

        Extractor extractor =
            previewBuilding.GetComponent<Extractor>();

        if (extractor != null)
        {
            canPlace =
                gridManager.CanPlaceExtractorOnSpawner(
                    gridPosition,
                    gridObject
                );
        }

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

        Extractor extractor =
            gridManager.GetExtractor(
                gridPosition
            );

        if (extractor != null)
            return extractor;

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

        if (IsSelectedConveyor() &&
            Mouse.current.leftButton.isPressed)
        {
            return;
        }

        if (isMovingSelectedObject)
        {
            ConfirmMove();
            return;
        }

        if (selectedBuildingPrefab == null)
            return;

        PlaceBuilding();
    }

    private bool PlaceBuilding()
    {
        if (selectedBuildingPrefab == null)
            return false;

        if (previewBuilding == null)
            return false;

        GridObject previewGridObject =
            selectedBuildingPrefab.GetComponent<GridObject>();

        if (previewGridObject == null)
        {
            Debug.LogError(
                $"Selected building {selectedBuildingPrefab.name} " +
                $"does not have a GridObject component."
            );
            return false;
        }

        Extractor extractorPreview =
            previewBuilding.GetComponent<Extractor>();

        bool canPlace;

        if (extractorPreview != null)
        {
            canPlace =
                gridManager.CanPlaceExtractorOnSpawner(
                    previewGridPosition,
                    previewGridObject
                );
        }
        else
        {
            canPlace =
                gridManager.CanPlaceGridObject(
                    previewGridPosition,
                    previewGridObject
                );
        }

        if (!canPlace)
        {
            Debug.Log(
                $"Cannot place {selectedBuildingPrefab.name} " +
                $"at {previewGridPosition}."
            );
            return false;
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
            return false;
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
            return false;
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

        return true;
    }
}