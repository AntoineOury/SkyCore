#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FiniteStateMachine;

namespace FiniteStateMachineEditor
{
    public static class FSMEditorSaver
    {
        public static FSMParameter AddParameter(FSMDefinition fsmDefinition)
        {
            FSMParameter newParameter = CreateScriptableObject<FSMParameter>(fsmDefinition, "New Parameter", "Parameters");
            FSMEditorSaveDataChanger.AddParameterToFSMDefinition(newParameter, fsmDefinition);
            return newParameter;
        }

        public static FSMTransition CreateTransition(FSMDefinition fsmDefinition, FSMEditorOneState from, FSMEditorOneState to)
        {
            string transitionTitle = $"[{from.Name}] to [{to.Name}]";
            FSMTransition newTransition = CreateScriptableObject<FSMTransition>(fsmDefinition, transitionTitle, "Transitions");

            FSMEditorSaveDataChanger.SetFSMTransitionFromAndTo(newTransition, from.State, to.State);
            FSMEditorSaveDataChanger.AddTransitionToFSMDefinition(fsmDefinition, newTransition);

            return newTransition;
        }

        public static FSMState AddState(FSMDefinition fsmDefinition)
        {
            FSMState newState = CreateScriptableObject<FSMState>(fsmDefinition, "New State", "States");

            FSMEditorSaveDataChanger.AddStateToFSMDefinition(fsmDefinition, newState);

            DecideTitlesOfStateGameEvents(newState, out string enterTitle, out string updateTitle, out string exitTitle);
            GameEventScriptableObject enterEvent = CreateScriptableObject<GameEventScriptableObject>(fsmDefinition, enterTitle, "GameEvents");
            GameEventScriptableObject updateEvent = CreateScriptableObject<GameEventScriptableObject>(fsmDefinition, updateTitle, "GameEvents");
            GameEventScriptableObject exitEvent = CreateScriptableObject<GameEventScriptableObject>(fsmDefinition, exitTitle, "GameEvents");

            FSMEditorSaveDataChanger.SetGameEventsOfState(newState, enterEvent, updateEvent, exitEvent);

            return newState;
        }

        private static T CreateScriptableObject<T>(FSMDefinition fsmDefinition, string title, string subfolder) where T : ScriptableObject
        {
            string path = FSMEditorPathHelper.MakePathForScriptableObject(fsmDefinition, ref title, subfolder);
            T newScriptableObject = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(newScriptableObject, path);
            return newScriptableObject;
        }



        public static void RenameState(FSMEditorOneState stateEditor, string renameTo)
        {
            renameTo = renameTo.Trim();

            if (renameTo.Length != 0 && renameTo != stateEditor.Name)
            {
                Rename(stateEditor.State, renameTo);
            }
        }

        public static void RenameTransitionWhenRenamedState(FSMEditorOneTransition transitionEditor)
        {
            string renameTo = $"[{transitionEditor.From.Name}] to [{transitionEditor.To.Name}]";
            Rename(transitionEditor.Transition, renameTo);
        }

        public static void RenameGameEventsWhenRenamedState(FSMEditorOneState stateEditor)
        {
            DecideTitlesOfStateGameEvents(stateEditor.State, out string enterTitle, out string updateTitle, out string exitTitle);

            if (stateEditor.State.OnEnter != null)
                Rename(stateEditor.State.OnEnter, enterTitle);

            if (stateEditor.State.OnUpdate != null)
                Rename(stateEditor.State.OnUpdate, updateTitle);

            if (stateEditor.State.OnExit != null)
                Rename(stateEditor.State.OnExit, exitTitle);
        }

        public static void Rename(ScriptableObject scriptableObject, string renameTo)
        {
            string path = AssetDatabase.GetAssetPath(scriptableObject);
            Rename(path, renameTo);
        }

        public static void Rename(string path, string renameTo)
        {
            if (path == null || path.Length == 0)
            {
                throw new System.Exception($"Path is null or empty: {path}");
            }

            string folderPath = FSMEditorPathHelper.PathOfFolder(path);

            renameTo = FSMEditorPathHelper.AdjustTitleToNotAlreadyExist(renameTo, folderPath, path);

            string errorMessage = AssetDatabase.RenameAsset(path, renameTo);
            if (errorMessage.Length != 0)
            {
                Debug.Break();
                throw new System.Exception(errorMessage);
            }
        }

        private static void DecideTitlesOfStateGameEvents(FSMState state
            , out string enterTitle, out string updateTitle, out string exitTitle)
        {
            enterTitle = state.name + " {Enter}";
            updateTitle = state.name + " {Update}";
            exitTitle = state.name + " {Exit}";
        }


        



        public static bool TryDeleteParameter(FSMDefinition fsmDefinition, FSMParameter toDelete)
        {
            FSMTransition transitionWhichUsesTheParameter = FSMEditorHelper.FindTransitionUsingParameter(fsmDefinition
                , toDelete, out bool usedForMinDuration);
            if (transitionWhichUsesTheParameter == null)
            {
                string usedBy = usedForMinDuration ? "Min Duration in Prior State" : "conditions";
                Debug.LogWarning($"Cannot remove the parameter {toDelete.name} because it's used by a transition's " +
                    $"{usedBy}. The transition scriptable object is named: {transitionWhichUsesTheParameter.name}");
                return false;
            }

            Delete(toDelete);
            FSMEditorSaveDataChanger.RemoveParameterFromFSMDefinition(fsmDefinition, toDelete);
            return true;
        }

        public static void DeleteState(FSMDefinition fsmDefinition, FSMState toDelete)
        {
            Delete(toDelete);
            FSMEditorSaveDataChanger.RemoveStateFromFSMDefinition(fsmDefinition, toDelete);
        }

        public static void DeleteTransition(FSMDefinition fsmDefinition, FSMTransition toDelete)
        {
            Delete(toDelete);
            FSMEditorSaveDataChanger.RemoveTransitionFromFSMDefinition(fsmDefinition, toDelete);
        }

        public static void DeleteGameEvent(GameEventScriptableObject gameEvent)
        {
            Delete(gameEvent);
        }

        private static void Delete(ScriptableObject delete)
        {
            if (delete == null)
            {
                Debug.Break();
                throw new System.ArgumentNullException("delete");
            }

            if (!AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(delete)))
            {
                Debug.Break();
                throw new System.Exception($"The asset \"{delete.name}\" couldn't be deleted.");
            }
        }
    }
}
#endif