using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    public float throwForce = 10f;
    public bool isPickedUp;
    private Rigidbody rb;

    public static List<Food> foodList = new List<Food>();

    private new Renderer renderer;

    public bool lookedUpon = false;
    private Vector3 spawnPoint;

	void Start ()
    {
        renderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        foodList.Add(this);
        spawnPoint = transform.position;
    }
	
	void LateUpdate () {

        renderer.material.SetFloat("_Outline", 0f);
    }

    public void OnLookedUpon()
    {
        renderer.material.SetFloat("_Outline", .02f);
    }

    public void PickUp()
    {
        rb.isKinematic = true;
        isPickedUp = true;
    }

    public void Throw(Vector3 direction)
    {
        rb.isKinematic = false;
        rb.AddForce(direction * throwForce, ForceMode.Impulse);
        isPickedUp = false;
    }

    public void Eat()
    {
        foodList.Remove(this);
        Destroy(gameObject);
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
