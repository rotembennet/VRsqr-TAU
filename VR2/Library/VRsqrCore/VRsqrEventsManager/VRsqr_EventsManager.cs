//#define VERBOSE
//#define VERBOSE_EXTRA

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Reflection;
using VRsqrUtil;

namespace VRsqrCore
{
    [System.Serializable]
    public class UnityEventInt : UnityEvent<int> // any event includes a single EventContext int identifier
    {
    }


    [Serializable]
    public class EventOut
    {
        //public string eventName;
        [SerializeField]
        private string _eventName;
        public string eventName
        {
            set
            {
                _eventName = VRsqr_EventsManager.StandardizeString(value);
            }
            get
            {
                return _eventName;
            }
        }
        public EventParam[] eventParams;

        public void trigger()
        {
            eventName = VRsqr_EventsManager.StandardizeString(eventName);
            VRsqr_EventsManager.TriggerDataEvent(eventName, eventParams);
            VRsqrUtil.Debug.LogInfo("------------ EventOut - trigger: eventName = " + eventName);
        }
    };


    [Serializable]
    public class EventIn 
    {
        [Tooltip("Input variable value gets updated automatically when inputs are intercepted")]
        [Header("Input Variable")]
        private string _eventName;
        private string eventName
        {
            set
            {
                _eventName = VRsqr_EventsManager.StandardizeString(value);
                //VRsqrUtil.Debug.LogInfo(">>>>>>>>>>> EventIn - SET: _eventName = " + _eventName);
            }
            get
            {
                //VRsqrUtil.Debug.LogInfo("<<<<<<<<<< EventIn - GET NameString: _nameString = " + _eventName);
                return _eventName;
            }
        }

        private UnityAction<int> basicEventHandler;
        private UnityAction<int> customEventHandler;

        private class EventParamsField
        {
            public FieldInfo fiEventParamObj;
            public FieldInfo fiParamName;
        }
        private List<EventParamsField> eventParamFields = new List<EventParamsField>();

        public void eventHandlerFunc(int EventContext)
        {
            VRsqrUtil.Debug.LogInfo("eventHandlerFunc: EventContext = " + EventContext);
            // use reflection mechanisms to collect the EventParam fields, as added in the definition of the current sub-class, which is inheriting EventIn
            foreach (EventParamsField epf in this.eventParamFields)
            {
                string nameFieldVal = (string)epf.fiParamName.GetValue(epf.fiEventParamObj.GetValue(this));
                //VRsqrUtil.Debug.Log(LogLevel.Debug, "eventHandlerFunc: nameFieldVal = " + nameFieldVal);
                EventParam ep = VRsqr_EventsManager.GetEventParam(EventContext, nameFieldVal);
                epf.fiEventParamObj.SetValue(this, ep);
            }
        }

        public void StartListening(UnityAction<int> customEventHandler = null)
        {
            VRsqrUtil.Debug.Log(LogLevel.Warning, "StartListening: customEventHandler = " + customEventHandler);
            this.basicEventHandler = eventHandlerFunc;

            Type tempType = this.GetType();
            VRsqrUtil.Debug.Log(LogLevel.Warning, "StartListening: tempType = " + tempType);
            FieldInfo tempInfo = tempType.GetField("eventName");
            VRsqrUtil.Debug.Log(LogLevel.Warning, "StartListening: tempInfo = " + tempInfo);
            string eventName = (string)this.GetType().GetField("eventName").GetValue(this);
            VRsqrUtil.Debug.Log(LogLevel.Warning, "StartListening: eventName = " + eventName);
            VRsqr_EventsManager.StartListening(eventName, this.basicEventHandler);
            if (customEventHandler != null)
            {
                this.customEventHandler = customEventHandler;
                VRsqr_EventsManager.StartListening(eventName, this.customEventHandler);
            }

            eventParamFields = new List<EventParamsField>();
            FieldInfo[] eventParamFieldInfo = this.GetType().GetFields();
            foreach (FieldInfo pfi in eventParamFieldInfo)
            {
                if (pfi.FieldType == typeof(EventParam))
                {
                    //string nameFieldVal = (string)typeof(EventParam).GetField("NameString").GetValue(pfi.GetValue(eventListenerData));
                    FieldInfo fiParamName = typeof(EventParam).GetField("NameString");
                    this.eventParamFields.Add(new EventIn.EventParamsField { fiEventParamObj = pfi, fiParamName = fiParamName });
                    VRsqrUtil.Debug.Log(LogLevel.Warning, "StartListening: fiParamName.Name = " + fiParamName.Name);
                    //EventParam ep = VRsqr_EventsManager.GetEventParam(EventContext, nameFieldVal);
                    //pfi.SetValue(eventListenerData, ep);
                }
            }
        }
    }



    //public enum EventDataType
    //{
    //    Undefined = 0,
    //    Int,
    //    Float,
    //    Double,
    //    String,
    //    Vector2,
    //    Vector3,
    //    Quaternion
    //};


    [System.Serializable]
    public class EventParam
    {
        [SerializeField]
        private string _nameString;
        public string NameString
        {
            set
            {
                _nameString = VRsqr_EventsManager.StandardizeString(value);
                VRsqrUtil.Debug.LogInfo(">>>>>>>>>>> EventParam - SET NameString: _nameString = " + _nameString);
            }
            get
            {
                VRsqrUtil.Debug.LogInfo("<<<<<<<<<< EventParam - GET NameString: _nameString = " + _nameString);
                return _nameString;
            }
        }

        public string ValString;

        //[SerializeField]
        private string TypeString;
        private Type _paramType;
        public Type paramType
        {
            set
            {
                _paramType = value;
                TypeString = value.ToString();
                VRsqrUtil.Debug.Log(LogLevel.Debug, ">>>>>>>>>>> EventParam - SET paramType: paramType = " + paramType + " , TypeString = " + TypeString);
            }
            get
            {
                return _paramType;
            }
        }

        public object paramObjVal;


        public object StringToObjVal()
        {
            if (this.paramType == null)
                return null;

            switch (this.paramType.ToString().ToLower())
            {
                case "string":
                case "system.string":
                    return ValString;
                case "int":
                case "system.int16":
                case "system.int32":
                case "system.int64":
                    int intRes = 0;
                    if (Val(ref intRes))
                        return intRes;
                    break;
                case "float":
                case "system.single":
                    float floatRes = 0f;
                    if (Val(ref floatRes))
                        return floatRes;
                    break;
                case "double":
                case "system.double":
                    double doubleRes = 0;
                    if (Val(ref doubleRes))
                        return doubleRes;
                    break;
                case "bool":
                case "boolean":
                case "system.boolean":
                    bool boolRes;
                    boolRes = (ValString.ToLower() == "true" ? true : false);
                    return boolRes;
                case "vector3":
                case "unityengine.vector3":
                    Vector3 vec3Res = Vector3.zero;
                    if (Val(ref vec3Res))
                        return vec3Res;
                    break;
                case "quaternion":
                case "unityengine.quaternion":
                    Quaternion quaternRes = Quaternion.identity;
                    if (Val(ref quaternRes))
                        return quaternRes;
                    break;
                default:
                    return null; // paramObjVal;
            }
            return null;
        }

        public bool Val(ref float resVar)
        {
            if (ValString == "")
                return false;

            if (float.TryParse(ValString, out resVar))
            {
                return true;
            }

            return false;
        }

        public bool Val(ref double resVar)
        {
            if (ValString == "")
                return false;
            if (double.TryParse(ValString, out resVar))
            {
                return true;
            }
            return false;
        }

        public bool Val(ref int resVar)
        {
            if (ValString == "")
                return false;

            if (int.TryParse(ValString, out resVar))
            {
                return true;
            }

            return false;
        }

        public bool Val(ref Vector3 resVar)
        {
            if (ValString == "")
                return false;

            string[] sArray = SplitVecVar(ValString);
            if (sArray.Length == 3 &&
                float.TryParse(sArray[0], out resVar.x) &&
                float.TryParse(sArray[1], out resVar.y) &&
                float.TryParse(sArray[2], out resVar.z))
            {
                return true;
            }

            return false;
        }

        public bool Val(ref Quaternion resVar)
        {
            if (ValString == "")
                return false;

            string[] sArray = SplitVecVar(ValString);
            if (sArray.Length == 4 &&
                float.TryParse(sArray[0], out resVar.x) &&
                float.TryParse(sArray[1], out resVar.y) &&
                float.TryParse(sArray[2], out resVar.z) &&
                float.TryParse(sArray[2], out resVar.w))
            {
                return true;
            }

            return false;
        }

        public static string[] SplitVecVar(string sVec)
        {
            if (sVec.StartsWith("(") && sVec.EndsWith(")"))
            {
                sVec = sVec.Substring(1, sVec.Length - 2);
            }

            // split the items
            return sVec.Split(',');
        }
    }


    public class UnityEventData
    {
        public int eventContextInd;

        [SerializeField]
        private string _eventName;
        public string eventName
        {
            set
            {
                _eventName = VRsqr_EventsManager.StandardizeString(value); //  value.Replace(" ", string.Empty).ToLower();
                VRsqrUtil.Debug.LogInfo(">>>>>>>>>>> UnityEventData - SET: _eventName = " + _eventName);
            }
            get
            {
                return _eventName;
            }
        }
        //public string eventName;

        public double eventOutTimeStamp;
        public double eventInTimeStamp;

        public EventParam[] eventParams;
        public Dictionary<string, int> varInd;

        public bool presistentData;
    };


    public sealed class VRsqr_EventsManager //: MonoBehaviour
    {
        private Dictionary<string, UnityEventInt> NameToListenedEventDictionary;
        private Dictionary<string, int> NameToLastEventIndDictionary;

        public const int numDataBuffers = 500; // Max Concurrent Events
        public static UnityEventData[] eventDataBuffers = new UnityEventData[numDataBuffers];
        public static int freeDataInd = 0;

        public static readonly VRsqr_EventsManager instance = new VRsqr_EventsManager();

        static VRsqr_EventsManager()
        {
        }

        private VRsqr_EventsManager()
        {
            Init();
        }

        void Init()
        {
            if (NameToListenedEventDictionary == null)
            {
                NameToListenedEventDictionary = new Dictionary<string, UnityEventInt>();
            }
            if (NameToLastEventIndDictionary == null)
            {
                NameToLastEventIndDictionary = new Dictionary<string, int>();
            }
        }

        public static string StandardizeString(string input)
        {
            VRsqrUtil.Debug.LogInfo("xxxxxxxxxxxx StandardizeString: input = " + input + " >>>>> input.Replace(...).ToLower() = " + input.Replace(" ", string.Empty).ToLower());
            return input.Replace(" ", string.Empty).ToLower();
        }

        public static void StartListening(string eventName, UnityAction<int> listener)
        {
            eventName = StandardizeString(eventName);

            VRsqrUtil.Debug.LogInfo("StartListening: eventName = " + eventName);
            UnityEventInt thisEvent = null;
            if (instance.NameToListenedEventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEventInt();
                thisEvent.AddListener(listener);
                //instance.NameToListenedEventDictionary.Add(eventName.ToLower(), thisEvent);
                instance.NameToListenedEventDictionary[eventName] = thisEvent;
            }
        }

        public static void StopListening(string eventName, UnityAction<int> listener)
        {
            eventName = StandardizeString(eventName);

            //if (eventManager == null) return;
            UnityEventInt thisEvent = null;
            if (instance.NameToListenedEventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent(string eventName)
        {
            eventName = StandardizeString(eventName);

            TriggerDataEvent(eventName);
        }

        public static UnityEventData GetEventData(int EventContextInd)
        {
            int dataBufferInd = EventContextInd % numDataBuffers;
            if (eventDataBuffers[dataBufferInd].eventContextInd == EventContextInd)
            {
                return eventDataBuffers[dataBufferInd]; // returns a *reference* to the buffer
            }
 
            return null;
        }

        public static UnityEventData GetLastEventData(string eventName)
        {
            eventName = StandardizeString(eventName);

            int dataBufferInd;
            bool eventExists = instance.NameToLastEventIndDictionary.TryGetValue(eventName, out dataBufferInd);
            if (eventExists)
            {
                return GetEventData(dataBufferInd);
            }

            return null;
        }

        private static int GetFreeEventDataInd()
        {
            //int EventContextInd = freeDataInd;
            //freeDataInd = (freeDataInd + 1) % numDataBuffers;
            return freeDataInd++; // returns a *reference* to the buffer
        }

 
        // rotem 27.2.19
        public static void TriggerPosEvent(string eventName, string varName, Vector3 posVec, bool persistent = false, double timeStamp = -1)
        {
            EventParam[] eventParams = new EventParam[] { new EventParam { NameString = varName, ValString = posVec.ToString("F4"), /*TypeString = "Vector3",*/ paramType = posVec.GetType() } };
            //EventDataVar eventData = new EventDataVar { data3D = posVec };
            TriggerDataEvent(eventName, eventParams, persistent, timeStamp);
        }

        public static void TriggerRotEvent(string sEventName, string sVarName, Quaternion qRot, bool persistent = false, double dTimeStamp = -1)
        {
            TriggerDataEvent(sEventName, sVarName, qRot.ToString("F4"), "Quaternion", persistent, dTimeStamp);
        }

        public static void TriggerDataEvent(string eventName, string sVarName, string sVarVal, string sVarType, bool persistent = false, double timeStamp = -1)
        {
            EventParam[] eventParams = new EventParam[] { new EventParam { NameString = sVarName, ValString = sVarVal, /*TypeString = sVarType,*/ paramType = sVarVal.GetType() } };
            //EventDataVar eventData = new EventDataVar { data3D = posVec };
            TriggerDataEvent(eventName, eventParams, persistent, timeStamp);
        }

        public static void TriggerDataEvent(string eventName, EventParam[] eventParams = null, bool persistent = false, double timeStamp = -1) 
        {
            if (String.IsNullOrEmpty(eventName))
                return;

            VRsqrUtil.Debug.LogInfo("TriggerDataEvent: eventName = " + eventName);
            if (timeStamp < 0)
            {
                timeStamp = DateTime.Now.Hour * 60 * 60 * 1000 + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond; //(float)ts.TotalMilliseconds;
            }

            TriggerDataEvent(new UnityEventData { eventName = eventName, eventParams = eventParams, presistentData = persistent, eventOutTimeStamp = timeStamp });    
        }

        public static int GetEventDataVarsNum(int EventContextInd)
        {
            UnityEventData eventData = GetEventData(EventContextInd);
            return eventData.eventParams.Length;
        }

        public static EventParam[] GetEventDataVars(int EventContextInd)
        {
            UnityEventData eventData = GetEventData(EventContextInd);
            return eventData.eventParams;
        }

        public static EventParam GetEventParam(int EventContextInd, int varInd)
        {
            UnityEventData eventData = GetEventData(EventContextInd);
            if (eventData.eventParams != null && 
                eventData.eventParams.Length > varInd)
            {
                return eventData.eventParams[varInd];
            }
            return null;
        }

        public static EventParam GetEventParam(int EventContextInd, string varName)
        {
            UnityEventData eventData = GetEventData(EventContextInd);
            return GetEventParam(eventData, varName);
        }

        public static double getEventInTime(int EventContextInd)
        {
            UnityEventData eventData = GetEventData(EventContextInd);
            return eventData.eventInTimeStamp;
        }

        public static double getEventOutTime(int EventContextInd)
        {
            UnityEventData eventData = GetEventData(EventContextInd);
            return eventData.eventOutTimeStamp;
        }

        public static EventParam GetEventParam(UnityEventData eventData, string varName)
        {
            varName = StandardizeString(varName);

            int varInd = 0;
            if (varName != null && eventData != null &&
                eventData.varInd != null &&
                eventData.varInd.TryGetValue(varName, out varInd))
            {
                VRsqrUtil.Debug.LogInfo("GetEventParam: varName = " + varName + " return eventData.eventParams[varInd] = " + eventData.eventParams[varInd]);
                return eventData.eventParams[varInd];
            }
            else
            {
                VRsqrUtil.Debug.Log(LogLevel.Warning, "GetEventParam: Illegal arguments - eventData = " + eventData + " , varName = " + varName + " , eventData.varInd = " + eventData.varInd);
            }
            return null;
        }

        public static void GetEventParam(int EventContextInd, ref EventParam eventParam)
        {
            EventParam tempParam = GetEventParam(EventContextInd, eventParam.NameString);
            if (tempParam != null)
            {
                eventParam = tempParam;
            }
        }

        // return last value of data param of *persistent* event
        public static EventParam GetLastEventParam(string eventName, string paramName)
        {
            UnityEventData eventData = GetLastEventData(eventName);
            if (eventData != null)
            {
                return GetEventParam(eventData, paramName);
            }

            return null;
        }


        public static void TriggerDataEvent(UnityEventData unityEventData)
        {
            if (unityEventData == null || unityEventData.eventName == "")
            {
                VRsqrUtil.Debug.Log(LogLevel.Error, "TriggerDataEvent: Illegal unityEventData argument");
                return;
            }

            UnityEventInt thisEvent = null;
            bool listenerExists = instance.NameToListenedEventDictionary.TryGetValue(unityEventData.eventName, out thisEvent);
            VRsqrUtil.Debug.LogInfo("TriggerDataEvent: eventName = " + unityEventData.eventName + "    listenerExists = " + listenerExists);
            if (unityEventData.presistentData || listenerExists) // there is at least one listerner to this event
            {
                int eventContextInd = GetFreeEventDataInd();
                int dataBufferInd = eventContextInd % numDataBuffers;
                eventDataBuffers[dataBufferInd] = unityEventData;
                eventDataBuffers[dataBufferInd].eventContextInd = eventContextInd;

                // keep the index of the most recent triggering of this event - to support getting values of past events (e.g. in case of persistent events with no listener)
                //instance.NameToLastEventIndDictionary.Add(unityEventData.eventName.ToLower(), dataBufferInd);
                instance.NameToLastEventIndDictionary[unityEventData.eventName] = dataBufferInd;

                unityEventData.varInd = new Dictionary<string, int>();
                if (unityEventData.eventParams != null)
                {
                    for (int i = 0; i < unityEventData.eventParams.Length; i++)
                    {
                        //unityEventData.varInd.Add(unityEventData.eventParams[i].NameString.ToLower(), i);
                        //unityEventData.varInd[unityEventData.eventParams[i].NameString.ToLower()] = i;
                        unityEventData.eventParams[i].NameString = VRsqr_EventsManager.StandardizeString(unityEventData.eventParams[i].NameString);
                        unityEventData.varInd[unityEventData.eventParams[i].NameString] = i;
                        VRsqrUtil.Debug.Log(LogLevel.Debug, "---------- TriggerDataEvent: NameString = " + unityEventData.eventParams[i].NameString +
                                                " , ValString = " + unityEventData.eventParams[i].ValString +
                                                " , paramObjVal = " + unityEventData.eventParams[i].paramObjVal);
                    }
                }

                if (listenerExists)
                {
                    thisEvent.Invoke(eventContextInd);
                }
            }
        }

 
        //rotem 27.2.19
        public static void TriggerEvent(string eventName, double val)
        {
            EventParam[] eventParams = new EventParam[] { new EventParam { NameString = "val", ValString = val.ToString("F4"), /*TypeString = "double",*/ paramType = val.GetType() } };
            TriggerDataEvent(eventName, eventParams);
        }

        public static void TriggerEvent(string eventName, string val)
        {
            EventParam[] eventParams = new EventParam[] { new EventParam { NameString = "val", ValString = val/*, TypeString = "string"*/ } };
            TriggerDataEvent(eventName, eventParams);
        }


        public static void TriggerEvent(string eventName, double val1, double val2)
        {
            EventParam[] eventParams = new EventParam[] { new EventParam { NameString = "val1", ValString = val1.ToString("F4"), /*TypeString = "double",*/ paramType = val1.GetType() },
                                                 new EventParam { NameString = "val2", ValString = val2.ToString("F4"), /*TypeString = "double",*/ paramType = val2.GetType() }};
            TriggerDataEvent(eventName, eventParams);
        }

        public static void TriggerEvent(string eventName, double val1, double val2, double val3)
        {
            EventParam[] eventParams = new EventParam[] { new EventParam { NameString = "val1", ValString = val1.ToString("F4"), /*TypeString = val1.GetType().Name,*/ paramType = val1.GetType() },
                                                 new EventParam { NameString = "val2", ValString = val2.ToString("F4"), /*TypeString = "double",*/ paramType = val2.GetType() },
                                                 new EventParam { NameString = "val3", ValString = val3.ToString("F4"), /*TypeString = "double",*/ paramType = val3.GetType() }};
            TriggerDataEvent(eventName, eventParams);
        }

        public static void TriggerEvent(string eventName, double val1, double val2, double val3, double val4)
        {
            EventParam[] eventParams = new EventParam[] { new EventParam { NameString = "val1", ValString = val1.ToString("F4"), /*TypeString = "double",*/ paramType = val1.GetType() },
                                                 new EventParam { NameString = "val2", ValString = val2.ToString("F4"), /*TypeString = "double",*/ paramType = val2.GetType() },
                                                 new EventParam { NameString = "val3", ValString = val3.ToString("F4"), /*TypeString = "double",*/ paramType = val3.GetType() },
                                                 new EventParam { NameString = "val4", ValString = val4.ToString("F4"), /*TypeString = "double",*/ paramType = val4.GetType() }};
            TriggerDataEvent(eventName, eventParams);
        }
    }
}
