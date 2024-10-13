using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SA.BehaviorEditor
{
    public class BehaviorIO
    {
        
        public static State CreateStateSO(string stateName, string folderName)
        {
            if (string.IsNullOrEmpty(stateName) || string.IsNullOrEmpty(folderName))
            {
                Debug.LogError("State Name and Folder Name cannot be empty.");
                return null;
            }

            string folderPath = Path.Combine("Assets", folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, stateName + ".cs");
            if (File.Exists(filePath))
            {
                Debug.LogError("File already exists.");
                return null;
            }

            string assetPath = Path.Combine(folderPath, stateName + ".asset");
            State newState = ScriptableObject.CreateInstance<State>();
            newState.name = stateName;

            AssetDatabase.CreateAsset(newState, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newState;

            Debug.Log("State ScriptableObject created at " + assetPath);
            return newState;
        }

        public static ScriptableObject CreateSOForAction(string typeName, string newActionSOName, string newFolderNamee)
        {
            if (string.IsNullOrEmpty(newActionSOName) || string.IsNullOrEmpty(newFolderNamee))
            {
                Debug.LogError("State Name and Folder Name cannot be empty.");
                return null;
            }

            string folderPath = Path.Combine("Assets", newFolderNamee);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, newActionSOName + ".cs");
            if (File.Exists(filePath))
            {
                Debug.LogError("File already exists.");
                return null;
            }

            string assetPath = Path.Combine(folderPath, newActionSOName + ".asset");

            // Use reflection to find the type
            //Type type = Type.GetType(typeName);
            Type type = GetTypeFromAllAssemblies(typeName);
            if (type == null)
            {
                Debug.LogError("Type not found: " + typeName);
                return null;
            }
            
            // Check if the type is a ScriptableObject
            if (!typeof(ScriptableObject).IsAssignableFrom(type))
            {
                Debug.LogError(typeName + " is not a ScriptableObject type.");
                return null;
            }
            
            // Create an instance of the ScriptableObject
            ScriptableObject newAction = ScriptableObject.CreateInstance(type);
            newAction.name = newActionSOName;

            // Save the new instance as an asset
            AssetDatabase.CreateAsset(newAction, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Focus on the newly created asset
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newAction;

            Debug.Log("State ScriptableObject created at " + assetPath);
            return newAction;
        }

        private static Type GetTypeFromAllAssemblies(string typeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(type => type.Name == typeName);
        }
        public static bool CheckIfFileExists(string newActionName, string newActionFolderName)
        { 
            string folderPath = Path.Combine("Assets", newActionFolderName);
            if (!Directory.Exists(folderPath))
            {
                return false;
            }

            return true;
        }

        public static bool CreateScriptForActionWithMenuPath(string newActionName, string newActionFolderName, string newActionMenuPath)
        {
            if (string.IsNullOrEmpty(newActionName) || string.IsNullOrEmpty(newActionFolderName))
            {
                Debug.LogError("State Name and Folder Name cannot be empty.");
                return false;
            }

            string folderPath = Path.Combine("Assets", newActionFolderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, newActionName + ".cs");
            if (File.Exists(filePath))
            {
                Debug.LogError("File already exists.");
                return false;
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("");
                writer.WriteLine("[CreateAssetMenu(fileName = \"" + newActionName + $"\", menuName = \"{newActionMenuPath}/" + newActionName + "\", order = 1)]");
                writer.WriteLine("public class " + newActionName + " : ScriptableObject");
                writer.WriteLine("{");
                writer.WriteLine("    // Add your variables here");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
            Debug.Log("ScriptableObject script created at " + filePath);
            return true;
        }
    }
}