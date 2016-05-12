using System;
using System.Text;
using KSP.IO;
using UnityEngine;

namespace GrumpyCollector
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class GrumpyCollector : MonoBehaviour
    {
        private const string cfgName = "GrumpyCollector.cfg";

        private bool showUI = false;

        private Rect windowPos = new Rect(40, 40, 150, 46);
        private Rect windowDragRect = new Rect(0, 0, 150, 46);

        private Rect labelRectInterval = new Rect(10, 18, 80, 20);
        private Rect labelRectRate = new Rect(92, 18, 48, 20);

        private float interval = 0f;
        private string strInterval;

        [Persistent] private bool enableGC = false;
        [Persistent] private int intervalIdx = 2;

        private float lastGC = 0;

        private StringBuilder strBuild;

        internal void Awake()
        {
            DontDestroyOnLoad(gameObject);

            strBuild = new StringBuilder(32);

            if (File.Exists<GrumpyCollector>(cfgName))
            {
                ConfigNode config = ConfigNode.Load(IOUtils.GetFilePathFor(GetType(), cfgName));
                ConfigNode.LoadObjectFromConfig(this, config);
            }
            SetInterval();
        }

        internal void OnDestroy()
        {
            ConfigNode node = new ConfigNode("GrumpyCollector");
            ConfigNode.CreateConfigFromObject(this, node);
            node.Save(IOUtils.GetFilePathFor(GetType(), cfgName));
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.KeypadMultiply))
                {
                    showUI = !showUI;
                }
                if (Input.GetKeyDown(KeyCode.KeypadDivide))
                {
                    enableGC = !enableGC;
                }
                if (Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    intervalIdx--;
                    if (intervalIdx == 0)
                        intervalIdx = -2;
                    else if (intervalIdx < -10)
                        intervalIdx = -10;
                    SetInterval();
                }
                if (Input.GetKeyDown(KeyCode.KeypadMinus))
                {
                    intervalIdx++;
                    if (intervalIdx == 0)
                        intervalIdx = 2;
                    else if (intervalIdx > 10)
                        intervalIdx = 10;
                    SetInterval();
                }
            }

            if (enableGC && (Time.realtimeSinceStartup > lastGC + interval))
            {
                GC.Collect(0, GCCollectionMode.Forced);
                lastGC = Time.realtimeSinceStartup;
            }
        }

        public void OnGUI()
        {
            if (showUI)
            {
                windowPos = GUI.Window(
                    8785499,
                    windowPos,
                    WindowGUI,
                    "GrumpyCollector");
            }
        }

        public void WindowGUI(int windowID)
        {
            GUI.Label(labelRectInterval, enableGC ? "GC Enabled:" : "GC Disabled:");
            GUI.Label(labelRectRate, strInterval);
            GUI.DragWindow(windowDragRect);
        }

        private void SetInterval()
        {
            float newinterval = (intervalIdx > 0) ? 1f / intervalIdx : -intervalIdx;
            if (newinterval != interval)
            {
                interval = newinterval;
                strBuild.Length = 0;
                strBuild.Append(interval);
                strBuild.Length = (interval == 10f) ? 6 : 5;
                strBuild.Append(" s");
                strInterval = strBuild.ToString();
            }
        }
    }
}
