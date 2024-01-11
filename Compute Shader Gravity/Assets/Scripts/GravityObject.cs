using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GravityObject : MonoBehaviour {
    [SerializeField] private float mass;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rigidbody2D rb2d;

    public float Mass { get => mass; set => mass = value; }
    public Rigidbody Rb { get => rb; }
    public Rigidbody2D Rb2d { get => rb2d; }

    private void OnEnable() {
        mass = Mathf.PI * transform.localScale.x * transform.localScale.y;
        if (rb) rb.mass = mass;
        if (rb2d) rb2d.mass = mass;

        GravityShader.instance.AddGravityObject(this);
    }

    private void OnDisable() {
        GravityShader.instance.RemoveGravityObject(this);
    }

    private void OnCollisionEnter2D(Collision2D _collision) {
        GravityObject go = _collision.gameObject.GetComponent<GravityObject>();
        if (go == null) return;
        if (go.Mass > mass) return;

        mass += go.Mass;
        if (rb) rb.mass = mass;
        if (rb2d) rb2d.mass = mass;
        transform.localScale = Mathf.Sqrt(mass / Mathf.PI) * Vector3.one;

        Destroy(go.gameObject);
    }
}
