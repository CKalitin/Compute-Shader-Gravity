using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravityObjectCanvasDisplay : MonoBehaviour {
    public static GravityObjectCanvasDisplay instance;

    private struct GravityObjectHighlightStruct {
        public GameObject gravityObject;
        public GameObject gravityObjectHighlight;

        public GravityObjectHighlightStruct(GameObject _gravityObject, GameObject _gravityObjectHighlight) {
            gravityObject = _gravityObject;
            gravityObjectHighlight = _gravityObjectHighlight;
        }
    }

    [SerializeField] private GameObject gravityObjectHighlight;

    private Dictionary<int, GravityObjectHighlightStruct> gravityObjectHighlights = new Dictionary<int, GravityObjectHighlightStruct>();

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update() {
        foreach (KeyValuePair<int, GravityObjectHighlightStruct> highlightStruct in gravityObjectHighlights) {
            if (highlightStruct.Value.gravityObject == null) {
                Destroy(highlightStruct.Value.gravityObjectHighlight);
                gravityObjectHighlights.Remove(highlightStruct.Key);
                break;
            }
            highlightStruct.Value.gravityObjectHighlight.transform.position = CameraController.instance.Cam.WorldToScreenPoint(highlightStruct.Value.gravityObject.transform.position);
        }
    }

    public void HighlightGravityObject(GravityObject _go) {
        GameObject highlight = Instantiate(gravityObjectHighlight, CameraController.instance.Cam.WorldToScreenPoint(_go.transform.position), Quaternion.identity, transform);
        gravityObjectHighlights.Add(_go.gameObject.GetInstanceID(), new GravityObjectHighlightStruct(_go.gameObject, highlight));
    }

    public void UnhighlightGravityObject(GravityObject _go) {
        if (_go == null) return;

        Destroy(gravityObjectHighlights[_go.gameObject.GetInstanceID()].gravityObjectHighlight);
        gravityObjectHighlights.Remove(_go.gameObject.GetInstanceID());
    }
}
