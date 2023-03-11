using System.IO;
using UnityEditor;
using UnityEngine;


namespace unity_road_system
{
    public class Resources
    {
        private static Material roadNodeMaterial;

        private static Mesh roadNodeMesh;

        private static GUIContent toolbarIcon;
        
        private static string systemPath;


        private static T LoadResource<T>(string path) where T : Object
        {
            if (systemPath == null)
                systemPath = Path.GetDirectoryName(Util.GetMonoScriptPathFor(typeof(RoadSystem)));
            
            return AssetDatabase.LoadAssetAtPath<T>(systemPath +"/" + path);
        }


        public static Material RoadNodeMaterial
        {
            get
            {
                if (roadNodeMaterial == null)
                    roadNodeMaterial = LoadResource<Material>($"Materials/RoadNode.mat");

                return roadNodeMaterial;
            }
        }


        public static Mesh RoadNodeMesh
        {
            get
            {
                if (roadNodeMesh == null)
                    roadNodeMesh = Geometry.GenerateSphere(24, 12, 0.1f);

                return roadNodeMesh;
            }
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