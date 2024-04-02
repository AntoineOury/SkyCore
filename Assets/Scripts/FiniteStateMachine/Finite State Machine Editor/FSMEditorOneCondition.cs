#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FiniteStateMachine;

namespace FiniteStateMachineEditor
{
    public class FSMEditorOneCondition : MonoBehaviour
    {
        [SerializeField]
        private GameObject _boolRequirement;
        [SerializeField]
        private GameObject _floatRequirements;
        [SerializeField]
        private TMP_Dropdown _parameterSelectionDropdown;
        [SerializeField]
        private TMP_Dropdown _boolRequirementDropdown;
        [SerializeField]
        private TMP_Dropdown _floatComparisonDropdown;
        [SerializeField]
        private TMP_InputField _floatComparedToInputField;
        [SerializeField]
        private TMP_Dropdown _comparedToParameterSelectionDropdown;
        [SerializeField]
        private TMP_Dropdown _comparedToParameterOrValueDropdown;
        [SerializeField]
        private Image _backgroundImage;
        [SerializeField]
        private Toggle _toggle;
        [SerializeField]
        private Color _selectedColor;

        private Color _normalColor;
        private FSMDefinition _fsmDefinition;
        private FSMEditor _fsmEditor;

        public FSMTransitionCondition Condition { get; private set; }


        public void Initialize(FSMTransitionCondition condition, FSMDefinition fsmDefinition, ToggleGroup toggleGroup
            , FSMEditor fsmEditor)
        {
            Condition = condition;
            _fsmDefinition = fsmDefinition;
            _fsmEditor = fsmEditor;

            _normalColor = _backgroundImage.color;
            _toggle.group = toggleGroup;

            UpdateParameterDropdownsAndCondition();
        }

        public void UpdateParameterDropdownsAndCondition()
        {
            FSMEditorHelper.SetParametersInDropdown(_fsmDefinition, _parameterSelectionDropdown);
            FSMEditorHelper.SetParametersInDropdownOnlyIncludingFloats(_fsmDefinition, _comparedToParameterSelectionDropdown);
            UpdateDisplayedCondition();
        }

        private void UpdateDisplayedCondition()
        {
            _parameterSelectionDropdown.SetValueWithoutNotify(FSMEditorHelper.GetParameterIndex(_fsmDefinition, Condition.Parameter));

            _boolRequirementDropdown.SetValueWithoutNotify(Condition.EqualsForBoolParameter ? 1 : 0);
            _floatComparisonDropdown.SetValueWithoutNotify((int)Condition.ComparisonTypeForFloatParameter);

            bool compareToOtherParameter = Condition.OtherFloatParameterToCompareTo != null;
            _comparedToParameterOrValueDropdown.SetValueWithoutNotify(compareToOtherParameter ? 1 : 0);
            if (compareToOtherParameter)
            {
                _comparedToParameterSelectionDropdown.SetValueWithoutNotify(
                    FSMEditorHelper.GetParameterIndexAmongstFloats(_fsmDefinition, Condition.OtherFloatParameterToCompareTo));
            }

            _boolRequirement.SetActive(Condition.Parameter.Type == FSMParameter.ParameterType.Bool);
            _floatRequirements.SetActive(Condition.Parameter.Type == FSMParameter.ParameterType.Float);
            _comparedToParameterSelectionDropdown.gameObject.SetActive(compareToOtherParameter);
            _floatComparedToInputField.gameObject.SetActive(!compareToOtherParameter);

            _floatComparedToInputField.SetTextWithoutNotify("" + Condition.ComparedToForFloatParameter);
        }

       

        public void OnConditionParameterDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionParameterDropdownChange(this, changeTo);
            UpdateDisplayedCondition();
        }
        public void OnConditionBoolRequirementDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionBoolRequirementDropdownChange(this, changeTo == 1 ? true : false);
            UpdateDisplayedCondition();
        }
        public void OnConditionFloatComparisonTypeDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionFloatComparisonTypeDropdownChange(this, changeTo);
            UpdateDisplayedCondition();
        }

        public void OnConditionInputFieldFloatValueToCompareToChange(string changeTo)
        {
            if (!float.TryParse(changeTo, out float asFloat))
                return;
            _fsmEditor.OnConditionInputFieldFloatValueToCompareToChange(this, asFloat);
            UpdateDisplayedCondition();
        }

        public void OnConditionFloatParameterToCompareToDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionFloatParameterToCompareToDropdownChange(this, changeTo);
            UpdateDisplayedCondition();
        }

        public void OnConditionCompareToValueOrParameterModeDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionCompareToValueOrParameterModeDropdownChange(this, changeTo, _comparedToParameterOrValueDropdown);
            UpdateDisplayedCondition();
        }

        public void OnSelectedToggle(bool toggleTo)
        {
            _backgroundImage.color = toggleTo ? _selectedColor : _normalColor;
            _fsmEditor.OnToggleSelectedConditionEditor(this, toggleTo);
        }
    }
}
#endif