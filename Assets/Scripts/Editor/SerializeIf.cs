using UnityEditor;
using UnityEngine;
using System;

/// <summary>
/// 
/// Stolen and adapted from https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// 
/// </summary>


[CustomPropertyDrawer(typeof(SerializeIfAttribute))]
public class DrawIfPropertyDrawer : PropertyDrawer {
    // Reference to the attribute on the property.
    SerializeIfAttribute drawIf;

    // Field that is being compared.
    SerializedProperty comparedField;

    // Height of the property.
    private float propertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return propertyHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Set the global variables.
        drawIf = attribute as SerializeIfAttribute;
        comparedField = property.serializedObject.FindProperty(drawIf.comparedPropertyName);

        if(comparedField == null) {
            Debug.LogError("[SERIALIZEIF] Could NOT find property \"" + drawIf.comparedPropertyName + "\" for "+property.serializedObject.GetType().Name+".");
            return;
		}

        // Is the condition met? Should the field be drawn?
        bool conditionMet = false;

        if(drawIf.comparisonType == ComparisonType.Boolean) {
            bool field = comparedField.boolValue;
            bool attr = (bool) drawIf.comparedValue;
            conditionMet = (field == attr);
        } else {
            // Get the value of the compared field.
            int comparedFieldValue;// .GetValue<object>();
            int comparedAttributeValue;

            try {
                comparedFieldValue = comparedField.intValue;
            } catch(Exception) {
                Debug.LogWarning("[SERIALIZEIF] Could not get int value from field " + comparedField.name + ".");
                return;
            }
            try {
                comparedAttributeValue = (int) drawIf.comparedValue;
            } catch(Exception) {
                Debug.LogWarning("[SERIALIZEIF] Could not get int value from compared value insinde the attribute.");
                return;
            }


            // Compare the values to see if the condition is met.
            switch(drawIf.comparisonType) {
                case ComparisonType.Equals:
                    conditionMet = comparedFieldValue == comparedAttributeValue;
                    break;

                case ComparisonType.NotEqual:
                    conditionMet = comparedFieldValue != comparedAttributeValue;
                    break;

                case ComparisonType.GreaterThan:
                    conditionMet = comparedFieldValue > comparedAttributeValue;
                    break;

                case ComparisonType.SmallerThan:
                    conditionMet = comparedFieldValue < comparedAttributeValue;
                    break;

                case ComparisonType.SmallerOrEqual:
                    conditionMet = comparedFieldValue <= comparedAttributeValue;
                    break;

                case ComparisonType.GreaterOrEqual:
                    conditionMet = comparedFieldValue >= comparedAttributeValue;
                    break;
            }
        }

        

        // The height of the property should be defaulted to the default height.
        propertyHeight = base.GetPropertyHeight(property, label);

        // If the condition is met, simply draw the field. Else...
        if(conditionMet) {
            EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
        } else {
            //...check if the disabling type is read only. If it is, draw it disabled, else, set the height to zero.
            if(drawIf.disablingType == DisablingType.ReadOnly) {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property);
                GUI.enabled = true;
            } else {
                propertyHeight = 0f;
            }
        }
    }
}
