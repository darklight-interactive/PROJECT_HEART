using System.Collections.Generic;
using Darklight.Collections;
using Darklight.Core2D;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UXML
{
    [CreateAssetMenu(menuName = "MeetTheRookie/Library/DialogueBubbleLibrary")]
    public class TextBubbleLibrary : ScriptableObject
    {
        [SerializeField]
        CollectionDictionary<Spatial2D.AnchorPoint, Sprite> _dataLibrary;

        public CollectionDictionary<Spatial2D.AnchorPoint, Sprite> Data
        {
            get
            {
                if (_dataLibrary == null)
                {
                    _dataLibrary = new CollectionDictionary<Spatial2D.AnchorPoint, Sprite>(
                        new List<Spatial2D.AnchorPoint>
                        {
                            Spatial2D.AnchorPoint.TOP_LEFT,
                            Spatial2D.AnchorPoint.TOP_CENTER,
                            Spatial2D.AnchorPoint.TOP_RIGHT,
                            Spatial2D.AnchorPoint.BOTTOM_LEFT,
                            Spatial2D.AnchorPoint.BOTTOM_CENTER,
                            Spatial2D.AnchorPoint.BOTTOM_RIGHT
                        }
                    );
                }

                return _dataLibrary;
            }
        }
    }
}
