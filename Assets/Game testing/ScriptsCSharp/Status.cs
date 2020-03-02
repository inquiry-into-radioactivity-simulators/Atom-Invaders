using UnityEngine;
using System.Collections;

[System.Serializable]
public class WordBalloon : object
{
    public string text;
    public Color color;
    public Transform transform;
    public Vector3 localPos;
    public float fadeTime;
    public string tag;
    public WordBalloon()
    {
        this.text = string.Empty;
    }

}
[System.Serializable]
public partial class Status : MonoBehaviour
{
    public static bool hitAtom;
    public static Transform hitTransform;
    public static bool zoomed;
    public static bool zooming;
    public static bool scanner;
    public static bool atomMode;
    public static float zoomAmt;
    public float zoomSpeed;
    public float zoomInHeight;
    public float inNear;
    public int inFar;
    public float outNear;
    public float outFar;
    public GUISkin skin;
    public int balloonHeight;
    public int lineHeight;
    private Vector3 originalPos;
    public static Color eColor;
    public static Color pColor;
    public static Color nColor;
    public static ArrayList balloons;
    private ArrayList offsets;
    private ArrayList styles;
    public virtual void Start()
    {
        this.originalPos = Camera.main.transform.position;
        Status.hitAtom = false;
        Status.zoomed = false;
        Status.zooming = false;
        this.offsets = new ArrayList();
        this.offsets.Add(new Vector2(-1, 0));
        this.offsets.Add(new Vector2(-1, -1));
        this.offsets.Add(new Vector2(0, 0));
        this.offsets.Add(new Vector2(0, -1));
        this.styles = new ArrayList();
        this.styles.Add(this.skin.GetStyle("label_downleft"));
        this.styles.Add(this.skin.GetStyle("label_upleft"));
        this.styles.Add(this.skin.GetStyle("label_downright"));
        this.styles.Add(this.skin.GetStyle("label"));
    }

    public virtual void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            Status.atomMode = false;
            ((AtomSpawner) UnityEngine.Object.FindObjectOfType(typeof(AtomSpawner))).timer = 9001;
        }
        if (Input.GetKeyDown("a"))
        {
            Status.atomMode = true;
            ((AtomSpawner) UnityEngine.Object.FindObjectOfType(typeof(AtomSpawner))).timer = 9001;
        }
        if ((Input.GetKeyDown("z") && (Status.hitAtom || Status.zoomed)) && !Status.zooming)
        {
             // the next line is from something that we used to do here. If I remove it, it causes a compiler cycle error in other scripts
            UnityEngine.Object.FindObjectOfType(typeof(Shooter));
            this.ToggleZoom();
        }
    }

    public static void StaticToggleZoom()
    {
        ((Status) UnityEngine.Object.FindObjectOfType(typeof(Status))).ToggleZoom();
    }

    public virtual void ToggleZoom()
    {
        if (Status.zoomed)
        {
            this.StartCoroutine(this.Zoom(null, this.originalPos));
            Atom a = (Atom) Status.hitTransform.GetComponent(typeof(Atom));
            a.SetZoomed(false);
        }
        else
        {
            this.StartCoroutine(this.Zoom(Status.hitTransform, new Vector3(0, this.zoomInHeight, 0)));
            Atom b = (Atom) Status.hitTransform.GetComponent(typeof(Atom));
            b.SetZoomed(true);
        }
    }

    public virtual IEnumerator Zoom(Transform trans, Vector3 target)
    {
        float time = 0f;
        float lerp = 0f;
        Vector3 previousPos = Camera.main.transform.position;
        float previousNear = Camera.main.nearClipPlane;
        float previousFar = Camera.main.farClipPlane;
        float near = 0f;
        if (Status.zoomed)
        {
            near = this.outNear;
        }
        else
        {
            near = this.inNear;
        }
        float far = 0f;
        if (Status.zoomed)
        {
            far = this.outFar;
        }
        else
        {
            far = this.inFar;
        }
        Status.zooming = true;
        while (time < 1f)
        {
            time = time + (Time.deltaTime * this.zoomSpeed);
            if (Status.zoomed)
            {
                lerp = ((time * time) * time) * time;
            }
            else
            {
                lerp = 1 - ((((1 - time) * (1 - time)) * (1 - time)) * (1 - time));
            }
            Camera.main.transform.position = Vector3.Lerp(previousPos, trans != null ? trans.position + target : target, lerp);
            Camera.main.nearClipPlane = Mathf.Lerp(previousNear, near, lerp);
            Camera.main.farClipPlane = Mathf.Lerp(previousFar, far, lerp);
            Status.zoomAmt = Status.zoomed ? 1 - time : time;
            yield return null;
        }
        Camera.main.transform.position = trans != null ? trans.position + target : target;
        Camera.main.nearClipPlane = near;
        Camera.main.farClipPlane = far;
        Status.zoomAmt = Status.zoomed ? 0 : 1;
        Status.zoomed = !Status.zoomed;
        Status.zooming = false;
        if (Status.zoomed && (trans != null))
        {
            Atom theAtom = (Atom) trans.GetComponent(typeof(Atom));
            if (!theAtom.connected)
            {
                theAtom.mov.enabled = false;
            }
            else
            {
                theAtom.connected.doing = false;
                theAtom.connected.myRot.enabled = false;
                theAtom.connected.myMov.enabled = false;

                {
                    int _12 = 0;
                    Vector3 _13 = trans.position;
                    _13.y = _12;
                    trans.position = _13;
                }
            }
            while (!Status.zooming)
            {
                Camera.main.transform.position = trans.position + target;
                yield return null;
            }
            if (!theAtom.connected)
            {
                theAtom.mov.enabled = true;
            }
            else
            {
                theAtom.connected.myRot.enabled = true;
                theAtom.connected.myMov.enabled = true;
                theAtom.connected.doing = true;
            }
        }
    }

    public static void SpawnBalloon(string text, Color color, float life, Vector3 pos, Transform trans, string tag)
    {
        Status.RemoveBalloonWithTag(tag);
        WordBalloon wordy = new WordBalloon();
        wordy.text = text;
        wordy.color = color;
        wordy.fadeTime = Time.time + life;
        wordy.transform = trans;
        wordy.localPos = trans != null ? trans.InverseTransformPoint(pos) : pos;
        wordy.tag = tag;
        Status.balloons.Add(wordy);
    }

    public static void RemoveBalloonWithTag(string tag)
    {
        int @remove = -1;
        int i = 0;
        foreach (WordBalloon b in Status.balloons)
        {
            if (b.tag == tag)
            {
                @remove = i;
            }
            i++;
        }
        if (@remove != -1)
        {
            Status.balloons.RemoveAt(@remove);
        }
    }

    public virtual void OnGUI()
    {
        GUI.skin = this.skin;
        int @remove = -1;
        int i = 0;
        foreach (WordBalloon b in Status.balloons)
        {
            if (Time.time > (b.fadeTime + 1))
            {
                @remove = i;
            }
            else
            {
                int lines = 0;
                int index = b.text.IndexOf("\n", 0);
                while ((index != -1) && ((index + 1) < b.text.Length))
                {
                    index = b.text.IndexOf("\n", index + 1);
                    lines++;
                }
                Vector3 spoint = Camera.main.WorldToScreenPoint(b.transform != null ? b.transform.TransformPoint(b.localPos) : b.localPos);
                int index1 = spoint.x > (Screen.width * 0.5f) ? (spoint.y > (Screen.height * 0.5f) ? 0 : 1) : (spoint.y > (Screen.height * 0.5f) ? 2 : 3);
                spoint.y = Screen.height - spoint.y;
                GUIStyle theStyle = (GUIStyle)this.styles[index1];
                Vector2 size = theStyle.CalcSize(new GUIContent(b.text));
                Color oColor = GUI.color;
                if (Time.time > b.fadeTime)
                {
                    GUI.color = Color.Lerp(b.color, new Color(b.color.r, b.color.g, b.color.b, 0), Time.time - b.fadeTime);
                }
                else
                {
                    GUI.color = b.color;
                }
                Vector2 theOffset = (Vector2)this.offsets[index1];
                Vector2 buh = new Vector2((theOffset.x + 0.5f) * 30, (theOffset.y + 0.5f) * 50);
                GUILayout.BeginArea(new Rect((spoint.x + (size.x * theOffset.x)) + buh.x, (spoint.y + (size.y * theOffset.y)) + buh.y, size.x, size.y));
                GUILayout.Label(b.text, theStyle, new GUILayoutOption[] {});
                GUILayout.EndArea();
                GUI.color = oColor;
            }
            i++;
        }
        if (@remove != -1)
        {
            Status.balloons.RemoveAt(@remove);
        }
    }

    public Status()
    {
        this.zoomInHeight = 6f;
        this.inNear = 0.01f;
        this.inFar = 100;
        this.outNear = 500f;
        this.outFar = 2000f;
        this.balloonHeight = 30;
        this.lineHeight = 13;
    }

    static Status()
    {
        Status.atomMode = true;
        Status.eColor = new Color(0.1f, 1, 0.75f, 1f);
        Status.pColor = new Color(1, 0.1f, 0.08f, 1f);
        Status.nColor = new Color(0.45f, 0.3f, 0.9f, 1f);
        Status.balloons = new ArrayList();
    }

}