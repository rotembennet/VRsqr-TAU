using UnityEngine;
using System.Collections;
using VRsqrCore;
//using System.Windows.Forms;
using System.Drawing;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class MouseMotionPlugin : MonoBehaviour {

    public Vector2 mousePosition;
    public string mouseMotionEvent = "";
    public string mouseLeftKeyEvent = "";

    private float lastUpdateTime = 0;
    private float lastMouseUpdateTime = 0;
    private Vector3 lastUpdateMousePos;
    public float FixedUpdateFPS = 0;
    public float FixedUpdateMouseFPS = 0;

    private float lastFixedUpdateTime = 0;
    private float lastMouseFixedUpdateTime = 0;
    private Vector3 lastFixedUpdateMousePos;
    public float UpdateFPS = 0;
    public float UpdateMouseFPS = 0;

    //private float lastGuiTime = 0;
    //private float lastMouseGuiTime = 0;
    //private Vector3 lastGuiMousePos;
    //public float GuiFPS = 0;
    //public float GuiMouseFPS = 0;

    private float lastWinTime = 0;
    private float lastMouseWinTime = 0;
    public float WinFPS = 0;
    public float WinMouseFPS = 0;
    public Vector3 lastWinMousePos;

    public Text debugText;

    //public InEvent testEvent;

    // Use this for initialization
    void Start () {
        lastUpdateTime = Time.realtimeSinceStartup;
        lastFixedUpdateTime = Time.realtimeSinceStartup;
        lastWinTime = Time.realtimeSinceStartup;

        //testEvent.StartListening(gameObject, dataHandler);

        UnityEngine.Application.targetFrameRate = 10;
        QualitySettings.vSyncCount = 1;
    }

    void dataHandler(int context)
    {
        debugText.text = "Got Event";
    }
	
	// Update is called once per frame
	void Update () {
        updateTimeMeasurements(ref UpdateFPS, ref lastUpdateTime, ref UpdateMouseFPS, ref lastMouseUpdateTime, ref lastUpdateMousePos, Input.mousePosition);

        /*
        UpdateFPS = Mathf.Lerp(UpdateFPS, 1f / (Time.realtimeSinceStartup - lastUpdateTime), 0.01f);
        if (lastUpdateMousePos != Input.mousePosition)
        {
            UpdateMouseFPS = 1f / (Time.realtimeSinceStartup - lastMouseUpdateTime);
            lastUpdateMousePos = Input.mousePosition;
            lastMouseUpdateTime = Time.realtimeSinceStartup;
        }
        else
        {
            UpdateMouseFPS = 0;
        }
        lastUpdateTime = Time.realtimeSinceStartup;
        */

        //mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //VRsqr_EventsManager.TriggerEvent(mouseMotionEvent, mousePosition.x, mousePosition.y);

        //if (Input.GetMouseButtonDown(0))
        //{
        //    VRsqr_EventsManager.TriggerEvent(mouseLeftKeyEvent);
        //}
    }

    public Point mousePos;
    void FixedUpdate()
    {
        updateTimeMeasurements(ref FixedUpdateFPS, ref lastFixedUpdateTime, ref FixedUpdateMouseFPS, ref lastMouseFixedUpdateTime, ref lastFixedUpdateMousePos, Input.mousePosition);

        mousePos = GetCursorPosition();
        Vector3 currWinMousePos = new Vector3(mousePos.X, mousePos.Y);
        updateTimeMeasurements(ref WinFPS, ref lastWinTime, ref WinMouseFPS, ref lastMouseWinTime, ref lastWinMousePos, currWinMousePos);

        //mousePos = GetCursorPosition();
        //lastWinMousePos = new Vector3(mousePos.X, mousePos.Y);
        //updateTimeMeasurements(ref WinFPS, ref lastWinTime, ref WinMouseFPS, ref lastMouseWinTime, ref lastWinMousePos);

        /*
        FixedUpdateFPS = Mathf.Lerp(FixedUpdateFPS, 1f / (Time.realtimeSinceStartup - lastFixedUpdateTime), 0.01f);
        if (lastFixedUpdateMousePos != Input.mousePosition)
        {
            FixedUpdateMouseFPS = 1f / (Time.realtimeSinceStartup - lastMouseFixedUpdateTime);
            lastFixedUpdateMousePos = Input.mousePosition;
            lastMouseFixedUpdateTime = Time.realtimeSinceStartup;
        }
        else
        {
            FixedUpdateMouseFPS = 0;
        }

        lastFixedUpdateTime = Time.realtimeSinceStartup;
        */
    }

    //void OnGUI()
    //{
    //    //updateTimeMeasurements(ref GuiFPS, ref lastGuiTime, ref GuiMouseFPS, ref lastMouseGuiTime, ref lastGuiMousePos, Input.mousePosition);

    //    //mousePos = GetCursorPosition();
    //    //Vector3 currWinMousePos = new Vector3(mousePos.X, mousePos.Y);
    //    //updateTimeMeasurements(ref WinFPS, ref lastWinTime, ref WinMouseFPS, ref lastMouseWinTime, ref lastWinMousePos, currWinMousePos);
    //}

    //protected void OnMouseMove(MouseEventArgs e)
    //{
    //    debugText.text = "OnMouseMove";
    //    lastWinMousePos = new Vector3(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);
    //}


    void updateTimeMeasurements(ref float UpdateFPS, ref float lastUpdateTime, ref float UpdateMouseFPS, ref float lastMouseUpdateTime, ref Vector3 lastUpdateMousePos, Vector3 currMousePos)
    {
        UpdateFPS = Mathf.Lerp(UpdateFPS, 1f / (Time.realtimeSinceStartup - lastUpdateTime), 0.01f);
        if (lastUpdateMousePos != currMousePos)
        {
            UpdateMouseFPS = Mathf.Lerp(UpdateMouseFPS, 1f / (Time.realtimeSinceStartup - lastMouseUpdateTime), 0.01f);
            lastUpdateMousePos = currMousePos;
            lastMouseUpdateTime = Time.realtimeSinceStartup;
        }
        else
        {
            //UpdateMouseFPS = 0;
        }
        lastUpdateTime = Time.realtimeSinceStartup;
    }



    /// <summary>
    /// Struct representing a point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    public static Point GetCursorPosition()
    {
        POINT lpPoint;
        GetCursorPos(out lpPoint);

        return lpPoint;
    }

}
