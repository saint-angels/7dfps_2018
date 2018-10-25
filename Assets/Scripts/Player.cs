﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonComponent<Player> {

    [SerializeField] private Transform itemSlot;
    [SerializeField] private Camera cam;

    [Header("Energy Gaining")]
    public float energyRecoverSpeed = 1f;
    public float energy = 0;

    [Header("Energy Draining")]
    public float energyDrainSpeed = 1f;
    public bool energyDraining = false;

    private Food holdingFood;
    private int itemLayerMask;
    private Vector3 screenCenter;

    // Use this for initialization
    void Start () {
        itemLayerMask = LayerMask.GetMask("Items");
        screenCenter = new Vector3(cam.pixelWidth / 2f, cam.pixelHeight / 2f, cam.nearClipPlane);
    }

    void Update ()
    {
        if (holdingFood == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(screenCenter), out hit, 100f, itemLayerMask))
            {
                Food foodFound = hit.collider.GetComponent<Food>();
                foodFound.OnLookedUpon();
                //TODO: Highlight?
                if (Input.GetKeyDown(KeyCode.Mouse0) && Vector3.Distance(transform.position, foodFound.transform.position) < 2.5f)
                {
                    foodFound.transform.parent = itemSlot;
                    foodFound.transform.localPosition = Vector3.zero;
                    foodFound.PickUp();
                    holdingFood = foodFound;
                }   
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                this.holdingFood.transform.parent = null;
                this.holdingFood.Throw(transform.forward + transform.up);
                holdingFood = null;
            }
        }

        //Energy GAIN
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

        //Energy SPENDING
        if (energyDraining)
        {
            energy -= Time.deltaTime * energyDrainSpeed;
        }


        energy = Mathf.Clamp(energy, 0f, 100f);

        if (Mathf.Approximately(0, energy))
        {
            Die();
        }
    }

    private void Die()
    {
        print("Dead!!!");
    }

    void OnGUI()
    {
        GUI.Label(new Rect(30, 30, 100, 50), energy.ToString());
    }
}
