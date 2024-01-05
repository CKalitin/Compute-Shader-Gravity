using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static CameraController instance;

    [Header("Move & Zoom")]
    [SerializeField] private float panSpeed;
    [Space]
    [SerializeField] private float zoomStepSize;
    [SerializeField] private float zoomLerpSpeed;
    [Space]
    [Tooltip("Multiply cam size by this variable and add to pan speed.")]
    [SerializeField] private float panSpeedMultiplier;
    [Tooltip("Multiply cam size by this variable and add to zoom step size.")]
    [SerializeField] private float zoomStepSizeMultiplier;

    private float camSize { get => cam.orthographicSize; set => cam.orthographicSize = value; }
    private float targetCamSize;

    [Header("Camera")]
    [SerializeField] private float maxCamSize;
    [SerializeField] private float minCamSize;
    [Space]
    [SerializeField] private Camera cam;
    [Space]
    [SerializeField] private Transform followTransform;

    public Camera Cam { get => cam; set => cam = value; }
    public Transform FollowTransform { get => followTransform; set => followTransform = value; }

    private void Awake() {
        if (instance == null) { instance = this; }
        else { Destroy(this); Debug.LogError("Multiple CameraControllers in scene!"); }
    }

    private void Start() {
        targetCamSize = camSize;
    }

    private void Update() {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera() {
        Vector2 pos = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (pos != Vector2.zero) { followTransform = null; }
        if (followTransform != null) {
            transform.position = followTransform.position;
            return;
        }

        transform.Translate(pos * panSpeed * Time.unscaledDeltaTime + (pos * (camSize - minCamSize) * panSpeedMultiplier));
    }

    private void ZoomCamera() {
        targetCamSize -= Input.GetAxisRaw("Mouse ScrollWheel") * 10 * zoomStepSize + (Input.GetAxisRaw("Mouse ScrollWheel") * (camSize - minCamSize) * zoomStepSizeMultiplier); // Input.GetAxisRaw("Mouse ScrollWheel") returns +/- 0.1
        targetCamSize = Mathf.Clamp(targetCamSize, minCamSize, maxCamSize);

        camSize = Mathf.Lerp(camSize, targetCamSize, zoomLerpSpeed * Time.unscaledDeltaTime);
    }
}
