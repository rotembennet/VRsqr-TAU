#define VERBOSE

using UnityEngine;
using System.Collections;
using VRsqrCore;
using VRsqrUtil;
using UnityEngine.UI;
using System;

public class VRsqr_Timer : MonoBehaviour {

    //private float startTime;
    [Serializable]
    public class InEventVars : InEvent
    {
        [Tooltip("Input variable values get updated automatically when inputs are intercepted")]
        [Header("Input Variables")]
        public int TimerDuration;
    }
    public InEventVars TimerInputs;


    //[Serializable]
    //public class InputVariables : InEvent
    //{
    //    [Serializable]
    //    public class Variables
    //    {
    //        //public EventParam DurationMS;
    //        [SerializeField]
    //        public int TimerDuration;
    //        [SerializeField]
    //        public Vector3 testPos;
    //    }
    //    Variables variables;
    //}
    //public InputVariables inputVariables;

    //private int timerDurationMS;
    public string timerEndEventOut = "";
 
    //public void OnEnable()
    public void Awake()
    {
        //TimerInputs.StartListening(startTimer);
        TimerInputs.StartListening(gameObject, startTimerHandler); //this.name, startTimer);
    }

    private void OnValidate()
    {
        //TimerStartEventIn.Init(startTimer);
    }

    private void startTimerHandler(int eventContext)
    {
        VRsqrUtil.Debug.Log(LogLevel.Debug, "startTimerHandler: eventContext = " + eventContext);
        //if (TimerInputs.DurationMS.Val(ref timerDurationMS))
        {
            StartCoroutine("TimerFunc");
        }
    }

    private IEnumerator TimerFunc()
    {
        //while (true)
        {
            VRsqrUtil.Debug.Log(LogLevel.Debug, "TimerFunc: TimerInputs.TimerDuration = " + TimerInputs.TimerDuration);
            yield return new WaitForSeconds(TimerInputs.TimerDuration / 1000.0f); //timerDurationMS / 1000.0f);
            VRsqrUtil.Debug.Log(LogLevel.Debug, "TimerFunc: Timer ended - triggering timerEndEventOut = " + timerEndEventOut);
            VRsqr_EventsManager.TriggerEvent(timerEndEventOut);
        }
    }
}
