using System.Data;
using Darklight.Editor;
using Darklight.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UXML
{
    [UxmlElement]
    public partial class ControlledLabel : VisualElement
    {
        const string TAG = "controlledLabel";
        const string PATH_TO_DEFAULTBKG =
            "Assets/Plugins/Darklight/_textures/DRKL_TextBubble_Default_0.png";

        VisualElement _labelContainer;
        Label _label;
        Sprite _backgroundImage;

        string _fullText =
            "New UXML Element Controlled Label. This is a test string to see how the text wraps around the bubble. Hopefully it works well.";
        float _rollingTextPercentValue = 1;
        int _currentIndex;
        int _fontSizePercent = 100;
        int _padding = 32;

        protected VisualElement labelContainer
        {
            get { return _labelContainer; }
            set { _labelContainer = value; }
        }
        protected Label label
        {
            get { return _label; }
            set { _label = value; }
        }

        #region ======== [[ PROPERTIES ]] ================================== >>>>

        [Header("[CONTROLLED_LABEL] ================ >>>>")]
        [UxmlAttribute, TextArea(3, 10)]
        public string FullText
        {
            get { return _fullText; }
            set { SetFullText(value); }
        }

        [UxmlAttribute, ShowOnly]
        public string CurrentText
        {
            get { return label.text; }
            set { label.text = value; }
        }

        [UxmlAttribute, Range(0, 1)]
        public float RollingTextPercentage
        {
            get { return _rollingTextPercentValue; }
            set
            {
                _rollingTextPercentValue = value;
                _currentIndex = Mathf.FloorToInt(FullText.Length * _rollingTextPercentValue);
                SetTextToIndex(_currentIndex);
            }
        }

        [Header("(( Text Style )) ---- >>")]
        [UxmlAttribute, Range(0, 200)]
        public int FontSizePercentage
        {
            get { return _fontSizePercent; }
            set
            {
                _fontSizePercent = value;
                label.style.fontSize = Length.Percent(_fontSizePercent);
            }
        }

        [UxmlAttribute]
        public TextAnchor TextAlign
        {
            get { return label.style.unityTextAlign.value; }
            set { label.style.unityTextAlign = value; }
        }

        [UxmlAttribute]
        public int Padding
        {
            get { return _padding; }
            set
            {
                _padding = value;
                _labelContainer.style.paddingTop = _padding;
                _labelContainer.style.paddingBottom = _padding;
                _labelContainer.style.paddingLeft = _padding;
                _labelContainer.style.paddingRight = _padding;
            }
        }

        [Header("(( Background Image )) ---- >>")]
        [UxmlAttribute]
        public Sprite BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                _backgroundImage = value;
                _labelContainer.style.backgroundImage = new StyleBackground(_backgroundImage);
            }
        }

        [UxmlAttribute]
        public Color BackgroundColor
        {
            get { return _labelContainer.style.unityBackgroundImageTintColor.value; }
            set { _labelContainer.style.unityBackgroundImageTintColor = value; }
        }
        #endregion

        // ======== [[ CONSTRUCTORS ]] =============================== >>>>
        public ControlledLabel()
        {
            _labelContainer = new VisualElement
            {
                name = $"{TAG}-container",
                style =
                {
                    overflow = Overflow.Hidden,
                    alignSelf = Align.Stretch,
                    flexWrap = Wrap.Wrap,

                    unityBackgroundImageTintColor = Color.white
                }
            };

            label = new Label
            {
                name = $"{TAG}-label",
                text = FullText,
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                    overflow = Overflow.Hidden,
                    alignSelf = Align.Stretch,

                    fontSize = Length.Percent(100),
                }
            };

            // Add the label to the container
            _labelContainer.Add(label);
            this.Add(_labelContainer);

            label.style.fontSize = _fontSizePercent;
        }

        public void SetFullText(string text)
        {
            this._fullText = text;
            SetTextToIndex(0);
        }

        public void RollingTextStep()
        {
            SetTextToIndex(_currentIndex + 1);
        }

        void SetTextToIndex(int index)
        {
            int ind = Mathf.Min(index, FullText.Length);

            // Skip over any HTML tags
            if (FullText.Length > ind && FullText[ind] == '<')
            {
                while (ind < FullText.Length && FullText[ind] != '>')
                {
                    ind++;
                }
                ind++; // Move past the '>'
            }

            // Ensure _currentIndex stays within bounds
            _currentIndex = Mathf.Min(ind, FullText.Length);
            this.CurrentText = FullText.Substring(0, _currentIndex);
        }

        public void InstantCompleteText()
        {
            SetTextToIndex(FullText.Length);
        }

        public class ControlledSizeLabelFactory : UxmlFactory<ControlledLabel> { }
    }
}
