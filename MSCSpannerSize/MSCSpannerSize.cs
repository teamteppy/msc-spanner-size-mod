using HutongGames.PlayMaker;
using MSCLoader;
using System.Diagnostics;
using UnityEngine;

namespace MSCSpannerSize
{
    public class MSCSpannerSize : Mod
    {
        public override string ID => "MSCSpannerSize"; // Your (unique) mod ID 
        public override string Name => "Display & Change Spanner Size"; // Your mod name
        public override string Author => "taxi"; // Name of the Author (your name)
        public override string Version => "1.0"; // Version
        public override string Description => ""; // Short description of your mod 
        public override Game SupportedGames => Game.MySummerCar;

        private PlayMakerGlobals globals;
        private SettingsKeybind debugKey;
        private FsmFloat spannerSize;
        private SettingsKeybind increaseSizeKey;
        private SettingsKeybind decreaseSizeKey;
        private Transform toolsContainer;



        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.OnGUI, Mod_OnGUI);
            SetupFunction(Setup.Update, Mod_Update);
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }

        private void LogToFile(string message)
        {
            string path = Application.persistentDataPath + "/MSCPauseMod_debug.txt";
            System.IO.File.AppendAllText(path, message + "\n");
        }

        private string GetGameObjectPath(GameObject go)
        {
            string path = go.name;
            Transform t = go.transform.parent;
            while (t != null)
            {
                path = t.name + "/" + path;
                t = t.parent;
            }
            return path;
        }
        private void SetSpannerInBox(float oldSize, float newSize)
        {
            if (toolsContainer == null) return;

            int oldIndex = Mathf.RoundToInt(oldSize * 10);
            int newIndex = Mathf.RoundToInt(newSize * 10);

            Transform oldSpanner = toolsContainer.Find($"spanner({oldIndex})(Clone)");
            Transform newSpanner = toolsContainer.Find($"spanner({newIndex})(Clone)");

            if (oldSpanner != null) oldSpanner.gameObject.SetActive(true);
            if (newSpanner != null) newSpanner.gameObject.SetActive(false);
        }

        private void Mod_Settings()
        {
            debugKey = Keybind.Add("DebugKey", "Debug Game", KeyCode.Alpha9);
            increaseSizeKey = Keybind.Add("IncreaseSizeKey", "Increase Spanner Size", KeyCode.G);
            decreaseSizeKey = Keybind.Add("DecreaseSizeKey", "Decrease Spanner Size", KeyCode.B);

        }

        private void Mod_OnLoad()
        {
            globals = PlayMakerGlobals.Instance;
            spannerSize = globals.Variables.FindVariable("ToolWrenchSize") as FsmFloat;

            GameObject spannerSet = GameObject.Find("spanner set(itemx)");
            if (spannerSet != null)
            {
                toolsContainer = spannerSet.transform.Find("Tools");
            }

        }
        private void Mod_OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 60;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperCenter;

            //GUI.Label(new Rect(0, 20, Screen.width, 60), "Testing", style);

            string label;
            if (spannerSize != null && !(spannerSize.Value == 0 || spannerSize.Value == 0.55f || spannerSize.Value == 0.65f))
            {
                label = (spannerSize.Value * 10) + "mm";
                GUI.Label(new Rect(0, 60, Screen.width, 60), label, style);
            }
        }
        private void Mod_Update()
        {

            if (debugKey.GetKeybindDown())
            {
                //foreach (var v in globals.Variables.FloatVariables)
                //    LogToFile($"GLOBAL FloatVar: {v.Name.PadRight(30)} Val: {v.Value}");

                //foreach (var v in globals.Variables.IntVariables)
                //    LogToFile($"GLOBAL IntVar:   {v.Name.PadRight(30)} Val: {v.Value}");

                //foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                //{
                //    string lower = go.name.ToLower();
                //    if (lower.Contains("spanner") || lower.Contains("wrench") || lower.Contains("tool") || lower.Contains("box") || lower.Contains("set"))
                //    {
                //        LogToFile($"[MATCH] {go.name} at path: {GetGameObjectPath(go)}");
                //    }
                //}

                //GameObject spannerSet = GameObject.Find("spanner set(itemx)");
                //if (spannerSet != null)
                //{
                //    LogToFile($"[FOUND] spanner set at: ITEMS/spanner set(itemx)");
                //    LogToFile($"Child count: {spannerSet.transform.childCount}");
                //    foreach (Transform child in spannerSet.transform)
                //    {
                //        LogToFile($"  Child: {child.name} active={child.gameObject.activeSelf}");
                //    }
                //}
                //else
                //{
                //    LogToFile("spanner set NOT found");
                //}

                GameObject spannerSet = GameObject.Find("spanner set(itemx)");
                if (spannerSet != null)
                {
                    Transform tools = spannerSet.transform.Find("Tools");
                    if (tools != null)
                    {
                        LogToFile($"Tools child count: {tools.childCount}");
                        foreach (Transform child in tools)
                        {
                            LogToFile($"  Child: {child.name} active={child.gameObject.activeSelf}");
                        }
                    }
                }

            }
            
            if (spannerSize != null && !(spannerSize.Value == 0 || spannerSize.Value == 0.55f || spannerSize.Value == 0.65f))
            {
                if (increaseSizeKey.GetKeybindDown())
                {
                    float newSize = Mathf.Round(Mathf.Clamp(spannerSize.Value + 0.1f, 0.5f, 1.6f) * 10) / 10f;
                    SetSpannerInBox(spannerSize.Value, newSize);
                    spannerSize.Value = newSize;
                }

                if (decreaseSizeKey.GetKeybindDown())
                {
                    float newSize = Mathf.Round(Mathf.Clamp(spannerSize.Value - 0.1f, 0.5f, 1.6f) * 10) / 10f;
                    SetSpannerInBox(spannerSize.Value, newSize);
                    spannerSize.Value = newSize;
                }
            }

        }
    }
}
