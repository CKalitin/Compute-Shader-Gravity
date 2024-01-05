using System.Collections;
using System.Collections.Generic;
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

    private void Start() {
        selectionBox.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            selectionBoxStart = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetKey(KeyCode.Mouse0)) {
            selectionBoxEnd = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(selectionBoxStart, selectionBoxEnd) > 0.1f) DrawSelectionBox();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            selectionBox.SetActive(false);
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
}
