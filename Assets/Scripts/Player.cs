using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float energyRecoverSpeed = 1f;
    public float energy = 0;

    [SerializeField] private Camera cam;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        float energyGained = 0;
        foreach (var seagull in SeagullManager.Instance.activeSeagulls)
        {
            Vector3 targetDir = seagull.transform.position - cam.transform.position;
            float angle = Vector3.Angle(targetDir, cam.transform.forward);

            bool seagullVisible = angle < 20.0f;
            if (seagullVisible)
            {
                float distanceToSeagull = targetDir.magnitude;
                energyGained += (seagull.transform.localScale.x / distanceToSeagull) * energyRecoverSpeed * Time.deltaTime;
                //print("distance: " + distanceToSeagull);
            }
        }
        //print(energyGained);
        energy += energyGained;
        energy = Mathf.Clamp(energy, 0f, 100f);



    }

    void OnGUI()
    {
        GUI.Label(new Rect(30, 30, 100, 50), energy.ToString());
    }
}
