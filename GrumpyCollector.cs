using System;
using KSP.IO;
using UnityEngine;

namespace GrumpyCollector
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class GrumpyCollector : MonoBehaviour
    {

        private const string cfgName = "GrumpyCollector.cfg";

        private bool showUI = false;

        private Rect windowPos = new Rect(40, 40, 50, 50);

        private float interval = 0.5f;

        [Persistent] private int intervalIdx = 2;

        private float lastGC = 0;

        internal void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (File.Exists<GrumpyCollector>(cfgName))
            {
                ConfigNode config = ConfigNode.Load(IOUtils.GetFilePathFor(this.GetType(), cfgName));
                ConfigNode.LoadObjectFromConfig(this, config);
            }
            SetInterval();
        }

        internal void OnDestroy()
        {
            ConfigNode node = new ConfigNode("GrumpyCollector");
            ConfigNode.CreateConfigFromObject(this, node);
            node.Save(IOUtils.GetFilePathFor(this.GetType(), cfgName));
        }

        void Update()
        {
            SetInterval();
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                showUI = !showUI;
            }
            if (Time.realtimeSinceStartup > lastGC + interval)
            {
                GC.Collect(0, GCCollectionMode.Forced);
                lastGC = Time.realtimeSinceStartup;
                MonoBehaviour.print("Forced GC");
            }
        }

        public void OnGUI()
        {
            if (showUI)
            {
                windowPos = GUILayout.Window(
                    8785499,
                    windowPos,
                    WindowGUI,
                    "GrumpyCollector",
                    GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true));
            }
        }

        public void WindowGUI(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Forced Garbage Interval");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Faster", GUILayout.ExpandWidth(true)))
            {
                intervalIdx++;
                if (intervalIdx == 0)
                    intervalIdx = 2;
            }

            GUILayout.Label(string.Format("{0} s", interval.ToString("F3")), GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Slower", GUILayout.ExpandWidth(true)))
            {
                intervalIdx--;
                if (intervalIdx == 0)
                    intervalIdx = -2;
            }

            intervalIdx = Mathf.Clamp(intervalIdx, -10, 10);

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        private void SetInterval()
        {
            if (intervalIdx > 0)
            {
                interval = 1f / intervalIdx;
            }
            else
            {
                interval = -intervalIdx;
            }
        }
    }
}
