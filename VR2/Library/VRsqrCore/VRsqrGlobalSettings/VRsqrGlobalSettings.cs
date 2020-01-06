//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using VRsqrCore;
using VRsqrUtil;

namespace VRsqrCore
{
    public class VRsqrGlobalSettings : MonoBehaviour
    {

        public static string DataPrePath;
        public string dataPrePath;
        public static string DataPostPath;
        public string dataPostPath;

        public static string SubjectsStr;
        public string subjectsStr;

        public static VRsqrUtil.LogLevel MaxLogLevel;
        public VRsqrUtil.LogLevel maxLogLevel = VRsqrUtil.LogLevel.None;

        public static string MessageFilter;
        public string messageFilter;


        // Use this for initialization
        void Start()
        {
            updateVars();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnValidate()
        {
            updateVars();
        }

        void updateVars()
        {
            SubjectsStr = subjectsStr;
            DataPrePath = dataPrePath;
            DataPostPath = dataPostPath;
            MaxLogLevel = maxLogLevel;
            VRsqrUtil.Debug.maxLogLevel = MaxLogLevel;
            MessageFilter = messageFilter;
            VRsqrUtil.Debug.messageFilter = MessageFilter;
        }
    }
}
