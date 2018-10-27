using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrigger : MonoBehaviour {


    private void OnTriggerEnter(Collider other)
    {
        var possiblePlayer = other.GetComponent<Player>();
        if (possiblePlayer != null)
        {
            possiblePlayer.Respawn();
        }

        var possibleFood = other.GetComponent<Food>();
        if (possibleFood != null)
        {
            possibleFood.Respawn();
        }

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
