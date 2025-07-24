using System.Collections;
using System.Linq;
using Darklight.Core2D;
using Darklight.Editor;
using Darklight.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UXML
{
    public class TextBubbleObject : UXML_RenderTextureObject
    {
        TextBubble _textBubble;
        string _currText;

        [SerializeField, Required]
        [CreateAsset("NewTextBubbleLibrary", "Assets/Resources/Darklight/UXML/TextBubble")]
        TextBubbleLibrary _textBubbleLibrary;

        [Header("Bubble Alignment")]
        public Spatial2D.AnchorPoint originPoint;
        public Spatial2D.AnchorPoint directionPoint;
        public int padding = 32;

        [Header("Text Values")]
        public int fontSize = 32;

        [Range(0, 1)]
        public float rollingTextPercentage;

        private bool _isTransitioning = false;

        public override void Initialize()
        {
            base.Initialize();

            Root.style.flexGrow = 1;
            Root.style.flexDirection = FlexDirection.Column;
            Root.style.alignSelf = Align.Stretch;

            _textBubble = ElementQuery<TextBubble>();
            _textBubble.Library = _textBubbleLibrary;
            _textBubble.AlignBubble(originPoint, directionPoint);
            _textBubble.Padding = padding;

            _textBubble.FontSize = fontSize;
            _textBubble.RollingTextPercentage = rollingTextPercentage;

            // Register for text change event
            _textBubble.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                if (_currText != evt.newValue)
                {
                    _currText = evt.newValue;
                }
            });

            // Register for geometry changes (size/layout)
            _textBubble.RegisterCallback<GeometryChangedEvent>(evt => { });

            // Register for animation events
            _textBubble.RegisterCallback<TransitionRunEvent>(evt =>
            {
                _isTransitioning = true;
            });

            _textBubble.RegisterCallback<TransitionEndEvent>(evt =>
            {
                _isTransitioning = false;
            });
        }

        public void SetText(string text)
        {
            if (_currText != text)
            {
                _currText = text;
                _textBubble.SetFullText(text);
                _textBubble.InstantCompleteText();
            }
        }

        public void Select()
        {
            _textBubble.Select();
        }

        public void Deselect()
        {
            _textBubble.Deselect();
        }

        /*
        #if UNITY_EDITOR
            [CustomEditor(typeof(TextBubbleObject))]
            public class TextBubbleObjectCustomEditor : UXML_RenderTextureObjectCustomEditor
            {
                public override void OnInspectorGUI()
                {
                    serializedObject.Update();
    
                    EditorGUI.BeginChangeCheck();
    
                    base.OnInspectorGUI();
    
                    if (GUILayout.Button("Select"))
                    {
                        (target as TextBubbleObject).Select();
                    }
    
                    if (GUILayout.Button("Deselect"))
                    {
                        (target as TextBubbleObject).Deselect();
                    }
    
                    if (GUILayout.Button("SetOriginToBottom"))
                    {
                        (target as TextBubbleObject)._textBubble.OriginPoint = Spatial2D.AnchorPoint.BOTTOM_CENTER;
                    }
    
                    if (GUILayout.Button("SetOriginToTop"))
                    {
                        (target as TextBubbleObject)._textBubble.OriginPoint = Spatial2D.AnchorPoint.TOP_CENTER;
                    }
    
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        #endif
            */
    }
}
