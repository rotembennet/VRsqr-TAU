using System;
using UnityEngine;
using UnityEngine.Events;

namespace VRsqrCore
{
    [Serializable]
    public class InputVars //: MonoBehaviour
    {
        [Tooltip("Input variable value gets updated automatically when inputs are intercepted")]
        [Header("Input Variable")]
        private string eventName;
        private UnityAction<int> basicEventHandler;
        private UnityAction<int> customEventHandler;

         private class EventParamsField
        {
            public FieldInfo fiEventParamObj;
            public string paramName;
        }
        private List<EventParamsField> eventParamFields = new List<EventParamsField>();


        public void StartListening(GameObject listeningObj, UnityAction<int> customEventHandler = null)
        {
            VRsqrUtil.Debug.Log("StartListening");
            this.basicEventHandler = eventHandlerFunc;

            VRsqrUtil.Debug.Log("StartListening: listeningObj.name = " + listeningObj.name);
            string gameObjName = listeningObj.name;

            Type tempType = this.GetType();
            String typeString = tempType.ToString();
            string[] splitArray = typeString.Split("+"[0]);
            string scriptName = splitArray[0];
            VRsqrUtil.Debug.Log("StartListening: scriptName = " + scriptName);

            Component script = listeningObj.GetComponent(scriptName);
            FieldInfo[] scriptFields = script.GetType().GetFields();

            string eventName = gameObjName;
            foreach (FieldInfo field in scriptFields)
            {
                VRsqrUtil.Debug.Log("StartListening: field = " + field.Name);
                if (System.Object.ReferenceEquals(field.GetValue(script), this))
                {
                    VRsqrUtil.Debug.Log("=== StartListening: field.GetValue(script) = " + field.GetValue(script));
                    eventName += "." + field.Name;
                }
            }
            this.eventName = eventName;

            VRsqr_EventsManager.StartListening(eventName, this.basicEventHandler);
            if (customEventHandler != null)
            {
                this.customEventHandler = customEventHandler;
                VRsqr_EventsManager.StartListening(eventName, this.customEventHandler);
            }

            FieldInfo[] eventParamFieldInfo = this.GetType().GetFields();
            foreach (FieldInfo pfi in eventParamFieldInfo)
            {
                if (pfi.FieldType != typeof(EventParam)) // TEMP
                {
                    string varName = pfi.Name;
                    VRsqrUtil.Debug.Log("StartListening: varName = " + varName);

                    this.eventParamFields.Add(new EventParamsField { fiEventParamObj = pfi, paramName = varName });
                }
            }
        }

        public void eventHandlerFunc(int EventContext)
        {
            VRsqrUtil.Debug.Log("eventHandlerFunc: EventContext = " + EventContext);
            // use reflection mechanisms to collect the EventParam fields, as added in the definition of the current sub-class, which is inheriting EventIn
            foreach (EventParamsField epf in this.eventParamFields)
            {
                VRsqrUtil.Debug.Log("eventHandlerFunc:  epf.paramName = " + epf.paramName);
                EventParam ep = VRsqr_EventsManager.GetEventParam(EventContext, epf.paramName);
                VRsqrUtil.Debug.Log("eventHandlerFunc:  ep.Val() = " + ep.Val());
                epf.fiEventParamObj.SetValue(this, ep.Val());
            }
        }

    }
}
