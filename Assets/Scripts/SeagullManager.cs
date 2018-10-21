using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullManager : SingletonComponent<SeagullManager> {

    public List<Seagull> activeSeagulls = new List<Seagull>();

    public void Register(Seagull newSeagull)
    {
        activeSeagulls.Add(newSeagull);
    }

	// Use this for initialization
	void Start () {
	    	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
