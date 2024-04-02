#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using TMPro;

namespace FiniteStateMachineEditor
{
    public static class FSMEditorHelper
    {
        public static Vector2 GetPositionOfState(FSMDefinition fsmDefinition, FSMState state)
        {
            if (state == null)
            {
                return fsmDefinition.EditorInfo.StateEditorForAnystatePosition;
            }
            int index = FindStateIndexInFSMDefinition(fsmDefinition, state);
            return fsmDefinition.EditorInfo.StateEditorPositions[index];
        }

        public static int FindStateIndexInFSMDefinition(FSMDefinition fsmDefinition, FSMState state)
        {
            int i = 0;
            for (; i < fsmDefinition.EditorInfo.States.Length; i++)
            {
                if (fsmDefinition.EditorInfo.States[i] == state)
                    break;
            }
            if (i >= fsmDefinition.EditorInfo.StateEditorPositions.Length)
                throw new System.Exception("Error while initializing editor for one state");
            return i;
        }

        public static int GetTransitionIndex(FSMDefinition fsmDefinition, FSMTransition transition)
        {
            for (int i = 0; i < fsmDefinition.Transitions.Length; i++)
            {
                if (fsmDefinition.Transitions[i] == transition)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetIndexOfDefaultState(FSMDefinition fsmDefinition)
        {
            for (int i = 0; i < fsmDefinition.EditorInfo.States.Length; i++)
            {
                if (fsmDefinition.EditorInfo.States[i] == fsmDefinition.DefaultState)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetStateIndex(FSMDefinition fsmDefinition, FSMState state)
        {
            for (int i = 0; i < fsmDefinition.EditorInfo.States.Length; i++)
            {
                if (fsmDefinition.EditorInfo.States[i] == state)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetParameterIndex(FSMDefinition fsmDefinition, FSMParameter parameter)
        {
            for (int i = 0; i < fsmDefinition.Parameters.Length; i++)
            {
                if (fsmDefinition.Parameters[i] == parameter)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetParameterIndexAmongstFloats(FSMDefinition fsmDefinition, FSMParameter parameter)
        {
            if (parameter.Type != FSMParameter.ParameterType.Float)
            {
                throw new System.InvalidOperationException("must be a float parameter");
            }
            int result = 0;
            for (int i = 0; i < fsmDefinition.Parameters.Length; i++)
            {
                if (fsmDefinition.Parameters[i] == parameter)
                    return result;
                if (fsmDefinition.Parameters[i].Type == FSMParameter.ParameterType.Float)
                    result++;
            }
            throw new System.InvalidOperationException("shouldnt get here, is the parameter not included in the fsmDefinition?");
        }

        public static void SetParametersInDropdown(FSMDefinition fsmDefinition, TMP_Dropdown dropdown)
        {
            FSMParameter[] parameters = fsmDefinition.Parameters;
            dropdown.options.Clear();
            for (int i = 0; i < parameters.Length; i++)
                dropdown.options.Add(new TMP_Dropdown.OptionData(parameters[i].name));
            dropdown.RefreshShownValue();
        }

        public static void SetParametersInDropdownOnlyIncludingFloats(FSMDefinition fsmDefinition, TMP_Dropdown dropdown)
        {
            FSMParameter[] parameters = fsmDefinition.Parameters;
            dropdown.options.Clear();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Type == FSMParameter.ParameterType.Float)
                    dropdown.options.Add(new TMP_Dropdown.OptionData(parameters[i].name));
            }
            dropdown.RefreshShownValue();
        }

        public static int ConvertIndexOfParameterAmongstFloatsToAmongstAllParameters(FSMDefinition fsmDefinition, int indexAmongstFloats)
        {
            int nthFloat = 0;
            for (int i = 0; i < fsmDefinition.Parameters.Length; i++)
            {
                if (fsmDefinition.Parameters[i].Type == FSMParameter.ParameterType.Float)
                {
                    if (nthFloat == indexAmongstFloats)
                        return i;
                    nthFloat++;
                }
            }
            throw new System.InvalidOperationException("didnt find the float parameter");
        }

        public static bool HasAnyFloatParameter(FSMDefinition fsmDefinition)
        {
            foreach (FSMParameter parameter in fsmDefinition.Parameters)
            {
                if (parameter.Type == FSMParameter.ParameterType.Float)
                    return true;
            }
            return false;
        }

        public static int GetConditionIndex(FSMTransition transition, FSMTransitionCondition condition)
        {
            for (int i = 0; i < transition.Conditions.Length; i++)
            {
                if (transition.Conditions[i] == condition)
                    return i;
            }
            throw new System.InvalidOperationException("couldn't find the condition in the transition");
        }

        public static FSMTransition FindTransitionUsingParameter(FSMDefinition fsmDefinition, FSMParameter parameter
            , out bool usedForMinDuration)
        {
            usedForMinDuration = false;

            FSMTransition[] transitions = fsmDefinition.Transitions;
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].ParameterForMinDurationInFrom == parameter)
                {
                    usedForMinDuration = true;
                    return transitions[i];
                }

                FSMTransitionCondition[] conditions = transitions[i].Conditions;
                for (int j = 0; j < conditions.Length; j++)
                {
                    bool parameterIsComparedToIt = conditions[i].Parameter.Type == FSMParameter.ParameterType.Float
                        && conditions[i].OtherFloatParameterToCompareTo == parameter;

                    if (conditions[i].Parameter == parameter || parameterIsComparedToIt)
                    {
                        return transitions[i];
                    }
                }
            }
            return null;
        }
    }
}
#endif