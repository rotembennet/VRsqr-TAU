using UnityEngine;
using System.Collections;
using VRsqrCore;

public class FlowSwitch : MonoBehaviour {

    public string startEventIn = "";
    public string endEventIn = "";
    public string exitEventOut = "";

	// Use this for initialization
	void Awake () {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false); // !child.gameObject.activeSelf);
        }
        VRsqr_EventsManager.StartListening(endEventIn, EndFunction);

        if (startEventIn != "")
        {
            VRsqr_EventsManager.StartListening(startEventIn, InitFunction);
        }
        else
        {
            Init();
        }

    }

    // Update is called once per frame
    void Update () {
    }

    void Init()
    {
        ////Debug.Log("InitFunction was called!");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true); // !child.gameObject.activeSelf);
        }

    }

    void InitFunction(int EventContext)
    {
        //UnityEventData eventData = VRsqr_EventsManager.GetEventData(EventContext);
        Init();
    }

    void EndFunction(int EventContext)
    {
        //Debug.Log("EndFunction was called! triggering event:" + exitEventOut);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false); // !child.gameObject.activeSelf);
        }
        VRsqr_EventsManager.TriggerEvent(exitEventOut);
    }
}
