using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    public float throwForce = 10f;
    public bool isPickedUp;
    private Rigidbody rb;

    public static List<Food> foodList = new List<Food>();

	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        foodList.Add(this);
    }
	
	// Update is called once per frame
	void Update () {
		
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
}
