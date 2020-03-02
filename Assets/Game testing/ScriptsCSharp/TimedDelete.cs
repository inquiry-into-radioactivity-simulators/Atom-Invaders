using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class TimedDelete : MonoBehaviour
{
    public float emitTimeOut;
    public float deleteTimeOut;
    public float timeRandom;
    public bool detachChildren;
    public bool fade;
    public string fadeColorName;
    public string fadeFloatName;
    public bool fadeOnNoParent;
    public float fadeStartTime;
    public float fadeTime;
    private float timer;
    private Color originalColor;
    private float originalFloat;
    private float originalParticleMinSize;
    private float originalParticleMaxSize;
    private Vector3 originalParticleRandomVelocity;
    public virtual void Start()
    {
        float r = Random.Range(-this.timeRandom, this.timeRandom);
        this.emitTimeOut = this.emitTimeOut + r;
        this.deleteTimeOut = this.deleteTimeOut + r;
        if (this.fade)
        {
            if (this.GetComponent<Renderer>())
            {
                if (this.fadeColorName != "")
                {
                    this.originalColor = this.GetComponent<Renderer>().material.GetColor(this.fadeColorName);
                }
                if (this.fadeFloatName != "")
                {
                    this.originalFloat = this.GetComponent<Renderer>().material.GetFloat(this.fadeFloatName);
                }
            }
            if (this.GetComponent<Light>())
            {
                this.originalColor = this.GetComponent<Light>().color;
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if (this.fade)
        {
            if (this.GetComponent<Renderer>())
            {
                if (this.fadeColorName != "")
                {
                    this.GetComponent<Renderer>().material.SetColor(this.fadeColorName, Color.Lerp(this.originalColor, Color.clear, (this.timer - this.fadeStartTime) / this.fadeTime));
                }
                if (this.fadeFloatName != "")
                {
                    this.GetComponent<Renderer>().material.SetFloat(this.fadeFloatName, Mathf.Lerp(this.originalFloat, 0f, (this.timer - this.fadeStartTime) / this.fadeTime));
                }
            }
            if (this.GetComponent<Light>())
            {
                this.GetComponent<Light>().color = Color.Lerp(this.originalColor, Color.black, (this.timer - this.fadeStartTime) / this.fadeTime);
            }
        }
        if(GetComponent<ParticleSystem>())
        {
        	if(timer > emitTimeOut) {
                var emissionModule = GetComponent<ParticleSystem>().emission;
                emissionModule.enabled = false;
            }
        }
        if (this.timer > this.deleteTimeOut)
        {
            if (this.detachChildren)
            {
                this.transform.DetachChildren();
            }
            UnityEngine.Object.DestroyObject(this.gameObject);
        }
        if (this.fadeOnNoParent && this.transform.parent)
        {
            return;
        }
        this.timer = this.timer + Time.fixedDeltaTime;
    }

    public TimedDelete()
    {
        this.fadeColorName = "_Color";
        this.fadeFloatName = "";
    }

}