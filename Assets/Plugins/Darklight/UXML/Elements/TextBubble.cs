using Darklight.Core2D;
using Darklight.UXML;
using UnityEngine;
using UnityEngine.UIElements;

namespace Darklight.UXML
{
    [UxmlElement]
    public partial class TextBubble : ControlledLabel
    {
        const string TAG = "TextBubble";

        TextBubbleLibrary _library;
        Spatial2D.AnchorPoint _originPoint = Spatial2D.AnchorPoint.CENTER;
        Spatial2D.AnchorPoint _directionPoint = Spatial2D.AnchorPoint.CENTER;

        [Header("[TEXT_BUBBLE] ================ >>>>")]
        [UxmlAttribute]
        public TextBubbleLibrary Library
        {
            get { return _library; }
            set { _library = value; }
        }

        [UxmlAttribute]
        public Spatial2D.AnchorPoint OriginPoint
        {
            get { return _originPoint; }
            set { AlignToOriginPoint(value); }
        }

        [
            UxmlAttribute,
            Tooltip(
                "The directional anchor point of the bubble. Determines the alignment of the bubble and what sprite is used."
            )
        ]
        public Spatial2D.AnchorPoint DirectionPoint
        {
            get { return _directionPoint; }
            set { AlignToDirectionPoint(value); }
        }

        public TextBubble()
            : base()
        {
            this.style.flexGrow = 1;
            this.style.flexDirection = FlexDirection.Column;
            this.style.alignSelf = Align.Stretch;

            UpdateBubbleSprite();
        }

        public void AlignBubble(
            Spatial2D.AnchorPoint originPoint,
            Spatial2D.AnchorPoint directionPoint
        )
        {
            AlignToOriginPoint(originPoint);
            AlignToDirectionPoint(directionPoint);
        }

        public void AlignBubble() => AlignBubble(_originPoint, _directionPoint);

        void AlignToOriginPoint(Spatial2D.AnchorPoint originPoint)
        {
            _originPoint = originPoint;
            switch (originPoint)
            {
                case Spatial2D.AnchorPoint.TOP_LEFT:
                    this.style.justifyContent = Justify.FlexStart;
                    this.style.alignItems = Align.FlexStart;
                    break;
                case Spatial2D.AnchorPoint.TOP_CENTER:
                    this.style.justifyContent = Justify.FlexStart;
                    this.style.alignItems = Align.Center;
                    break;
                case Spatial2D.AnchorPoint.TOP_RIGHT:
                    this.style.justifyContent = Justify.FlexStart;
                    this.style.alignItems = Align.FlexEnd;
                    break;
                case Spatial2D.AnchorPoint.CENTER_LEFT:
                    this.style.justifyContent = Justify.Center;
                    this.style.alignItems = Align.FlexStart;
                    break;
                case Spatial2D.AnchorPoint.CENTER:
                    this.style.justifyContent = Justify.Center;
                    this.style.alignItems = Align.Center;
                    break;
                case Spatial2D.AnchorPoint.CENTER_RIGHT:
                    this.style.justifyContent = Justify.Center;
                    this.style.alignItems = Align.FlexEnd;
                    break;
                case Spatial2D.AnchorPoint.BOTTOM_LEFT:
                    this.style.justifyContent = Justify.FlexEnd;
                    this.style.alignItems = Align.FlexStart;
                    break;
                case Spatial2D.AnchorPoint.BOTTOM_CENTER:
                    this.style.justifyContent = Justify.FlexEnd;
                    this.style.alignItems = Align.Center;
                    break;
                case Spatial2D.AnchorPoint.BOTTOM_RIGHT:
                    this.style.justifyContent = Justify.FlexEnd;
                    this.style.alignItems = Align.FlexEnd;
                    break;
            }
        }

        void AlignToDirectionPoint(Spatial2D.AnchorPoint directionPoint)
        {
            _directionPoint = directionPoint;
            switch (directionPoint)
            {
                case Spatial2D.AnchorPoint.TOP_LEFT:
                case Spatial2D.AnchorPoint.CENTER_LEFT:
                case Spatial2D.AnchorPoint.BOTTOM_LEFT:
                    this.style.alignItems = Align.FlexStart;
                    break;
                case Spatial2D.AnchorPoint.TOP_CENTER:
                case Spatial2D.AnchorPoint.CENTER:
                case Spatial2D.AnchorPoint.BOTTOM_CENTER:
                    this.style.alignItems = Align.Center;
                    break;
                case Spatial2D.AnchorPoint.TOP_RIGHT:
                case Spatial2D.AnchorPoint.CENTER_RIGHT:
                case Spatial2D.AnchorPoint.BOTTOM_RIGHT:
                    this.style.alignItems = Align.FlexEnd;
                    break;
            }

            UpdateBubbleSprite();
        }

        void UpdateBubbleSprite()
        {
            if (_library != null)
            {
                if (_library.Data.ContainsKey(_directionPoint))
                {
                    Sprite sprite = _library.Data[_directionPoint];
                    BackgroundImage = sprite;
                }
            }
        }

        public void Select()
        {
            AddToClassList("selected");
            BackgroundColor = Color.yellow;
        }

        public void Deselect()
        {
            RemoveFromClassList("selected");
            BackgroundColor = Color.white;
        }
    }
}
