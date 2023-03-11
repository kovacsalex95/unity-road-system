using System.IO;
using UnityEditor;
using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    public class Resources
    {
        private static GUIContent toolbarIcon = null;
        
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