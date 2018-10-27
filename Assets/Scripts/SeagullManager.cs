using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullManager : SingletonComponent<SeagullManager> {

    public Seagull seagullPrefab;
    public int spawnSeagulls = 5;
    public List<Seagull> activeSeagulls = new List<Seagull>();
    public Transform landPoint;
    public Transform[] flyPoints;

    public Vector3 GetRandomFlyPoint()
    {
        var randIndex = Random.Range(0, flyPoints.Length);
        return flyPoints[randIndex].position;
    }

	// Use this for initialization
	void Start () {
        for (int i = 0; i < spawnSeagulls; i++)
        {
            var newSeagull = Instantiate(seagullPrefab, GetRandomFlyPoint(), Quaternion.identity) as Seagull;
            newSeagull.transform.parent = transform;
            activeSeagulls.Add(newSeagull);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
