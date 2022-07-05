using ThunderKit.Core.Pipelines;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class DevUtilsWindow : EditorWindow
    {
        [MenuItem("Window/UI Toolkit/DevUtilsWindow")]
        public static void ShowExample()
        {
            DevUtilsWindow wnd = GetWindow<DevUtilsWindow>();
            wnd.titleContent = new GUIContent("DevUtilsWindow");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DevUtilsWindow.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            labelFromUXML.Q<Button>("BuildButton").clicked += async () =>
            {
                await AssetDatabase.LoadAssetAtPath<Pipeline>("Assets/Pipelines/Deploy.asset").Execute();
            };
        }
    }
}