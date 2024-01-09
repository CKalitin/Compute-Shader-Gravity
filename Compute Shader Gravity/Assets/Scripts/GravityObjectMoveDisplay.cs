 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObjectMoveDisplay : MonoBehaviour {
    [SerializeField] private GameObject movingObjectsDisplay;
    [SerializeField] private GameObject pushingObjectsDisplayLine;
    [Space]
    [SerializeField] private Vector2 movingObjectsDisplayOffset;
    [SerializeField] private float pushingObjectsHeight;

    private void Update() {
        if (GravityObjectPlayerControl.instance.MovingObjects) {
            movingObjectsDisplay.SetActive(true);
            movingObjectsDisplay.transform.position = (Vector2)Input.mousePosition + movingObjectsDisplayOffset;
        } else if (GravityObjectPlayerControl.instance.PushingObjects) {
            pushingObjectsDisplayLine.SetActive(true);

            Vector2 start = CameraController.instance.Cam.WorldToScreenPoint(GravityObjectPlayerControl.instance.PushObjectsStart);
            Vector2 end = (Vector2)Input.mousePosition;
            Vector2 pos = start + ((end - start) / 2);
            pushingObjectsDisplayLine.transform.position = pos;

            Vector3 dir = (end-pos).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            pushingObjectsDisplayLine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            float scale = Vector2.Distance(start, end) / pushingObjectsHeight;
            pushingObjectsDisplayLine.transform.localScale = new Vector3(scale, scale, 1);

        } else {
            movingObjectsDisplay.SetActive(false);
            pushingObjectsDisplayLine.SetActive(false);
        }
    }
}
