using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroomEffectsController : MonoBehaviour
{
    TrailRenderer trailRenderer;
    ParticleSystem speedCone;
    ParticleSystem novaSparkles;
    ParticleSystem slipSparks;

    public GameObject novaAirPuff;
    // Start is called before the first frame update
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        speedCone = transform.Find("SpeedCone").GetComponent<ParticleSystem>();
        novaSparkles = transform.Find("NovaSparkles").GetComponent<ParticleSystem>();
        slipSparks = transform.Find("SlipSparks").GetComponent<ParticleSystem>();

        SetTrail(false);
        SetSpeedCone(false);
        SetNovaSparkles(false);
        SetSlipSparks(false);
    }

    public void SetTrail(bool on = true) {
        trailRenderer.enabled = on;
    }

    public void SetSpeedCone(bool on = true) {
        if (on) {
            speedCone.Play();
        } else {
            speedCone.Stop();
        }
    }

    public void SetNovaSparkles(bool on = true) {
        if (on) {
            if (!novaSparkles.isPlaying) {
                novaSparkles.Play();
            } 
        } else {
            novaSparkles.Stop();
        }
    }

    public void SetSlipSparks(bool on = true) {
        if (on) {
            if (!slipSparks.isPlaying) {
                slipSparks.Play();
            } 
        } else {
            slipSparks.Stop();
        }
    }

    public void createNovaAirPuff() {
        Instantiate(novaAirPuff, transform);
    }
}
