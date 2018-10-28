using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class Player : SingletonComponent<Player> {

    [SerializeField] private Transform itemSlot;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PostProcessVolume ppVolume;
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.FirstPersonController fpsControler;

    [Header("Energy Gaining")]
    public float energyRecoverSpeed = 1f;
    public float energy = 0;
    public TextMeshProUGUI energyValue;

    public TMP_ColorGradient drainGradient;
    public TMP_ColorGradient gainGradient;
    public TMP_ColorGradient idleGradient;

    [Header("Energy Draining")]
    public float energyDrainSpeed = 1f;
    public bool energyDraining = false;

    private Food holdingFood;
    private int itemLayerMask;
    private int nonPlayerLayerMask;
    private Vector3 screenCenter;

    public bool haveAFriend = false;

    Vignette vignetteLayer = null;



    // Use this for initialization
    void Start () {
        itemLayerMask = LayerMask.GetMask("Items");
        nonPlayerLayerMask = LayerMask.GetMask("Default");
        screenCenter = new Vector3(cam.pixelWidth / 2f, cam.pixelHeight / 2f, cam.nearClipPlane);
        ppVolume.profile.TryGetSettings(out vignetteLayer);

    }

    public void AddAFriend()
    {
        haveAFriend = true;
        energyValue.colorGradientPreset = gainGradient;
        energyValue.text = "You've made a seagull friend";
        fpsControler.m_WalkSpeed *= 2f;
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

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

        energyValue.colorGradientPreset = idleGradient;

        if (haveAFriend == false)
        {
            //Energy GAIN
            float energyGained = 0;
            foreach (var seagull in SeagullManager.Instance.activeSeagulls)
            {
                Vector3 targetDir = seagull.transform.position - cam.transform.position;
                float angle = Vector3.Angle(targetDir, cam.transform.forward);

                bool seagullAngleAlright = angle < 20.0f;
                if (seagullAngleAlright)
                {

                    if (Physics.Linecast(cam.transform.position, seagull.transform.position) == false)
                    {
                        float distanceToSeagull = targetDir.magnitude;
                        energyGained += (seagull.transform.localScale.x / distanceToSeagull) * energyRecoverSpeed * Time.deltaTime;
                        //print("distance: " + distanceToSeagull);
                        energyValue.colorGradientPreset = gainGradient;
                    }
                }
            }
            energy += energyGained;

            if (energyGained == 0)
            {
                //Energy SPENDING
                if (energyDraining)
                {
                    energy -= Time.deltaTime * energyDrainSpeed;
                    energyValue.colorGradientPreset = drainGradient;
                }
            }

            energy = Mathf.Clamp(energy, 0f, 100f);
            vignetteLayer.intensity.value = (1f - energy / 100f) / 2f;
            energyValue.text = string.Format("Energy: {0}%", energy.ToString("0.##"));


            if (Mathf.Approximately(0, energy))
            {
                Die();
            }
        }
    }

    private void Die()
    {
        energy = 10f;
        energyDraining = false;
        Respawn();
    }

    public void Respawn()
    {
        transform.position = spawnPoint.position;
    }
}
