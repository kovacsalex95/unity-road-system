using System.IO;
using UnityEditor;
using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    public class Resources
    {
        private static GUIContent toolbarIcon = null;
        
        private static string systemPath = null;


        private static T LoadResource<T>(string path) where T : Object
        {
            if (systemPath == null)
                systemPath = Path.GetDirectoryName(Util.GetMonoScriptPathFor(typeof(RoadSystem)));
            
            return AssetDatabase.LoadAssetAtPath<T>(systemPath +"/" + path);
        }


        public static GUIContent ToolbarIcon
        {
            get
            {
                if (toolbarIcon == null)
                {
                    string iconSuffix = EditorGUIUtility.isProSkin ? "light" : "dark";
                    toolbarIcon = new GUIContent(LoadResource<Texture2D>($"Images/icon_{iconSuffix}.png"));
                }

                return toolbarIcon;
            }
        }
    }
}