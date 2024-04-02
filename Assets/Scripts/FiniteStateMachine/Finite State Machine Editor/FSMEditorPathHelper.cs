#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FiniteStateMachine;

namespace FiniteStateMachineEditor
{
    public static class FSMEditorPathHelper
    {
        

        public static string PathOfFolder(string pathOfAsset)
        {
            return System.IO.Path.GetDirectoryName(pathOfAsset);
        }

        public static string PathOfFolder(ScriptableObject scriptableObject)
        {
            string pathOfAsset = AssetDatabase.GetAssetPath(scriptableObject);
            return PathOfFolder(pathOfAsset);
        }

        public static string MakePathForScriptableObject(FSMDefinition fsmDefinition, ref string title, string subfolder = null
            , string pathBeingRenamed = null)
        {
            string pathOfParentFolder = PathOfFolder(fsmDefinition);
            string pathOfFolder = pathOfParentFolder;
            if (subfolder != null && subfolder.Length != 0)
            {
                pathOfFolder = System.IO.Path.Combine(pathOfFolder, subfolder);
                if (!AssetDatabase.IsValidFolder(pathOfFolder))
                    AssetDatabase.CreateFolder(pathOfParentFolder, subfolder);
            }
            if (!AssetDatabase.IsValidFolder(pathOfFolder))
                throw new System.InvalidOperationException("The folder isn't valid somehow.");

            title = AdjustTitleToNotAlreadyExist(title, pathOfFolder, pathBeingRenamed);
            return System.IO.Path.Combine(pathOfFolder, $"{title}.asset");
        }

        public static string AdjustTitleToNotAlreadyExist(string title, string pathOfFolder
            , string pathBeingRenamed = null)
        {
            // Maybe AssetDatabase.Rename etc. already handle this.

            int number = 0;
            string path;
            string newTitle;
            do
            {
                newTitle = number == 0 ? title : $"{title} {number}";
                string fileName = $"{newTitle}.asset";
                path = System.IO.Path.Combine(pathOfFolder, fileName);
                number++;
            } while (AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets).Length != 0
            && (pathBeingRenamed == null || path != pathBeingRenamed)); // the path can already exist if it's being renamed

            return newTitle;
        }
    }
}
#endif