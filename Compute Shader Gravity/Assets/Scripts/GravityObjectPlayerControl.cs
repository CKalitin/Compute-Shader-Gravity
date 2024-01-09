using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GravityObjectPlayerControl : MonoBehaviour {
    public static GravityObjectPlayerControl instance;

    [SerializeField] private GameObject selectionBox;
    [Space]
    [SerializeField] private float gravityObjectPushMultiplier = 10f;

    private bool movingObjects = false;
    private bool pushingObjects = false;

    private Vector2 selectionBoxStart;
    private Vector2 selectionBoxEnd;

    private Vector2 pushObjectsStart = Vector2.zero;
    private Vector2 moveObjectsStart = Vector2.zero;

    private List<GravityObject> selectedGravityObjects = new List<GravityObject>();

    public bool MovingObjects { get => movingObjects; }
    public bool PushingObjects { get => pushingObjects; }

    public Vector2 PushObjectsStart { get => pushObjectsStart; }
    public Vector2 MoveObjectsStart { get => moveObjectsStart; }

    private void Awake() {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
    }

    private void Start() {
        selectionBox.SetActive(false);
    }

    private void Update() {
        movingObjects = false;

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            selectionBoxStart = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);

            if (selectedGravityObjects.Count > 0) moveObjectsStart = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetKey(KeyCode.Mouse0) && moveObjectsStart == Vector2.zero) {
            selectionBoxEnd = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);

            // If dist from start to end is greater than 3 pixels
            if (Vector2.Distance(CameraController.instance.Cam.WorldToScreenPoint(selectionBoxStart), Input.mousePosition) > 3f) {
                DrawSelectionBox();
                GetGravityObjectsInSelectionBox();
            }
            // If dist is low and key is up, then focus on object
            else if (Input.GetKeyUp(KeyCode.Mouse0)) {
                RaycastHit2D hit = Physics2D.Raycast(CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.GetComponent<GravityObject>()) CameraController.instance.FollowTransform = hit.collider.transform;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) && moveObjectsStart != Vector2.zero) {
            // If more than 3px, move it, if not then clear selection
            if (Vector2.Distance(CameraController.instance.Cam.WorldToScreenPoint(selectionBoxStart), Input.mousePosition) > 3f) {
                Vector2 moveObjectsEnd = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);
                MoveGravityObjects(moveObjectsEnd - moveObjectsStart);
                moveObjectsStart = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);
                movingObjects = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            selectionBox.SetActive(false);

            if (Vector2.Distance(CameraController.instance.Cam.WorldToScreenPoint(selectionBoxStart), Input.mousePosition) < 3f) {
                HighlightGravityObjects(new List<GravityObject>(), selectedGravityObjects);
                selectedGravityObjects = new List<GravityObject>();
            }

            moveObjectsStart = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            moveObjectsStart = Vector2.zero;
            pushObjectsStart = CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition);
            pushingObjects = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1)) {
            // If left click held down and moved more than 3px, apply velocity to objects
            if (Vector2.Distance(CameraController.instance.Cam.WorldToScreenPoint(pushObjectsStart), Input.mousePosition) > 3f) {
                Vector2 vel = ((Vector2)CameraController.instance.Cam.ScreenToWorldPoint(Input.mousePosition) - pushObjectsStart) * gravityObjectPushMultiplier;
                ApplyVelocityToGravityObjects(vel);
            }
            else {
                HighlightGravityObjects(new List<GravityObject>(), selectedGravityObjects);
                selectedGravityObjects = new List<GravityObject>();
            }
            moveObjectsStart = Vector2.zero;
            pushingObjects = false;
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

    public void ZeroVelocity() {
        for (int i = 0; i < selectedGravityObjects.Count; i++) {
            if (!CheckGravityObjectNull(selectedGravityObjects[i])) selectedGravityObjects[i].SetVelocity(Vector3.zero);
        }
    }

    public void DeleteGravityObjects() {
        for (int i = 0; i < selectedGravityObjects.Count; i++) {
            Destroy(selectedGravityObjects[i].gameObject);
        }
        selectedGravityObjects = new List<GravityObject>();
    }

    public void SpawnGravityObject(GameObject _go, Vector2 _pos, Vector2 _vel) {
        GameObject go = Instantiate(_go, _pos, Quaternion.identity);

        if (go.GetComponent<Rigidbody2D>()) go.GetComponent<Rigidbody2D>().velocity = _vel;
        else if (go.GetComponent<Rigidbody>()) go.GetComponent<Rigidbody>().velocity = _vel;
    }

    private void ApplyVelocityToGravityObjects(Vector2 _vel) {
        for (int i = 0; i < selectedGravityObjects.Count; i++) {
            if (!CheckGravityObjectNull(selectedGravityObjects[i])) selectedGravityObjects[i].AddForce(_vel);
        }
    }

    private void MoveGravityObjects(Vector3 _pos) {
        for (int i = 0; i < selectedGravityObjects.Count; i++) {
            if (!CheckGravityObjectNull(selectedGravityObjects[i])) selectedGravityObjects[i].transform.position += _pos;
        }
    }

    private bool CheckGravityObjectNull(GravityObject _go) {
        if (_go == null) {
            selectedGravityObjects.Remove(_go);
            GravityObjectCanvasDisplay.instance.UnhighlightGravityObject(_go);
            return true;
        }
        return false;
    }
}
