using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class NucleusObject : MonoBehaviour
{
    public int ds;
    public float speed;
    public float speedFalloffDistMin;
    public float speedFalloffDistMax;
    public float force;
    public float predictTarget;
    public float predictTargetSharpness;
    public float predictThis;
    public float predictThisSharpness;
    public Transform target;
    public Vector3 lastTargetPos;
    private Vector3 predictThisVel;
    private Vector3 predictTargetVel;
    private Atom atom;
    private bool warned;
    public virtual void Start()
    {
        this.atom = (Atom) this.transform.root.GetComponent(typeof(Atom));
    }

    public virtual void FixedUpdate()
    {
        this.ds++;
        if ((this.ds < 3) && this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        if (this.GetComponent<Rigidbody>())
        {
            // get the smooothed velocity of this and the target this frame
            this.predictThisVel = Vector3.Lerp(this.predictThisVel, this.GetComponent<Rigidbody>().velocity, Time.fixedDeltaTime * this.predictThisSharpness);
            this.predictTargetVel = Vector3.Lerp(this.predictTargetVel, (this.target.position - this.lastTargetPos) / Time.fixedDeltaTime, Time.fixedDeltaTime * this.predictTargetSharpness);
            this.lastTargetPos = this.target.position;
            // predict future positions for this and the target
            Vector3 predictedTarget = this.target.position + (this.predictTargetVel * this.predictTarget);
            Vector3 predictedPosition = this.transform.position + (this.predictThisVel * this.predictThis);
            // construct a velocity vector from our position to the target's, tweaking the falloff relative to distance
            Vector3 toTarget = predictedTarget - predictedPosition;
            Vector3 wantedVelocity = (toTarget.normalized * Mathf.InverseLerp(-this.speedFalloffDistMin, this.speedFalloffDistMax, toTarget.magnitude)) * this.speed;
            // safeguard against things exploding
            float explosionDanger = Mathf.Clamp01(Mathf.Pow((Vector3.Angle(wantedVelocity, this.GetComponent<Rigidbody>().velocity) / 180f) * (this.GetComponent<Rigidbody>().velocity.magnitude / this.speed), 3));
            this.GetComponent<Rigidbody>().AddForce((-this.GetComponent<Rigidbody>().velocity * explosionDanger) * 0.8f, ForceMode.Acceleration);
            // the final force
            Vector3 usedForce = (wantedVelocity - this.GetComponent<Rigidbody>().velocity) * Mathf.Lerp(this.force, 0f, explosionDanger);
            if ((Mathf.Sin(Time.time * 30) > 0) || !((this.atom && (this.atom.protons == 7)) && (this.atom.neutrons == 9)))
            {
                this.GetComponent<Rigidbody>().AddForce(usedForce, ForceMode.Acceleration);
            }
            else
            {
                this.GetComponent<Rigidbody>().AddForce(-usedForce.normalized * 3, ForceMode.Acceleration);
            }
            // if we are exploding, lower forces so that the simulation becomes sane
            if (explosionDanger > 0.9f)
            {
                this.force = Mathf.Lerp(this.force, this.force * 0.8f, Time.fixedDeltaTime * 3);
                if (!this.warned)
                {
                    Debug.Log(("Forces on object `" + this.gameObject.name) + "` are really crazy, I'm automatically lowering them");
                }
                this.warned = true;
            }
        }
    }

    public NucleusObject()
    {
        this.speed = 100f;
        this.speedFalloffDistMax = 10f;
        this.force = 22f;
        this.predictTarget = 1f;
        this.predictTargetSharpness = 3f;
        this.predictThis = 0.9f;
        this.predictThisSharpness = 3f;
    }

}