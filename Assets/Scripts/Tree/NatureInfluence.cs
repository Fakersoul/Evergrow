using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GrowingSpline))]
public class NatureInfluence : MonoBehaviour
{
    [Header("Starting Values")]
    [SerializeField]
    [Tooltip("Amount of water (%) at the start")]
    [Range(0.0f, 100.0f)]
    float waterLevel = 75.0f;
    [SerializeField]
    [Tooltip("Amount of sun (%) at the start")]
    [Range(0.0f, 100.0f)]
    float sunLevel = 75.0f;
    [Space]

    [SerializeField]
    [Tooltip("Amount of seconds needed to decrease/increase with certain values")]
    float amountSeconds = 2.0f;

    [Header("Reduction")]
    [SerializeField]
    [Tooltip("Amount of water (%) that is reduced of total percentage after amount seconds")]
    [Range(0.0f, 100.0f)]
    float waterDecline = 1.0f;
    [SerializeField]
    [Tooltip("Amount of sunlight (%) that is reduced of total percentage after amount seconds")]
    [Range(0.0f, 100.0f)]
    float sunDecline = 1.0f;

    [Header("Increase")]
    [SerializeField]
    [Tooltip("Amount of raindrops to increase with water increase value")]
    int amountRaindrops = 1;
    [SerializeField]
    [Tooltip("Amount of water (%) that is reduced of total percentage after amount seconds")]
    [Range(0.0f, 100.0f)]
    float waterIncrease = 1.0f;
    [SerializeField]
    [Tooltip("Amount of sunlight (%) that is reduced of total percentage after amount seconds")]
    [Range(0.0f, 100.0f)]
    float sunIncrease = 1.0f;

    [Header("Influence curves")]
    [SerializeField]
    [Tooltip("X-axis is percentage value and Y-axis is influence value")]
    AnimationCurve influenceCurveSunlight = AnimationCurve.EaseInOut(0.0f, 0.25f, 100.0f, 1.0f);
    [Tooltip("X-axis is percentage value and Y-axis is influence value")]
    [SerializeField]
    AnimationCurve influenceCurveWater = AnimationCurve.EaseInOut(0.0f, 0.25f, 100.0f, 1.0f);

    GrowingSpline spline = null;
    
    List<ParticleCollisionEvent> rainCollisions = new List<ParticleCollisionEvent>(); // Not using this list... It is used for memory efficiency in the particle collision function

    void Start()
    {
        spline = GetComponent<GrowingSpline>();
    }

    // Update is called once per frame
    void Update()
    {
        float ratio = (Time.deltaTime / amountSeconds);
        waterLevel -= ratio * waterDecline;
        sunLevel -= ratio * sunDecline;

        waterLevel = Mathf.Clamp(waterLevel, 0.0f, 100.0f);
        sunLevel = Mathf.Clamp(sunLevel, 0.0f, 100.0f);

        spline.GrowthSpeed = spline.InitialGrowthSpeed * influenceCurveWater.Evaluate(waterLevel) * influenceCurveSunlight.Evaluate(sunLevel);
    }

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem part = other.GetComponent<ParticleSystem>();
        if (!part)
            Debug.LogError("No particle system found");
        int numCollisionEvents = part.GetCollisionEvents(gameObject, rainCollisions);

        waterLevel += ((float)numCollisionEvents / amountRaindrops) * waterIncrease;
    }

    void OnDrawGizmosSelected()
    {

    }
}
