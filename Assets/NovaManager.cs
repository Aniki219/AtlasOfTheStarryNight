using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Nova is a resource whose value is clamped between 0 and 100
The resource is kept within the ResourceManager class
The nova bar is stored in the CanvasController class and governs itself
*/
public class NovaManager : MonoBehaviour
{
    public enum GainSpeed {
        Slow = 100,
        Fast = 150,
        SlowMinus = -150,
        FastMinus = -400,
        Hold = 0,
        HoldCharged = 10000,
        Clear = -10000
    }
    GainSpeed gain = GainSpeed.Clear;
    private bool resetRequest;
    ResourceManager.Resource nova;
    private float chargedMin = 80f;
    public bool novaDashing;

    void Start()
    {
        nova = ResourceManager.Instance.nova;
    }

    void Update()
    {
        nova.plus(((float)gain) * Time.deltaTime);
    }

    void LateUpdate() {
        if (resetRequest) {
            nova.setValue(0);
            gain = GainSpeed.Clear;
        }
    }

    //Nova related behaviors will submit a reset request during their End phase
    //but if the new state Starts and continues the nova we cancel the reset request 
    public void setGain(GainSpeed to) {
        gain = to;
        if (gain.Equals(GainSpeed.HoldCharged)) {
            if (isCharged()) {
                nova.setValue(nova.max);
            } else {
                gain = GainSpeed.Clear;
            }
        }
        resetRequest = false;
    }

    public void ResetRequest() {
        resetRequest = true;
    }

    public bool isCharged() {
        return nova.value >= chargedMin;
    }

    public void setNovaDashing(bool on = true) {
        novaDashing = on;
    }

    public bool isNovaDashing() {
        return novaDashing;
    }
}
