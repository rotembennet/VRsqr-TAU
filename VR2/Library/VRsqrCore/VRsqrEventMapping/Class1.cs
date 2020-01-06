using UnityEngine;
using VRsqrCore;

namespace VRsqrEventMapping
{
    public class FlowElementScript : MonoBehaviour
    {

        public string someEvent_In = "";
        public string someEvent_Out = "";
        public float data1;
        public float data2;

        // Use this for initialization
        void OnEnable()
        {
            VRsqr_EventsManager.StartListening(someEvent_In, eventHandler_In);
        }

        void eventHandler_In(int dataId)
        { //float arg1, float arg2) {
            UnityEventData eventData = VRsqr_EventsManager.GetEventData(dataId);

            data1 = eventData.dataArgs[0];
            data2 = eventData.dataArgs[1];

            VRsqr_EventsManager.TriggerEvent(someEvent_Out, data1 * data2, data1 / data2);
        }

        void Update()
        {
            // do something every frame...
        }
    }

}
