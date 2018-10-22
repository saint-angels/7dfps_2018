using UnityEngine;
using System.Collections;

public class BoidBehaviour : MonoBehaviour
{
    // Reference to the controller.
    public BoidController controller;

    // Options for animation playback.
    public float animationSpeedVariation = 0.2f;

    // Random seed.
    float noiseOffset;

    // Caluculates the separation vector with a target.
    Vector3 GetSeparationVector(Transform target)
    {
        var diff = transform.position - target.transform.position;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / controller.neighborDist);
        return diff * (scaler / diffLen);
    }

    void Start()
    {
        noiseOffset = Random.value * 10.0f;

        var animator = GetComponent<Animator>();
        if (animator)
            animator.speed = Random.Range(-1.0f, 1.0f) * animationSpeedVariation + 1.0f;
    }

    private void Update()
    {
        Vector3 direction = controller.transform.position - transform.position;
        Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, controller.rotationCoeff * Time.time);
        transform.position = transform.position + transform.forward * (controller.velocity * Time.deltaTime);
    }

    void Update2()
    {
        var currentPosition = transform.position;
        var currentRotation = transform.rotation;

        // Current velocity randomized with noise.
        var noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
        var velocity = controller.velocity * (1.0f + noise * controller.velocityVariation);

        // Initializes the vectors.
        var separation = Vector3.zero;
        var alignment = controller.transform.forward;
        var cohesion = controller.transform.position;

        // Looks up nearby boids.
        var nearbyBoids = Physics.OverlapSphere(currentPosition, controller.neighborDist, controller.searchLayer);

        // Accumulates the vectors.
        foreach (var boid in nearbyBoids)
        {
            //if (boid.gameObject == gameObject) continue;
            var t = boid.transform;
            separation += GetSeparationVector(t);
            alignment += t.forward;
            cohesion += t.position;
        }

        var avg = 1.0f / 250f;
        alignment += Random.insideUnitSphere * controller.noiseAlignment;
        alignment *= avg;
        cohesion *= avg;
        cohesion = (cohesion - currentPosition).normalized;

        // Calculates a rotation from the vectors.
        var direction = separation + alignment + cohesion;
        var rotation = Quaternion.FromToRotation(Vector3.forward, alignment);

        // Applys the rotation with interpolation.
        //Quaternion targetRotation;
        if (rotation != currentRotation)
        {
            //targetRotation = rotation;
            var ip = Mathf.Exp(-controller.rotationCoeff * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(rotation, currentRotation, ip);
        }
        //else
        //{
        //    transform.LookAt(controller.transform.position);
        //}
        

        // Moves forawrd.
        transform.position = currentPosition + transform.forward * (velocity * Time.deltaTime);
    }
}
