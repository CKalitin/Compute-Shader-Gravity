using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GravityObjectPlayerControl : MonoBehaviour {
    // Spawn GravityObject with inital velocity
    // Delete GravityObject
    // Focus Camera on GravityObject
    // Move GravityObject
    // Zero out velocity of GravityObject
    // Select multiple GravityObjects

    [SerializeField] private GameObject selectionBox;

    private Vector2 selectionBoxStart;
    private Vector2 selectionBoxEnd;

    private List<GravityObject> selectedGravityObjects = new List<GravityObject>();

    private void Start() {
        selectionBox.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            selectionBoxStart = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);

            HighlightGravityObjects(new List<GravityObject>(), selectedGravityObjects);
            selectedGravityObjects = new List<GravityObject>();
        }

        if (Input.GetKey(KeyCode.Mouse0)) {
            selectionBoxEnd = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(selectionBoxStart, selectionBoxEnd) > 0.1f) {
                DrawSelectionBox();
                GetGravityObjectsInSelectionBox();
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            selectionBox.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1)) {
            HighlightGravityObjects(new List<GravityObject>(), selectedGravityObjects);
            selectedGravityObjects = new List<GravityObject>();
        }
    }

    private void DrawSelectionBox() {
        Vector2 center = (selectionBoxStart + selectionBoxEnd) / 2;
        Vector2 size = new Vector2(Mathf.Abs(selectionBoxStart.x - selectionBoxEnd.x), Mathf.Abs(selectionBoxStart.y - selectionBoxEnd.y));

        if (size.x * size.y == 0) return;

        selectionBox.SetActive(true);

        selectionBox.transform.position = center;
        selectionBox.transform.localScale = size;
        selectionBox.transform.GetChild(0).localScale = Vector2.one / size;
        selectionBox.transform.GetChild(0).GetComponent<SpriteRenderer>().size = size;

    }

    private void GetGravityObjectsInSelectionBox() {
        List<Collider2D> colliders = Physics2D.OverlapBoxAll(selectionBox.transform.position, selectionBox.transform.localScale, 0).ToList();
        List<GravityObject> newGravityObjects = new List<GravityObject>();
        List<GravityObject> unselectedGravityObjects = new List<GravityObject>();

        for (int i = 0; i < selectedGravityObjects.Count; i++) {
            if (colliders.Contains(selectedGravityObjects[i].GetComponent<Collider2D>())) continue;

            unselectedGravityObjects.Add(selectedGravityObjects[i]);
            selectedGravityObjects.Remove(selectedGravityObjects[i]);
        }

        for (int i = 0; i < colliders.Count; i++) {
            if (colliders[i].GetComponent<GravityObject>() == null) continue;
            if (selectedGravityObjects.Contains(colliders[i].GetComponent<GravityObject>())) continue;

            selectedGravityObjects.Add(colliders[i].GetComponent<GravityObject>());
            newGravityObjects.Add(colliders[i].GetComponent<GravityObject>());
        }

        HighlightGravityObjects(newGravityObjects, unselectedGravityObjects);
    }

    private void HighlightGravityObjects(List<GravityObject> news, List<GravityObject> unselected) {
        for (int i = 0; i < news.Count; i++) {
            GravityObjectCanvasDisplay.instance.HighlightGravityObject(news[i]);
        }

        for (int i = 0; i < unselected.Count; i++) {
            GravityObjectCanvasDisplay.instance.UnhighlightGravityObject(unselected[i]);
        }
    }
}
