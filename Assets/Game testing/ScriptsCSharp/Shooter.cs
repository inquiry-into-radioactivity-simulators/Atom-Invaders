using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Shooter : MonoBehaviour
{
    public float acceleration;
    public float maxSpeed;
    public float minSpeed;
    public float lean;
    public float movementRange;
    public float bounce;
    public float reloadTime;
    public Transform aimer;
    public int outAimerSize;
    public float inAimerSize;
    public Transform gun;
    public GameObject projectile;
    public float zoomedSpawnDistance;
    public int selectedProjectile;
    public float selectedEnergy;
    private float speed;
    private float timer;
    private RaycastHit hit;
    private Vector3 lastMousePos;
    private Vector3 lookDir;
    private Vector3 lookPoint;
    public bool inMenu;
    public static Shooter instance;
    private Atom scanMe;
    public virtual void Update()
    {
        Shooter.instance = this;
        if (Status.zooming)
        {
            this.aimer.gameObject.SetActive(!Status.zooming);
            return;
        }
        float input = Input.GetAxis("Horizontal");
        if ((input > 0) && (this.speed < this.maxSpeed))
        {
            this.speed = this.speed + (this.acceleration * Time.deltaTime);
        }
        else
        {
            if ((input < 0) && (this.speed > -this.maxSpeed))
            {
                this.speed = this.speed - (this.acceleration * Time.deltaTime);
            }
            else
            {
                if (this.speed > 0)
                {
                    this.speed = this.speed - (this.acceleration * Time.deltaTime);
                }
                else
                {
                    if (this.speed < 0)
                    {
                        this.speed = this.speed + (this.acceleration * Time.deltaTime);
                    }
                }
            }
        }
        if (Mathf.Abs(this.speed) < this.minSpeed)
        {
            this.speed = 0;
        }
        float nextPos = this.transform.position.x + (this.speed * Time.deltaTime);
        if (Mathf.Abs(nextPos) > this.movementRange)
        {
            this.speed = -this.speed * this.bounce;
        }

        {
            float _8 = this.transform.position.x + (this.speed * Time.deltaTime);
            Vector3 _9 = this.transform.position;
            _9.x = _8;
            this.transform.position = _9;
        }
        if (((!Status.scanner && Input.GetButtonDown("Jump")) && (this.timer > this.reloadTime)) && this.aimer.gameObject.activeSelf)
        {
            this.timer = 0;
            this.Fire();
        }
        Vector3 mousePos = Input.mousePosition;
        this.lookPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.y));
        this.aimer.gameObject.SetActive(!TheGUI.inGUI && !this.inMenu);
        this.lookPoint.z = Mathf.Clamp(this.lookPoint.z, this.transform.position.z, this.transform.position.z + 2000);
        this.lookDir = this.lookPoint - this.transform.position;
        this.lookDir.y = 0;
        this.transform.rotation = Quaternion.LookRotation(this.lookDir);

        {
            float _10 = (-input * this.lean) * this.transform.forward.z;
            Vector3 _11 = this.transform.localEulerAngles;
            _11.z = _10;
            this.transform.localEulerAngles = _11;
        }
        this.lastMousePos = mousePos;
        Status.hitAtom = false;
        foreach (Atom a in Atom.instances)
        {
            if (a && (Vector3.Distance(a.transform.position, this.lookPoint) < a.radius))
            {
                Status.hitAtom = true;
                Status.hitTransform = a.transform;
            }
        }
        this.aimer.position = this.lookPoint;
        this.aimer.localScale = Vector3.one * (Status.zoomed ? this.inAimerSize : this.outAimerSize);
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!Status.hitAtom)
            {
                Status.SpawnBalloon("Scanner results:\n\nNothing selected.", new Color(0.5f, 0.7f, 0.5f, 1), 4.5f, this.aimer.position, null, "scanner");
                this.scanMe = null;
                return;
            }
            this.scanMe = (Atom) Status.hitTransform.GetComponent(typeof(Atom));
        }
        if (this.scanMe)
        {
            string displayString = (((((("Scanner results:\n\nProtons: " + this.scanMe.protons) + "\nElectrons: ") + this.scanMe.electrons) + "\nNeutrons: ") + this.scanMe.neutrons) + "\n\nCharge: ") + Atom.GetNumString(this.scanMe.charge);
            Status.SpawnBalloon(displayString, new Color(0.5f, 0.7f, 0.5f, 1), 100, this.scanMe.transform.position, this.scanMe.transform, "scanner");
        }
        this.timer = this.timer + Time.deltaTime;
    }

    public virtual void LateUpdate()
    {
         //Screen.showCursor = !aimer.gameObject.active;	
        this.aimer.transform.rotation = Quaternion.identity;
    }

    public virtual void Fire()
    {
        GameObject newProjectileObject = null;
        Projectile newProjectile = null;
        if (!Status.zoomed)
        {
            newProjectileObject = (GameObject)UnityEngine.Object.Instantiate(this.projectile, this.gun.transform.position, this.gun.transform.rotation);
            newProjectile = (Projectile) newProjectileObject.GetComponent(typeof(Projectile));
            newProjectile.energy = this.selectedEnergy;
            newProjectile.Init(this.selectedProjectile);
        }
        else
        {
            Vector3 pos = Status.hitTransform.position + ((this.gun.transform.position - Status.hitTransform.position).normalized * this.zoomedSpawnDistance);
            Quaternion to = Quaternion.LookRotation(this.lookPoint - pos);
            newProjectileObject = (GameObject)UnityEngine.Object.Instantiate(this.projectile, pos, to);
            newProjectile = (Projectile) newProjectileObject.GetComponent(typeof(Projectile));
            newProjectile.energy = this.selectedEnergy;
            newProjectile.zoomed = Status.zoomed;
            newProjectile.Init(this.selectedProjectile);
        }
    }

    public Shooter()
    {
        this.outAimerSize = 5;
        this.inAimerSize = 0.1f;
        this.zoomedSpawnDistance = 2f;
        this.selectedEnergy = 3000f;
    }

}