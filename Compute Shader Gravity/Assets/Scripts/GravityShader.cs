using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.EditorTools;
using UnityEngine;


public class GravityShader : MonoBehaviour {
    public static GravityShader instance;

    private struct GravityObjectStruct {
        public Vector3 position;
        public float mass;
    }

    private struct GravityVector {
        public Vector3 acceleration;
    }

    [SerializeField] private ComputeShader gravityShader;
    [SerializeField] private float graviticConstant = 5f;
    [SerializeField] private float timeScale = 1f;

    private List<GravityObject> gravityObjects = new List<GravityObject>();
    private List<GravityObjectStruct> gravityObjectsStructs = new List<GravityObjectStruct>();

    private void Awake() {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
    }

    public void AddGravityObject(GravityObject _go) {
        gravityObjects.Add(_go);
        gravityObjectsStructs.Add(new GravityObjectStruct() { position = _go.transform.position, mass = _go.Mass });
    }

    public void RemoveGravityObject(GravityObject _t) {
        int index = gravityObjects.IndexOf(_t);
        gravityObjects.RemoveAt(index);
        gravityObjectsStructs.RemoveAt(index);
    }

    private void FixedUpdate() {
        UpdateGravity();
        Time.timeScale = timeScale;
    }

    private void UpdateGravity() {
        if (gravityObjects.Count <= 1) { return; }

        for (int i = 0; i < gravityObjectsStructs.Count; i++) {
            gravityObjectsStructs[i] = new GravityObjectStruct() { position = gravityObjects[i].transform.position, mass = gravityObjects[i].Mass };
        }

        ComputeBuffer gravityObjectsBuffer = new ComputeBuffer(gravityObjects.Count, sizeof(float) * 4); // Init compute buffer of length count*structByteSize
        gravityObjectsBuffer.SetData(gravityObjectsStructs);

        GravityVector[] gravityVectors = new GravityVector[gravityObjects.Count];
        ComputeBuffer gravityVectorsBuffer = new ComputeBuffer(gravityVectors.Length, sizeof(float) * 3); // Init compute buffer of length count*structByteSize
        gravityVectorsBuffer.SetData(gravityVectors);

        gravityShader.SetBuffer(0, "gos", gravityObjectsBuffer);
        gravityShader.SetBuffer(0, "gvs", gravityVectorsBuffer);
        gravityShader.SetFloat("graviticConstant", graviticConstant);
        gravityShader.Dispatch(0, Mathf.CeilToInt(gravityObjects.Count / 2), 2, 1);

        gravityVectorsBuffer.GetData(gravityVectors);

        for (int i = 0; i < gravityObjects.Count; i++) {
            Vector3 acc = gravityVectors[i].acceleration;
            GravityObject go = gravityObjects[i];

            if (go.Rb) { go.Rb.AddForce(acc, ForceMode.Force); }
            else if (go.Rb2d) { go.Rb2d.AddForce(acc, ForceMode2D.Force); }
        }

        gravityObjectsBuffer.Release();
        gravityVectorsBuffer.Release();
    }
}
