using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UXML
{
    [UxmlElement]
    public partial class SelectableSlider : Slider, ISelectableElement
    {
        float _minValue = 0;
        float _maxValue = 10;
        float _stepSize = 1f;

        VisualElement _handleElement;

        Texture2D _backgroundImage;
        Texture2D _fillImage;
        Texture2D _handleImage;

        public event Action OnSelect;
        public event Action OnValueChanged;

        [UxmlAttribute]
        public float MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                this.lowValue = _minValue;
            }
        }

        [UxmlAttribute]
        public float MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                this.highValue = _maxValue;
            }
        }

        [UxmlAttribute]
        public float StepSize
        {
            get { return _stepSize; }
            set { _stepSize = value; }
        }

        // UXML attributes for slider elements
        [UxmlAttribute("background-image")]
        public Texture2D BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                _backgroundImage = value;
                ApplyBackgroundImage();
            }
        }

        [UxmlAttribute("fill-image")]
        public Texture2D FillImage
        {
            get { return _fillImage; }
            set
            {
                _fillImage = value;
                ApplyFillImage();
            }
        }

        [UxmlAttribute("handle-image")]
        public Texture2D HandleImage
        {
            get { return _handleImage; }
            set
            {
                _handleImage = value;
                ApplyHandleImage();
            }
        }

        // Constructor
        public SelectableSlider()
        {
            if (label == null)
                label = "selectable-slider";

            this.RegisterValueChangedCallback(evt => InvokeValueChangedAction(evt.newValue));

            this.lowValue = _minValue;
            this.highValue = _maxValue;
            this.value = (_maxValue - _minValue) / 2;

            // Apply images if available
            ApplyImages();
        }

        // UXML factory class to handle UXML attributes
        public new class UxmlTraits : Slider.UxmlTraits
        {
            // UXML descriptions for background, fill, and handle images
            UxmlAssetAttributeDescription<Texture2D> backgroundImage =
                new UxmlAssetAttributeDescription<Texture2D> { name = "background-image" };
            UxmlAssetAttributeDescription<Texture2D> fillImage =
                new UxmlAssetAttributeDescription<Texture2D> { name = "fill-image" };
            UxmlAssetAttributeDescription<Texture2D> handleImage =
                new UxmlAssetAttributeDescription<Texture2D> { name = "handle-image" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var slider = (SelectableSlider)ve;

                // Set the images from the UXML attributes
                slider.BackgroundImage = backgroundImage.GetValueFromBag(bag, cc);
                slider.FillImage = fillImage.GetValueFromBag(bag, cc);
                slider.HandleImage = handleImage.GetValueFromBag(bag, cc);

                // Apply all the images
                slider.ApplyImages();
            }
        }

        // Apply all images to the slider components
        private void ApplyImages()
        {
            ApplyBackgroundImage();
            ApplyFillImage();
            ApplyHandleImage();
        }

        // Apply the background image to the slider
        private void ApplyBackgroundImage()
        {
            if (BackgroundImage != null)
            {
                style.backgroundImage = new StyleBackground(BackgroundImage);
            }
        }

        // Apply the fill image to the slider's fill element
        private void ApplyFillImage()
        {
            var fill = this.Q<VisualElement>("unity-fill");
            if (fill != null && FillImage != null)
            {
                fill.style.backgroundImage = new StyleBackground(FillImage);
            }
        }

        // Apply the handle image to the slider's handle (thumb) element
        private void ApplyHandleImage()
        {
            _handleElement = this.Q<VisualElement>("unity-dragger");
            if (_handleElement != null && HandleImage != null)
            {
                _handleElement.style.backgroundImage = new StyleBackground(HandleImage);
            }
        }

        public void Increment()
        {
            value += _stepSize;
        }

        public void Decrement()
        {
            value -= _stepSize;
        }

        // Methods for selection
        public void Select()
        {
            _handleElement?.AddToClassList(ISelectableElement.SELECTED_CLASS);
            OnSelect?.Invoke();
        }

        public void Deselect()
        {
            _handleElement?.RemoveFromClassList(ISelectableElement.SELECTED_CLASS);
        }

        // Enable/disable methods
        public void Enable()
        {
            RemoveFromClassList(ISelectableElement.DISABLED_CLASS);
            SetEnabled(true);
        }

        public void Disable()
        {
            AddToClassList(ISelectableElement.DISABLED_CLASS);
            SetEnabled(false);
        }

        // Value change handler
        public void InvokeValueChangedAction(float newValue)
        {
            OnValueChanged?.Invoke();
        }

        // UXML factory class
        public new class UxmlFactory : UxmlFactory<SelectableSlider, UxmlTraits> { }
    }
}
