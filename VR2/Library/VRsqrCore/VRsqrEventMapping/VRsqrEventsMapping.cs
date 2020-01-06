using System.Collections.Generic;
using UnityEngine;

namespace VRsqrCore
{
    public class VRsqrEventMapping : MonoBehaviour
    {
        [System.Serializable]
        public struct EventMapping
        {
            public string eventIn;
            public string eventOut;
        };

        public List<EventMapping> EventMap;

        public Dictionary<string, string> EventDict = new Dictionary<string, string>();

        // Use this for initialization
        void OnEnable()
        {
            foreach (var e in EventMap)
            {
                VRsqr_EventsManager.StartListening(e.eventIn, eventHandler_In);
            }
        }

        void eventHandler_In(int EventContextInd)
        { 
            UnityEventData eventData = VRsqr_EventsManager.GetEventData(EventContextInd);
            VRsqr_EventsManager.TriggerDataEvent(eventData);
        }

        void Update()
        {
            // do something every frame...
        }
    }

}
