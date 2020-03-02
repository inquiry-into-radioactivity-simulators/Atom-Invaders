using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Atom : MonoBehaviour
{
    public static ArrayList instances;
    public ArrayList radii;
    public GameObject prefab;
    public Molecule connected;
    public bool dirty;
    public int bounceSpring;
    public float nucleusForce;
    public ArrayList ionizedForces;
    public int protons;
    public int neutrons;
    public int electrons;
    public int charge;
    public int radius;
    public ArrayList orbitals;
    public ParticleSystem particles;
    public GameObject flaree;
    public float spawnSpacing;
    public GameObject projectile;
    public GameObject field;
    public float minFieldSize;
    public float maxFieldSize;
    private float minFieldAmt;
    private float maxFieldAmt;
    public GameObject protonPrefab;
    public GameObject neutronPrefab;
    public Rotator rot;
    public Mover mov;
    public Transform trans;
    private Rigidbody nucleus;
    private float neuProCount;
    private int lastCharge;
    private bool zoomed;
    private float id;
    public int smacked;
    public virtual void Start()
    {
        this.trans = this.transform;
        this.id = Random.value;
        this.nucleus = this.transform.Find("Nucleus").GetComponent<Rigidbody>();
        this.mov = this.GetComponent<Mover>();
        this.rot = this.GetComponent<Rotator>();

        this.radii = new ArrayList();
        this.radii.Add(0.0f);
        this.radii.Add(41.0f);
        this.radii.Add(41.0f);
        this.radii.Add(80.0f);
        this.radii.Add(80.0f);
        this.radii.Add(161.0f);
        this.radii.Add(161.0f);
        this.radii.Add(161.0f);
        this.ionizedForces = new ArrayList();
        this.ionizedForces.Add(14.0f);
        this.ionizedForces.Add(29.0f);
        this.ionizedForces.Add(48.0f);
        this.ionizedForces.Add(77.0f);
        this.ionizedForces.Add(98.0f);
        this.ionizedForces.Add(554.0f);
        this.ionizedForces.Add(669.0f);
        this.ionizedForces.Add(1000.0f);
        this.ionizedForces.Add(1000.0f);
        this.ionizedForces.Add(1000.0f);
        this.ionizedForces.Add(1000.0f);
        this.ionizedForces.Add(1000.0f);
        var orbitalComponents = this.GetComponentsInChildren<Orbital>();
        this.orbitals = new ArrayList();
        foreach (Orbital o in orbitalComponents) {
            this.orbitals.Add(o);
        }

        this.RedoOrbitals();
        int p = this.protons;
        while (p > 0)
        {
            this.AddProton(Vector3.zero);
            p--;
        }
        int n = this.neutrons;
        while (n > 0)
        {
            this.AddNeutron(Vector3.zero);
            n--;
        }
        this.SetZoomed(false);
    }

    public virtual void RedoOrbitals()
    {
        int ii = 0;
        while (ii < this.orbitals.Count)
        {
            ((Orbital)(this.orbitals[ii])).electrons = 0;
            ii++;
        }
        int esToAdd = this.electrons;
        int passes = 2;
        int pi = 0;
        while (pi < passes)
        {
            int i = 0;
            while ((esToAdd > 0) && (i < this.orbitals.Count))
            {
                var orbital = (Orbital)(this.orbitals[i]);
                if (orbital.electrons != 2)
                {
                    if ((!orbital.secondPass && (esToAdd > 1)) && (orbital.electrons == 0))
                    {
                        orbital.electrons = orbital.electrons + 2;
                        esToAdd = esToAdd - 2;
                    }
                    else
                    {
                        orbital.electrons = orbital.electrons + 1;
                        esToAdd = esToAdd - 1;
                    }
                }
                i++;
            }
            pi++;
        }
    }

    public virtual void OnEnable()
    {
        Atom.instances.Add(this);
    }

    public virtual void OnDisable()
    {
        int del = -1;
        int i = 0;
        while (i < Atom.instances.Count)
        {
            if (Atom.instances[i] == this)
            {
                del = i;
                i = Atom.instances.Count;
            }
            i++;
        }
        if (del != -1)
        {
            Atom.instances.RemoveAt(del);
        }
    }

    public virtual void LateUpdate()
    {
        int i = 0;
        while (i < Atom.instances.Count)
        {
            Atom instance = (Atom) Atom.instances[i];
            if ((((instance != null) && (instance.trans != null)) && (this.trans != null)) && (instance != this))
            {
                if ((this.electrons > 0) && (instance.electrons > 0))
                {
                    Vector3 toI = instance.trans.position - this.trans.position;
                    float dist = toI.magnitude;
                    // Debug.Log("this.radii[this.electrons]: " + this.radii[this.electrons]);
                    //  Debug.Log("instance.radii[instance.electrons]: " + instance.radii[instance.electrons]);
                    float collDist = (float)(this.radii[this.electrons]) + (float)(instance.radii[instance.electrons]);
                    if ((dist < collDist) && !(dist == 0))
                    {
                        Vector3 force = ((toI / dist) * (1f - Mathf.Clamp01(dist / collDist))) * this.bounceSpring;
                        instance.mov.Accel(force * Time.deltaTime);
                        this.mov.Accel(-force * Time.deltaTime);
                    }
                }
                if (((instance.electrons - instance.protons) < 0) && ((this.electrons - this.protons) < 0))
                {
                    int charge1 = -(instance.electrons - instance.protons) - (this.electrons - this.protons);
                    Vector3 toI1 = instance.trans.position - this.trans.position;
                    float dist1 = toI1.magnitude;
                    float collDist1 = 300;
                    if ((dist1 < collDist1) && !(dist1 == 0))
                    {
                        Vector3 force1 = (((toI1 / dist1) * (1f - Mathf.Clamp01(dist1 / collDist1))) * this.bounceSpring) * charge1;
                        instance.mov.Accel(force1 * Time.deltaTime);
                        this.mov.Accel(-force1 * Time.deltaTime);
                    }
                }
            }
            i++;
        }
        float fieldAmt = Mathf.Log(Mathf.Abs(this.protons - this.electrons)) * 0.11f;
        float colorAmt = Mathf.Lerp(this.minFieldAmt, this.maxFieldAmt, fieldAmt);
        float fieldSize = Mathf.Lerp(this.minFieldSize, this.maxFieldSize, fieldAmt);
        this.field.GetComponent<Renderer>().enabled = this.protons != this.electrons;
        //field.renderer.material = p > e ? posMat : negMat;
        float alpha1 = (190f / 256f) * colorAmt;
        this.field.GetComponent<Renderer>().material.SetColor("_TintColor", this.protons > this.electrons ? new Color(1f, 10f / 256f, 0, alpha1) : new Color(0, 73f / 256f, 1f, alpha1));
        this.field.transform.localScale = Vector3.one * fieldSize;
        this.field.transform.rotation = Quaternion.identity;
        ((Mover) this.GetComponent(typeof(Mover))).x = ((Mover) this.GetComponent(typeof(Mover))).y = !Status.zoomed;
        this.flaree.SetActive(Status.zooming || !Status.zoomed);
        this.flaree.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.black, Status.zoomAmt);
        if (Status.zoomed)
        {
            this.gameObject.layer = 2;
        }
        else
        {
            this.gameObject.layer = 1;
        }
        this.charge = this.protons - this.electrons;
        Color tint = this.charge > 0 ? Color.Lerp(new Color(0.3f, 0.3f, 0.3f, 0.5f), new Color(1, 0.2f, 0.2f, 0), (float)this.charge / this.protons) : Color.Lerp(new Color(0.3f, 0.3f, 0.3f, 0.5f), new Color(0.2f, 0.2f, 1, 0.5f), (float)(-this.charge) / this.electrons);
        tint.a = (1 - Status.zoomAmt) * 0.5f;
        this.particles.GetComponent<Renderer>().material.SetColor("_TintColor", tint);
        this.particles.GetComponent<Renderer>().enabled = Status.zoomAmt != 1;
        /**
	if(charge != lastCharge) {
		//Status.SpawnBalloon(Atom.GetNumString(charge), charge > 0 ? Status.pColor : Status.eColor, 2, transform.position, transform, gameObject.name + id);	
	}
	*/
        this.lastCharge = this.charge;
        this.smacked = this.smacked - 1;
    }

    public static string GetNumString(int num)
    {
        if (num != 0)
        {
            return (num > 0 ? "+" : "-") + Mathf.Abs(num).ToString();
        }
        else
        {
            return "0";
        }
    }

    /*
function FixedUpdate ()
{
	var i = 0;
	while(i < nucleus.transform.childCount)
	{ 
		
		child = nucleus.transform.GetChild(i).rigidbody;
		if(child)
		{
			(nucleus.transform.position - child.transform.position).normalized * nucleusForce
			child.AddForce();
		}
		
		i ++;	
	}	
}
*/
    public virtual void AddProton(Vector3 pos)
    {
        GameObject newP = null;
        Vector3 position = this.nucleus.transform.position + ((Random.insideUnitSphere * 10) * this.spawnSpacing);
        if (pos != Vector3.zero)
        {
            position = pos;
        }
        newP = (GameObject)UnityEngine.Object.Instantiate(this.protonPrefab, position, Quaternion.identity);
        newP.transform.localScale = newP.transform.localScale * this.transform.localScale.x;
        newP.transform.parent = this.nucleus.transform;
        ((NucleusObject) newP.GetComponent(typeof(NucleusObject))).target = this.transform;
        ((NucleusObject) newP.GetComponent(typeof(NucleusObject))).lastTargetPos = this.transform.position;
        foreach (Orbital o in this.orbitals)
        {
            Physics.IgnoreCollision(newP.GetComponent<Collider>(), o.GetComponent<Collider>());
        }
        this.neuProCount++;
    }

    public virtual void AddNeutron(Vector3 pos)
    {
        GameObject newN = null;
        Vector3 position = this.nucleus.transform.position + ((Random.insideUnitSphere * 10) * this.spawnSpacing);
        if (pos != Vector3.zero)
        {
            position = pos;
        }
        newN = (GameObject)UnityEngine.Object.Instantiate(this.neutronPrefab, position, Quaternion.identity);
        newN.transform.localScale = newN.transform.localScale * this.transform.localScale.x;
        newN.transform.parent = this.nucleus.transform;
        ((NucleusObject) newN.GetComponent(typeof(NucleusObject))).target = this.transform;
        ((NucleusObject) newN.GetComponent(typeof(NucleusObject))).lastTargetPos = this.transform.position;
        foreach (Orbital o in this.orbitals)
        {
            Physics.IgnoreCollision(newN.GetComponent<Collider>(), o.GetComponent<Collider>());
        }
        this.neuProCount++;
    }

    private IEnumerator coroutine;

    private IEnumerator decay(Vector3 pos) {
        yield return new WaitForSeconds(1);
        GameObject newProjectileObject = (GameObject)UnityEngine.Object.Instantiate(this.projectile, pos, Quaternion.AngleAxis(Random.value * 360, Vector3.up));
        Projectile newProjectile = (Projectile) newProjectileObject.GetComponent(typeof(Projectile));
        newProjectile.energy = 20;
        newProjectile.zoomed = true;
        newProjectile.Init(0);
        newProjectile.noTimeOut = true;
        GameObject prefabb = (GameObject) Resources.Load("follow", typeof(GameObject));
        if (prefabb)
        {
            GameObject newFollowCam = (GameObject)UnityEngine.Object.Instantiate(prefabb);
            ((FollowCam) newFollowCam.GetComponent(typeof(FollowCam))).proj = newProjectile.gameObject;
        }
        this.neutrons--;
        this.protons++;
    }
    public void AbsorbNeut(Vector3 pos)
    {
        this.dirty = true;
        this.neutrons++;
        this.AddNeutron(pos);
        if ((this.neutrons == 9) && (this.protons == 7))
        {
            this.coroutine = decay(pos);
            StartCoroutine(this.coroutine);
        }
    }

    

    public virtual void SetZoomed(bool zoom)
    {
        this.zoomed = zoom;
        int i = 0;
        while (i < this.nucleus.transform.childCount)
        {
            GameObject child = this.nucleus.transform.GetChild(i).gameObject;
            if (child)
            {
                child.layer = zoom ? 0 : 2;
            }
            i++;
        }
    }

    public virtual bool CheckForIonization(float p, int type)
    {
        if ((type == 0) && Mathf.Approximately(p, 1000))
        {
            return true;
        }
        //Debug.Log("this.ionizedForces[this.charge]: " + this.ionizedForces[this.charge]);
        return Random.value < Mathf.InverseLerp((float)(this.ionizedForces[this.charge]) / 1.01f, (float)(this.ionizedForces[this.charge]) * 1.01f, p);
    }

    public virtual void ESmack(float energy, int type, Transform trans)
    {
        if (this.smacked > 0)
        {
            return;
        }
        this.smacked = 5;
        this.dirty = true;
        Vector3 outDir = (Vector3.Scale(new Vector3(1, 0, 1), Random.onUnitSphere) + trans.forward) + (trans.position - this.transform.position).normalized;
        if (outDir.z < 0)
        {
            outDir.z = -outDir.z;
        }
        GameObject newProjectileObject = (GameObject)UnityEngine.Object.Instantiate(this.projectile, ((trans.position * 0.25f) + (this.transform.position * 0.75f)) + (outDir * 10), Quaternion.LookRotation(outDir));
        Projectile newProjectile = (Projectile) newProjectileObject.GetComponent(typeof(Projectile));
        newProjectile.energy = energy > 2000 ? 1000 * 0.05f : energy * 0.05f;
        newProjectile.reacted = false;
        newProjectile.ignoreAtom = this;
        newProjectile.ignoreCollisions = false;
        newProjectile.Init(0);
        this.electrons--;
        this.RedoOrbitals();
        if (type == 3)
        {
            UnityEngine.Object.Destroy(trans.root.gameObject);
        }
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }

    public Atom()
    {
        this.bounceSpring = 10;
        this.radius = 400;
        this.spawnSpacing = 0.1f;
        this.minFieldSize = 3f;
        this.maxFieldSize = 10f;
        this.minFieldAmt = 0.15f;
        this.maxFieldAmt = 1f;
    }

    static Atom()
    {
        Atom.instances = new ArrayList();
    }

}