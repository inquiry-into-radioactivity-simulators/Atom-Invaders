using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class TheGUI : MonoBehaviour
{
    public GUISkin skin;
    public Rect screenRect;
    public int margin;
    public float menuTime;
    public Shooter shooter;
    public string[] projectileNames;
    public string[] projectileNames2;
    public int[] evOption;
    public string[] evOptionName;
    private Rect evCheck;
    public bool evMenu;
    private float evMenuTime;
    private Rect projCheck;
    public bool projMenu;
    private float projMenuTime;
    private int mouseOver;
    private bool killMenu;
    public static bool inGUI;
    public virtual void Start() //Screen.SetResolution (640, 480, false);
    {
    }

    public bool mouseOut;
    public bool mouseUp;
    public virtual void Update()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        bool insideEvButton = (((this.evCheck.y < mousePos.y) && ((this.evCheck.y + this.evCheck.height) > mousePos.y)) && (this.evCheck.x < mousePos.x)) && ((this.evCheck.x + this.evCheck.width) > mousePos.x);
        bool insidePrjButton = (((this.projCheck.y < mousePos.y) && ((this.projCheck.y + this.projCheck.height) > mousePos.y)) && (this.projCheck.x < mousePos.x)) && ((this.projCheck.x + this.projCheck.width) > mousePos.x);
        if ((Input.GetMouseButtonDown(0) && !this.projMenu) && !this.evMenu)
        {
            if (insideEvButton)
            {
                this.evMenu = true;
                this.mouseOut = false;
                this.mouseUp = false;
                this.evMenuTime = Time.time;
            }
            if (insidePrjButton)
            {
                this.projMenu = true;
                this.mouseOut = false;
                this.mouseUp = false;
                this.projMenuTime = Time.time;
            }
        }
        Rect areaRect = new Rect(this.screenRect.x, Screen.height - this.screenRect.y, Screen.width, this.screenRect.height);
        Rect evRect = this.evCheck;
        Rect projRect = this.projCheck;
        float h = evRect.height;
        evRect.y = evRect.y - (evRect.height * (this.evOption.Length + 1));
        evRect.height = evRect.height + (evRect.height * (this.evOption.Length + 1));
        projRect.y = projRect.y - (projRect.height * (this.projectileNames.Length + 1));
        projRect.height = projRect.height + (projRect.height * (this.projectileNames.Length + 1));
        if (this.evMenu && !((((evRect.y < mousePos.y) && ((evRect.y + evRect.height) > mousePos.y)) && (evRect.x < mousePos.x)) && ((evRect.x + evRect.width) > mousePos.x)))
        {
            this.mouseOut = true;
        }
        if (this.projMenu && !((((projRect.y < mousePos.y) && ((projRect.y + projRect.height) > mousePos.y)) && (projRect.x < mousePos.x)) && ((projRect.x + projRect.width) > mousePos.x)))
        {
            this.mouseOut = true;
        }
        if (this.evMenu || this.projMenu)
        {
            if (((this.mouseUp && this.mouseOut) || (this.mouseUp && Input.GetMouseButtonDown(0))) || ((!(insideEvButton || insidePrjButton) && !this.mouseUp) && Input.GetMouseButtonUp(0)))
            {
                this.killMenu = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                this.mouseUp = true;
            }
            if ((this.killMenu && this.evMenu) && (Time.time > (this.evMenuTime + this.menuTime)))
            {
                this.killMenu = false;
                this.evMenu = false;
                if (this.mouseOver != -1)
                {
                    this.shooter.selectedEnergy = this.evOption[this.mouseOver];
                }
            }
            if ((this.killMenu && this.projMenu) && (Time.time > (this.projMenuTime + this.menuTime)))
            {
                this.killMenu = false;
                this.projMenu = false;
                if (this.mouseOver != -1)
                {
                    this.shooter.selectedProjectile = this.mouseOver;
                }
            }
        }
        this.shooter.inMenu = this.evMenu || this.projMenu;
    }

    public virtual void OnGUI()
    {
        //Debug.Log("ONGUI1");
        GUI.color = new Color(1, 1, 1, 0.3f);
        GUILayout.Label((("FPS:" + Mathf.Round(1f / Time.smoothDeltaTime)) + " Objects: ") + ((GameObject[]) UnityEngine.Object.FindObjectsOfType(typeof(GameObject))).Length, new GUILayoutOption[] {});
        GUI.skin = this.skin;
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        Rect areaRect = new Rect(this.screenRect.x, Screen.height - this.screenRect.y, Screen.width, this.screenRect.height);
        TheGUI.inGUI = areaRect.y < mousePos.y;
        GUI.Box(areaRect, "");
        GUI.BeginGroup(areaRect);
        if (!Status.scanner)
        {
            GUI.color = Status.eColor;
        }
        else
        {
            GUI.color = GUI.color = new Color(Status.eColor.r, Status.eColor.g, Status.eColor.b, 0.5f);
        }
        Rect evRect = new Rect(this.margin, this.margin, (areaRect.width * 0.5f) - (this.margin * 2), (areaRect.height * 0.5f) - (this.margin * 2));
        this.evCheck = evRect;
        this.evCheck.y = this.evCheck.y + areaRect.y;
        if (!Status.scanner)
        {
            GUI.Button(evRect, ("Energy: " + this.shooter.selectedEnergy) + "ev");
        }
        else
        {
            GUI.Label(evRect, ("Energy: " + this.shooter.selectedEnergy) + "ev", this.skin.button);
        }
        Rect projRect = new Rect((areaRect.width * 0.5f) + this.margin, this.margin, (areaRect.width * 0.5f) - (this.margin * 2), (areaRect.height * 0.5f) - (this.margin * 2));
        this.projCheck = projRect;
        this.projCheck.y = this.projCheck.y + areaRect.y;
        string projName = this.shooter.selectedEnergy < 2000 ? this.projectileNames[this.shooter.selectedProjectile] : this.projectileNames2[this.shooter.selectedProjectile];
        if (!Status.scanner)
        {
            GUI.Button(projRect, "Projectile: " + projName);
        }
        else
        {
            GUI.Label(projRect, "Projectile: " + projName, this.skin.button);
        }
        Color c = GUI.color;
        GUI.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        Rect scrutRect = new Rect(this.margin, (areaRect.height * 0.5f) + this.margin, (areaRect.width * 0.33f) - (this.margin * 2), (areaRect.height * 0.5f) - (this.margin * 2));
        GUI.Label(scrutRect, "", this.skin.button);
        GUI.color = c;
        Rect moleRect = new Rect((areaRect.width * 0.33f) + this.margin, (areaRect.height * 0.5f) + this.margin, (areaRect.width * 0.33f) - (this.margin * 2), (areaRect.height * 0.5f) - (this.margin * 2));
        string stringe = "Switch to " + (Status.atomMode == false ? "Atom" : "Molecule");
        if (GUI.Button(moleRect, stringe))
        {
            Status.atomMode = !Status.atomMode;
        }
        GUI.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        Rect otherRect = new Rect((areaRect.width * 0.66f) + (this.margin * 2), (areaRect.height * 0.5f) + this.margin, (areaRect.width * 0.33f) - (this.margin * 3), (areaRect.height * 0.5f) - (this.margin * 2));
        GUI.Box(otherRect, "Back to menu", new GUIStyle("button"));
        GUI.EndGroup();
        GUI.color = c;
        mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        this.mouseOver = -1;
        int i = 0;
        if (this.evMenu)
        {
            evRect.y = evRect.y + areaRect.y;
            evRect.x = evRect.x + (this.margin * 4);
            evRect.width = evRect.width - (this.margin * 8);
            while (i < this.evOption.Length)
            {
                evRect.y = evRect.y - evRect.height;
                if ((((evRect.y < mousePos.y) && ((evRect.y + evRect.height) > mousePos.y)) && (evRect.x < mousePos.x)) && ((evRect.x + evRect.width) > mousePos.x))
                {
                    GUI.color = new Color(1, 1, 1, 1);
                    this.mouseOver = i;
                }
                else
                {
                    GUI.color = new Color(1, 1, 1, 0.5f);
                }
                GUI.Button(evRect, this.evOptionName[i] + "ev");
                i++;
            }
        }
        if (this.projMenu)
        {
            projRect.y = projRect.y + areaRect.y;
            projRect.x = projRect.x + (this.margin * 4);
            projRect.width = projRect.width - (this.margin * 8);
            i = 0;
            while (i < this.projectileNames.Length)
            {
                projRect.y = projRect.y - projRect.height;
                if ((((projRect.y < mousePos.y) && ((projRect.y + projRect.height) > mousePos.y)) && (projRect.x < mousePos.x)) && ((projRect.x + projRect.width) > mousePos.x))
                {
                    GUI.color = new Color(1, 1, 1, 1);
                    this.mouseOver = i;
                }
                else
                {
                    GUI.color = new Color(1, 1, 1, 0.5f);
                }
                string projName1 = this.shooter.selectedEnergy < 2000 ? this.projectileNames[i] : this.projectileNames2[i];
                GUI.Button(projRect, projName1);
                i++;
            }
        }
    }

    public TheGUI()
    {
        this.margin = 4;
        this.menuTime = 0.5f;
    }

}