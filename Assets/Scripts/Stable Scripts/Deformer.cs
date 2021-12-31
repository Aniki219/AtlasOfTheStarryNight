using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Deformer : MonoBehaviour
{
    Material defaultMaterial;
    public Material flashMaterial;

    Collider2D col;

    Vector3 startPosition;
    Vector3 startRotation;
    Vector3 startScale;

    Vector3 totalScale = Vector3.one;
    Vector3 totalOffset = Vector3.zero;

    float frameCount;

    [SerializeField]
    public List<Oscillator> oscillators;
    [SerializeField]
    public List<Deformation> deforms;

    public void Start()
    {
        col = GetComponentInParent<Collider2D>();
        //deforms = new List<Deformation>();

        //frameCount is to remain sync'd for all oscillators
        frameCount = (int)Random.Range(0, 100) * Time.deltaTime;

        startPosition = transform.localPosition;
        startRotation = transform.localEulerAngles;
        startScale = transform.localScale;

        //Setup Deformers
        foreach (Deformation d in deforms)
        {
            d.startTime += Time.time;
        }

        //Setup Oscillators
        foreach (Oscillator o in oscillators)
        {
            switch ((int)o.axis)
            {
                case 0:
                    o.oscillationDirection = transform.right;
                    break;
                case 1:
                    o.oscillationDirection = transform.up;
                    break;
                case 2:
                    o.oscillationDirection = transform.forward;
                    break;
            }
            if (o.rotational)
            {
                transform.localEulerAngles -= o.oscillationDirection * o.oscillationSize * 0.5f;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        frameCount += Time.deltaTime;

        totalScale = Vector3.one;
        totalOffset = Vector3.zero;

        RemoveDisabledDeforms();
        UpdateDeformers();
        UpdateOscillators();

        startScale = Vector3.Scale(
            new Vector3(
                Mathf.Abs(startScale.x),
                Mathf.Abs(startScale.y),
                Mathf.Abs(startScale.z)
            ), 
            new Vector3(
                Mathf.Sign(transform.localScale.x),
                Mathf.Sign(transform.localScale.y),
                Mathf.Sign(transform.localScale.z)
            ));

        transform.localScale = Vector3.Scale(startScale, totalScale);
        transform.localPosition = totalOffset;
    }

    void UpdateOscillators()
    {
        totalOffset = startPosition;
        foreach (Oscillator o in oscillators)
        {
            if (!o.enabled) continue;
            if (o.rotational)
            {
                float angle = Mathf.Sin(frameCount * 2.0f * Mathf.PI * o.cyclesPerSecond);
                transform.localRotation = Quaternion.Euler(
                    startRotation +
                    o.oscillationDirection *
                    angle *
                    o.oscillationSize *
                    transform.localScale.y); //Upside down switches CW to CCW
            }
            else
            {
                totalOffset += o.oscillationDirection * Mathf.Sin(frameCount * 2.0f * Mathf.PI * o.cyclesPerSecond) * o.oscillationSize;
            }
        }
    }

    public void SetOscillatorActive(string tag = "default", bool on = true)
    {
        foreach (Oscillator o in oscillators)
        {
            if (o.tag == tag)
            {
                o.enabled = on;
            }
        }
    }

    void UpdateDeformers()
    {
        foreach (Deformation d in deforms)
        {
            if (!d.enabled) continue;

            Vector3 newScale = Vector3.one;
            Vector3 newOffset = Vector3.zero;

            float elapsedTime = Time.time - d.startTime;

            if (elapsedTime <= d.timeTo)
            {
                newScale = Vector3.Lerp(Vector3.one, d.to, elapsedTime / d.timeTo);
            }
            else 
            {
                if (d.timeReturn < 0)
                {
                    newScale = d.to;
                } else
                {
                    newScale = Vector3.Lerp(d.to, Vector3.one, (elapsedTime - d.timeTo) / d.timeReturn);
                }
            }

            if (col != null)
            {
                newOffset = startPosition + Vector3.up * (col.bounds.extents.y - col.offset.y) * (1.0f - newScale.y) * d.offsetDir;
            }

            if (d.timeReturn > 0 && elapsedTime >= d.timeTo + d.timeReturn)
            {
                d.enabled = false;
            }

            totalScale.Scale(newScale);
            totalOffset += newOffset;
        }
    }

    void RemoveDisabledDeforms()
    {
        deforms.RemoveAll(d => !d.enabled);
    }

    public void RemoveDeform(string tag)
    {
        deforms.RemoveAll(d => d.tag.Equals(tag));
    }

    public void startDeform(Vector3 to, float timeTo, float timeReturn = 0.5f, float offsetDir = 0, string tag = "default", bool unique = false)
    {
        Deformation newDeform = new Deformation(to, timeTo, timeReturn, offsetDir);
        newDeform.tag = tag;
        if (unique && tag != "default" && tag != null && tag != "")
        {
            if (deforms.Find(d => d.tag.Equals(tag)) != null)
            {
                return;
            }
        }
        deforms.Add(newDeform);
    }

    public void flashWhite(float time = 0.1f)
    {
        StartCoroutine(flashCoroutine(time));
    }

    private IEnumerator flashCoroutine(float time)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
        sprite.material = flashMaterial;
        yield return new WaitForSeconds(time);
        sprite.material = playerStatsManager.Instance.currentSkin;
    }
}

[System.Serializable]
public class Oscillator
{
    public enum Axes
    {
        x = 0,
        y = 1,
        z = 2
    }

    public string tag = "default";
    public bool enabled = true;
    public Axes axis = Axes.x;
    public bool rotational = false;
    public float cyclesPerSecond = 1.0f;
    public float oscillationSize = 5.0f;
    [HideInInspector] public Vector3 oscillationDirection;

    public Oscillator()
    {
        cyclesPerSecond = 1.0f;
        axis = Axes.x;
        oscillationSize = 5.0f;
        rotational = false;
        tag = "default";
        enabled = true;
    }
}

[System.Serializable]
public class Deformation
{
    public string tag = "default";
    public bool enabled = true;
    public Vector3 to;
    public float timeTo;
    public float timeReturn = 0.5f;
    public float offsetDir = 0;
    public float startTime;

    public Deformation(Vector3 to, float timeTo, float timeReturn = 0.5f, float offsetDir = 0)
    {
        this.to = to;
        this.timeTo = timeTo;
        this.timeReturn = timeReturn;
        this.offsetDir = offsetDir;
        startTime = Time.time;
    }
}
