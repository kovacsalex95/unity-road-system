using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    public class Util
    {
        public static string GetMonoScriptPathFor(Type type)
        {
            var asset = "";
            var guids = AssetDatabase.FindAssets($"{type.Name} t:script");

            if (guids.Length > 1)
            {
                foreach (var guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    var filename = Path.GetFileNameWithoutExtension(assetPath);

                    if (filename == type.Name)
                    {
                        asset = guid;
                        break;
                    }
                }
            }
            else if (guids.Length == 1)
                asset = guids[0];
            
            else
            {
                Debug.LogErrorFormat("Unable to locate {0}", type.Name);
                return "";
            }

            return AssetDatabase.GUIDToAssetPath(asset);
        }
    }
}
