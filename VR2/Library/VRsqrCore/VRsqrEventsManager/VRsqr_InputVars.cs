#define VERBOSE

using System;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using System.Collections.Generic;
using VRsqrUtil;

namespace VRsqrCore
{
    public class EventParamsField
    {
        public FieldInfo fiEventParamObj;
        [SerializeField]
        private string _paramName;
        public string paramName
        {
            set
            {
                _paramName = VRsqr_EventsManager.StandardizeString(value);
                //VRsqrUtil.Debug.LogInfo(">>>>>>>>>>> EventParamsField - SET: _paramName = " + _paramName);
            }
            get
            {
                return _paramName;
            }
        }
        [SerializeField]
        public Type paramType;
    }

    public class InOutEvent
    {
        protected bool initialized;

        [Tooltip("Full Name to use is ObjectName.EventName. Default EventName (if left empty) is the variable name (above).")]
        [Header("Event")]
        [SerializeField]
        private string _eventName;
        public string eventName
        {
            set
            {
                _eventName = VRsqr_EventsManager.StandardizeString(value);
                //VRsqrUtil.Debug.LogInfo(">>>>>>>>>>> InEvent - SET: _eventName = " + _eventName);
            }
            get
            {
                return _eventName;
            }
        }

        protected EventParam[] eventParams;

        [SerializeField]
        protected List<EventParamsField> eventParamFields = new List<EventParamsField>();

        public void Init(GameObject gameObj)
        {
            if (this.eventName == "")
            {
                this.eventName = extractEventName(gameObj); //eventName;
            }
            parseEventParamsInfo();
        }


        public string extractScriptName()
        {
            string scriptName = this.GetType().ToString().Split("+"[0])[0];
            VRsqrUtil.Debug.LogInfo("StartListening: scriptName = " + scriptName);

            return scriptName;
        }

        public string extractEventName(GameObject gameObj)
        {
            string scriptName = extractScriptName();

            VRsqrUtil.Debug.LogInfo("extractEventName: listeningObj.name = " + gameObj.name);
            string gameObjName = gameObj.name;

            Component script = gameObj.GetComponent(scriptName);
            FieldInfo[] scriptFields = script.GetType().GetFields();

            string eventName = gameObjName;
            foreach (FieldInfo field in scriptFields)
            {
                VRsqrUtil.Debug.LogInfo("extractEventName: field = " + field.Name);
                if (System.Object.ReferenceEquals(field.GetValue(script), this))
                {
                    VRsqrUtil.Debug.LogInfo("=== extractEventName: field.GetValue(script) = " + field.GetValue(script));
                    eventName += "." + field.Name;
                }
            }

            return eventName;
        }

        public void parseEventParamsInfo()
        {
            eventParamFields = new List<EventParamsField>();
            FieldInfo[] eventParamFieldInfo = this.GetType().GetFields();
            foreach (FieldInfo pfi in eventParamFieldInfo)
            {
                Type paramType = pfi.FieldType;
                if (pfi.FieldType != typeof(EventParam)) // TEMP
                {
                    string varName = pfi.Name;
                    VRsqrUtil.Debug.Log(LogLevel.Debug, "OOOOOOOOOOOOO parseEventParamsInfo: varName = " + varName + " , paramType = " + paramType);

                    this.eventParamFields.Add(new EventParamsField { fiEventParamObj = pfi, paramName = varName, paramType = paramType });
                }
            }
        }

        protected void buildEventParamArray()
        {
            int eventParamInd = 0;
            VRsqrUtil.Debug.LogInfo("OOOOOOOOOOOOO  buildEventOutputParams: eventParamFields.Count = " + eventParamFields.Count);
            foreach (EventParamsField epf in this.eventParamFields)
            {
                VRsqrUtil.Debug.LogInfo("buildEventOutputParams:  epf.paramName = " + epf.paramName + "  , epf.paramType = " + epf.paramType);
                object paramObjVal = epf.fiEventParamObj.GetValue(this);

                string valString = "";
                if (paramObjVal != null)
                {
                    valString = paramObjVal.ToString();
                }
                VRsqrUtil.Debug.Log(LogLevel.Debug, "buildEventOutputParams: paramObjVal = " + paramObjVal + " , valString = " + valString);
                eventParams[eventParamInd++] = new EventParam
                {
                    NameString = epf.paramName, //TypeString = epf.paramType.ToString().ToLower(),
                    paramType = epf.paramType,
                    paramObjVal = paramObjVal,
                    ValString = valString
                };
                //ValString = Convert.ChangeType(paramObjVal, epf.paramType).ToString() };
            }
        }
    }


    [Serializable]
    public class OutEvent : InOutEvent
    {
        [SerializeField]
        public bool persistent;

        public void sendData(GameObject sendingObj)
        {
            if (initialized == false)
            {
                Init(sendingObj);
                initialized = true;
            }

            eventParams = new EventParam[eventParamFields.Count];
            VRsqrUtil.Debug.Log(LogLevel.Debug, "TTTTTTTTTTTTTTT  sendData: eventParamFields.Count = " + eventParamFields.Count);

            buildEventParamArray();

            VRsqrUtil.Debug.Log(LogLevel.Debug, "TTTTTTTTTTTTTTT  sendData: TriggerDataEvent - eventParams.Length = " + eventParams.Length);
            VRsqr_EventsManager.TriggerDataEvent(this.eventName, this.eventParams, this.persistent);
        }
    }


    [Serializable]
    public class InEvent : InOutEvent
    {
        private UnityAction<int> basicEventHandler;
        private UnityAction<int> customEventHandler;

        public void StartListening(GameObject listeningObj, UnityAction<int> customEventHandler = null)
        {
            VRsqrUtil.Debug.LogInfo("StartListening");

            if (initialized == false)
            {
                Init(listeningObj);
                eventParams = new EventParam[eventParamFields.Count];

                initialized = true;
            }

            this.basicEventHandler = eventHandlerFunc;

            VRsqr_EventsManager.StartListening(this.eventName, this.basicEventHandler);
            if (customEventHandler != null)
            {
                this.customEventHandler = customEventHandler;
                VRsqr_EventsManager.StartListening(this.eventName, this.customEventHandler);
            }
        }

        private void eventHandlerFunc(int EventContext)
        {
            //eventInTimeStamp = VRsqr_EventsManager.getEventInTime(EventContext);
            //eventOutTimeStamp = VRsqr_EventsManager.getEventOutTime(EventContext);

            VRsqrUtil.Debug.LogInfo("eventHandlerFunc: EventContext = " + EventContext);
            int eventParamInd = 0;
            // use reflection mechanisms to collect the EventParam fields, as added in the definition of the current sub-class, which is inheriting EventIn
            foreach (EventParamsField epf in this.eventParamFields)
            {
                VRsqrUtil.Debug.Log(LogLevel.Debug, "eventHandlerFunc:  epf.paramName = " + epf.paramName + "  , epf.paramType = " + epf.paramType);
                EventParam ep = VRsqr_EventsManager.GetEventParam(EventContext, epf.paramName);
                if (ep != null) // data was set in this event for this param
                {
                    VRsqrUtil.Debug.Log(LogLevel.Debug, "eventHandlerFunc: ep.paramType = " + ep.paramType + " , ep.paramObjVal = " + ep.paramObjVal);
                    if (ep.paramObjVal != null) //  ep.Val() == null)
                    {
                        epf.fiEventParamObj.SetValue(this, ep.paramObjVal); // TODO: save the setter function one time, at init
                    }
                    else
                    {
                        if (ep.paramType == null)
                        {
                            ep.paramType = epf.paramType; // interpret the received parameter based on the receiving-end expected input
                        }
                        VRsqrUtil.Debug.Log(LogLevel.Debug, "eventHandlerFunc: ep.StringToObjVal() = " + ep.StringToObjVal());
                        epf.fiEventParamObj.SetValue(this, ep.StringToObjVal()); // TODO: make this mode, of string-based setting, deprecate
                    }
                    eventParams[eventParamInd++] = ep;
                }
            }
        }

        public EventParam[] getEventParams()
        {
            return eventParams;
        }
    }
}
