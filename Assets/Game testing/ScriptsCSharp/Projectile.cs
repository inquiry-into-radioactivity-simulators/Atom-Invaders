using UnityEngine;
using System.Collections;

[System.Serializable]
public class ColoredMaterial : object
{
    public Renderer ren;
    public ParticleSystem particleSystem;
    public Color[] col;
}
[System.Serializable]
public partial class Projectile : MonoBehaviour
{
    public bool reacted;
    public bool ignoreCollisions;
    public int type;
    public Atom ignoreAtom;
    public ColoredMaterial[] mats;
    public float energy;
    public bool zoomed;
    public float zoomedScale;
    public float zoomedSpeedScale;
    public float speedMult;
    public string particleType;
    public LineRenderer lineRenderer;
    public float lineMin;
    public float lineMax;
    public ParticleSystem head;
    public float headMin;
    public float headMax;
    public ParticleSystem trail;
    public float trMin;
    public float trMax;
    public float trLifetimeMin;
    public float trLifetimeMax;
    public float trSMin;
    public float trSMax;
    public float lineStretch;
    public GameObject explosion;
    public GameObject neut;
    public GameObject prot;
    public float range;
    public bool noTimeOut;
    private float speed;
    private Vector3 velocity;
    private float traveled;
    private RaycastHit hit;
    private int lineVerteces;
    private Vector3 origin;
    private bool activated;
    private bool particlesTurnedOn;
    public virtual void Init(int t)
    {
        this.type = t;
        float scale = this.zoomed ? this.zoomedScale : 1f;
        float speedScale = this.zoomed ? this.zoomedSpeedScale : 1f;
        float fxAmt = 1 - Mathf.Clamp01(Mathf.InverseLerp(-1000, 500000, this.energy));
        fxAmt = (1 - Mathf.Pow(fxAmt, 20)) + 0.08f;
        float speedAmt = 1 - Mathf.Clamp01(Mathf.InverseLerp(0, 10000, this.energy));
        speedAmt = (1 - Mathf.Pow(speedAmt, 20)) + 0.007f;
        if (this.type == 3)
        {
            speedAmt = 1;
            if (fxAmt < 0.2f)
            {
                fxAmt = 0.2f;
            }
        }


        var headMainModule = head.main;
  
        var startSizeCurve = new ParticleSystem.MinMaxCurve(0);
        startSizeCurve.mode = ParticleSystemCurveMode.TwoConstants;
        startSizeCurve.constantMin = headMainModule.startSize.constantMin * Mathf.Lerp(headMin, headMax, fxAmt) * scale;
        startSizeCurve.constantMax = headMainModule.startSize.constantMax * Mathf.Lerp(headMin, headMax, fxAmt) * scale;

        headMainModule.startSize = startSizeCurve;
  
        this.lineRenderer.startWidth = Mathf.Lerp(this.lineMin, this.lineMax, fxAmt) * scale;
        this.lineRenderer.endWidth = Mathf.Lerp(this.lineMin, this.lineMax, fxAmt) * scale;

        var trailEmissionModule = trail.emission;

        var rateOverTimeCurve = new ParticleSystem.MinMaxCurve(0);
        rateOverTimeCurve.mode = ParticleSystemCurveMode.TwoConstants;
        rateOverTimeCurve.constantMin = trailEmissionModule.rateOverTime.constantMin * Mathf.Lerp(trMin, trMax, fxAmt-0.15f) * scale;
        rateOverTimeCurve.constantMax = trailEmissionModule.rateOverTime.constantMax * Mathf.Lerp(trMin, trMax, fxAmt-0.15f) * scale;

        trailEmissionModule.rateOverTime = rateOverTimeCurve;
        //trailEmissionModule.rateOverDistance = rateOverTimeCurve;

        var trailMainModule = trail.main;

        var startSizeCurve2 = new ParticleSystem.MinMaxCurve(0);
        startSizeCurve2.mode = ParticleSystemCurveMode.TwoConstants;
        startSizeCurve2.constantMin = trailMainModule.startSize.constantMin * Mathf.Lerp(trSMin, trSMax, fxAmt) * scale;
        startSizeCurve2.constantMax = trailMainModule.startSize.constantMax * Mathf.Lerp(trSMin, trSMax, fxAmt) * scale;

        var startLifetimeCurve = new ParticleSystem.MinMaxCurve(0);
        startLifetimeCurve.mode = ParticleSystemCurveMode.TwoConstants;
        startLifetimeCurve.constantMin = headMainModule.startLifetime.constantMin * Mathf.Lerp(trLifetimeMin, trLifetimeMax, fxAmt) * scale;
        startLifetimeCurve.constantMax = headMainModule.startLifetime.constantMax * Mathf.Lerp(trLifetimeMin, trLifetimeMax, fxAmt) * scale;

        trailMainModule.startLifetime = startLifetimeCurve;
        trailMainModule.startSize = startSizeCurve2;

        this.speed = (speedAmt * this.speedMult) * speedScale;
        this.range = this.range * speedScale;
        if (!this.activated)
        {
            this.Activate();
        }
    }

    public virtual void Activate()
    {
        if (this.zoomed)
        {
            if ((this.energy != 20) && this.React((Atom) Status.hitTransform.GetComponent(typeof(Atom)), Status.hitTransform.position + ((Random.onUnitSphere * ((Atom) Status.hitTransform.GetComponent(typeof(Atom))).radius) * 0.7f)))
            {
                UnityEngine.Object.Destroy(this.gameObject);
                return;
            }
            GameObject parti = null;
            if (this.type == 1)
            {
                parti = (GameObject)UnityEngine.Object.Instantiate(this.neut, this.transform.position, Quaternion.identity);
            }
            if (this.type == 2)
            {
                if (this.energy == 1)
                {
                    UnityEngine.Object.Destroy(this.gameObject);
                    return;
                }
                parti = (GameObject)UnityEngine.Object.Instantiate(this.prot, this.transform.position, Quaternion.identity);
            }
            if (parti)
            {
                UnityEngine.Object.Destroy(parti.GetComponent<Rigidbody>());
                UnityEngine.Object.Destroy(parti.GetComponent<Collider>());
                parti.transform.localScale = parti.transform.localScale * 0.7f;
                parti.transform.parent = this.transform;
            }
            if ((this.type != 0) && (this.energy < 100))
            {
                this.lineRenderer.enabled = false;
            }
        }
        
        foreach (ColoredMaterial matt in this.mats)
        {
            if(matt.particleSystem) {
                var mainModule = matt.particleSystem.main;
                mainModule.startColor = matt.col[this.type];
            } else if (matt.ren && matt.ren.material)
            {
                matt.ren.material.SetColor("_TintColor", matt.col[this.type]);
            }
        }
        this.origin = this.transform.position;
        this.velocity = this.transform.TransformDirection(Vector3.forward) * this.speed;
        if (this.lineRenderer)
        {
            this.lineVerteces++;
            this.lineRenderer.SetVertexCount(this.lineVerteces);
            this.lineRenderer.SetPosition(this.lineVerteces - 1, this.transform.position);
        }
        this.activated = true;
        if ((this.type == 2) && (Status.zoomed == true))
        {
            this.StartCoroutine(this.Bend((Atom) Status.hitTransform.root.GetComponent(typeof(Atom)), 6));
            this.energy = Mathf.Clamp(this.energy, 0, 200);
            float speedAmt = 1 - Mathf.Clamp01(Mathf.InverseLerp(0, 10000, this.energy));
            float speedScale = this.zoomed ? this.zoomedSpeedScale : 1f;
            speedAmt = (1 - Mathf.Pow(speedAmt, 20)) + 0.007f;
            this.speed = ((this.speedMult * speedAmt) * speedScale) * 0.5f;
            this.velocity = this.transform.TransformDirection(Vector3.forward) * this.speed;
        }
    }

    public virtual void Update()
    {
        if (!this.activated)
        {
            return;
        }
        if(!this.particlesTurnedOn) {
            var headEmissionModule = head.emission;
            var trailEmissionModule = trail.emission;
            headEmissionModule.enabled = true;
            trailEmissionModule.enabled = true;
            this.particlesTurnedOn = true;
        }


        float distance = this.velocity.magnitude * Time.deltaTime;
        if (this.lineRenderer)
        {
            this.lineVerteces++;
            this.lineRenderer.SetVertexCount(this.lineVerteces);
            this.lineRenderer.SetPosition(this.lineVerteces - 1, this.transform.position);
        }
        if (this.lineRenderer && (this.type == 3))
        {
            this.lineVerteces++;
            this.lineRenderer.SetVertexCount(this.lineVerteces);
            this.lineRenderer.SetPosition(this.lineVerteces - 1, this.transform.position + (((this.transform.right * (Random.value - 0.5f)) * (Status.zoomed ? 0.5f : 170)) * 0.05f));
        }
        RaycastHit[] hits = null;
        hits = Physics.RaycastAll(this.transform.position, this.velocity, distance);
        foreach (RaycastHit hit in hits)
        {
            Atom atom = null;
            NucleusObject nuc = null;
            GameObject other = null;
            if (hit.collider)
            {
                other = hit.collider.transform.root.gameObject;
                atom = (Atom) other.GetComponent(typeof(Atom));
                nuc = (NucleusObject) hit.collider.GetComponent(typeof(NucleusObject));
            }
            if ((((!Status.zoomed && !this.reacted) && !this.ignoreCollisions) && (distance > 0f)) && other)
            {
                if (atom)
                {
                    if (!hit.collider.isTrigger)
                    {
                        this.reacted = true;
                    }
                    this.React(atom, hit.point);
                }
                else
                {
                    this.transform.position = hit.point;
                    this.StartCoroutine(this.Done());
                }
            }
            else
            {
                if (((((((!this.reacted && Status.zoomed) && (distance > 0f)) && other) && nuc) && (atom != null)) && (atom.neutrons != 0)) && (atom.neutrons < 9))
                {
                    float rv = Random.value;
                    this.reacted = true;
                    if ((((rv > 0.8f) && (this.energy < 200)) || ((rv > 0.95f) && (this.energy < 2000))) && (this.type == 1))
                    {
                        atom.AbsorbNeut(hit.point + (hit.normal * 0.1f));
                        this.transform.position = hit.point;
                        this.StartCoroutine(this.Done());
                    }
                }
            }
        }
        this.transform.position = this.transform.position + (this.velocity * Time.deltaTime);
        this.traveled = this.traveled + distance;
        if (this.lineRenderer)
        {

            {
                float _6 = Vector3.Distance(this.transform.position, this.origin) * this.lineStretch;
                Vector2 _7 = this.lineRenderer.material.mainTextureScale;
                _7.x = _6;
                this.lineRenderer.material.mainTextureScale = _7;
            }
        }
        if ((this.traveled >= this.range) && (!this.noTimeOut || !Status.zoomed))
        {
            this.StartCoroutine(this.Done());
        }
    }

    public virtual bool React(Atom atom, Vector3 pos)
    {
        if (this.ignoreAtom && (this.ignoreAtom == atom))
        {
            return false;
        }
        bool myreacted = false;
        if (this.type == 1)
        {
            return false;
        }
        if ((((((Random.value > 0.7f) && atom) && (this.type == 2)) && (this.energy > (float)(atom.ionizedForces[atom.charge]))) && (this.energy < 2000)) && (atom.electrons > 1))
        {
            atom.electrons--;
            atom.RedoOrbitals();
            atom.dirty = true;
            this.StartCoroutine(this.Done());
            GameObject hydrogen = (GameObject)UnityEngine.Object.Instantiate(atom.prefab, pos, Quaternion.identity);
            ((Atom) hydrogen.GetComponent(typeof(Atom))).electrons = 1;
            ((Atom) hydrogen.GetComponent(typeof(Atom))).protons = 1;
            ((Atom) hydrogen.GetComponent(typeof(Atom))).neutrons = 0;
            Vector3 dir = (((pos - atom.transform.position).normalized * 1.6f) + Random.onUnitSphere) + (this.velocity * 0.005f);
            ((Mover) hydrogen.GetComponent(typeof(Mover))).SetFactors(dir.x, 0, dir.z);
            return true;
        }
        float gammaProbability = this.energy < 200 ? 0.3f : (this.energy < 2000 ? 0.1f : (this.energy < 20000 ? 0.03f : 0.01f));
        int angle = (atom.protons - atom.electrons) < -2 ? (this.energy < 200 ? 180 : 40) : (this.energy < 200 ? 30 : 22);
        if (((atom.protons - atom.electrons) > -1) && (this.energy > 2000))
        {
            angle = (int) (angle * 1.6f);
        }
        if ((this.type == 0) && (Vector3.Angle(this.transform.position - atom.transform.position, Shooter.instance.transform.position - atom.transform.position) > 22))
        {
            return false;
        }
        if (!atom.CheckForIonization(this.energy, this.type) && (atom.smacked < 0))
        {
            if ((atom.charge > 0) && (this.type == 0))
            {
                atom.electrons++;
                atom.RedoOrbitals();
                atom.smacked = 5;
                this.StartCoroutine(this.Done());
                myreacted = false;
            }
            else
            {
                if (this.type == 0)
                {
                    this.StartCoroutine(this.Bend(atom, 3));
                }
                this.reacted = false;
            }
        }
        else
        {
            if ((atom.electrons > 0) && ((Random.value < gammaProbability) || (this.type != 3)))
            {
                if (((this.type != 0) || (this.energy < 20000)) || (Random.value < 0.3f))
                {
                    atom.ESmack(this.energy, this.type, this.transform);
                    this.energy = this.energy * 0.2f;
                    myreacted = false;
                }
            }
        }
        return myreacted;
    }

    public virtual IEnumerator Bend(Atom atome, float forcee)
    {
        this.traveled = 0;
        float speede = this.velocity.magnitude;
        float diste = (atome.transform.position - this.transform.position).magnitude;
        int radiuse = atome.radius;
        while (diste < (radiuse * 1.5f))
        {
            Vector3 force = this.transform.position - atome.transform.position;
            this.velocity = this.velocity + (((force.normalized * this.speed) * forcee) * Time.deltaTime);
            if (this.velocity.magnitude > speede)
            {
                this.velocity = this.velocity.normalized * speede;
            }
            yield return null;
            diste = (atome.transform.position - this.transform.position).magnitude;
        }
    }

    public IEnumerator Done()
    {
        this.enabled = false;
        yield return null;

        if (this.lineRenderer)
        {
            this.lineRenderer.transform.parent = null;
        }
        int i = 0;
        while (i < this.transform.childCount)
        {
            Transform child = this.transform.GetChild(i);
            if (child.GetComponent<ParticleSystem>())
            {
                var emissionModule = child.GetComponent<ParticleSystem>().emission;
                emissionModule.enabled = false;
            }
            else
            {
  
                UnityEngine.Object.Destroy(child.gameObject);
            }
            i++;
        }
        var children = this.GetComponentsInChildren<Transform>();
        this.transform.DetachChildren();
        
        StartCoroutine(deleteChildren(children));
    }

    private IEnumerator deleteChildren(Transform[] children) {

        yield return new WaitForSeconds(5);
        
        foreach(var child in children) {
            if(child && child.gameObject) {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }
        UnityEngine.Object.Destroy(this.gameObject);
    }

    public Projectile()
    {
        this.zoomedScale = 0.01f;
        this.zoomedSpeedScale = 0.01f;
        this.speedMult = 1f;
        this.particleType = "";
        this.lineMin = 0.5f;
        this.lineMax = 2f;
        this.headMin = 0.5f;
        this.headMax = 2f;
        this.trMin = 0.5f;
        this.trMax = 2f;
        this.trLifetimeMin = 0.5f;
        this.trLifetimeMax = 2f;
        this.trSMin = 0.5f;
        this.trSMax = 2f;
        this.lineStretch = 1f;
        this.range = 1000f;
        this.particlesTurnedOn = false;
    }

}