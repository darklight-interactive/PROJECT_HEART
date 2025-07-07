using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.Behaviour;
using Darklight.Editor;
using Ink;
using Ink.Runtime;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Darklight.Utility;
#endif

namespace Darklight.Inky
{
    /// <summary>
    ///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable game data.
    /// </summary>
    public class InkyStoryManager : MonoBehaviourSingleton<InkyStoryManager>, IUnityEditorListener
    {
        const string ASSET_PATH = "Assets/Resources/Darklight/InkyStory";

        #region ---- < STATIC_PROPERTIES > ---------------------------------
        protected static StoryIterator Iterator
        {
            get
            {
                if (Instance == null)
                {
                    Debug.LogError("InkyStoryManager: Instance is null.");
                    return null;
                }

                if (Instance._iterator == null)
                {
                    Instance._iterator = new StoryIterator();
                }
                return Instance._iterator;
            }
        }
        public static Story GlobalStory
        {
            get
            {
                if (Instance == null)
                {
                    Debug.LogError("InkyStoryManager: Instance is null.");
                    return null;
                }

                if (Instance._story == null)
                {
                    Instance._story = CreateStory(Instance._storyAsset);
                }
                return Instance._story;
            }
        }
        public static InkyStoryDataObject StoryDataObject => Instance._storyDataObject;
        public static bool IsInitialized
        {
            get => Instance._isInitialized;
            protected set => Instance._isInitialized = value;
        }
        public static StoryState CurrentState => Instance._currentStoryState;

        public static string CurrentDialogue => GlobalStory.currentText.Trim();
        public static List<Choice> CurrentChoices => GlobalStory.currentChoices;
        #endregion

        //  ---------------- [ Private Fields ] -----------------------------
        Story _story;
        StoryIterator _iterator;
        string[] _knots;
        Dictionary<string, List<string>> _stitchDitionary = new Dictionary<string, List<string>>();
        Dictionary<string, object> _variableDictionary = new Dictionary<string, object>();
        string[] _globalTags;
        Dictionary<Choice, int> _choiceMap = new Dictionary<Choice, int>();
        bool _isInitialized;
        string _currentStoryKnot;
        string _currentStoryStitch;

        //  ---------------- [ Serialized Fields ] -----------------------------
        [SerializeField]
        TextAsset _storyAsset;

        [Header("Active Story Info")]
        [SerializeField, ShowOnly]
        string _currentStoryPath;

        [SerializeField, ShowOnly]
        string _currentStoryDialogue;

        [SerializeField, ShowOnly]
        public StoryState _currentStoryState;

        [Header("Story Data Object")]
        [SerializeField, Expandable]
        InkyStoryDataObject _storyDataObject;

        #region ---- < Properties > ---------------------------------
        public List<string> KnotList
        {
            get
            {
                if (_knots == null)
                    _knots = GetAllKnots().ToArray();
                return _knots.ToList();
            }
        }

        #endregion


        #region ---- < Events > ---------------------------------
        /// <summary>
        /// A parameterless delegate that is called to indicate different story states
        /// </summary>
        public delegate void StorySimpleEvent(); // Simple Event with no parameters

        /// <summary>
        /// Delegate that is called when the story has new dialogue to present to the player.
        /// </summary>
        /// <param name="text"></param>
        public delegate void StoryDialogueEvent(string text);

        /// <summary>
        /// Delegate that is called when a new set of choices is presented to the player.
        /// </summary>
        /// <param name="choices"></param>
        public delegate void StoryChoiceEvent(List<Choice> choices);

        public static event StorySimpleEvent OnStoryInitialized;
        public static event StorySimpleEvent OnStartKnot;
        public static event StoryDialogueEvent OnNewDialogue;
        public static event StoryChoiceEvent OnNewChoices;
        public static event StorySimpleEvent OnEndKnot;
        #endregion

        void IUnityEditorListener.OnEditorReloaded()
        {
            Initialize();
        }

        #region ---- < PROTECTED_VIRTUAL_METHODS > ( Internal Data Handling ) ---------------------------------

        protected void RefreshDataObject()
        {
#if UNITY_EDITOR
            if (_storyDataObject == null)
            {
                _storyDataObject =
                    ScriptableObjectUtility.CreateOrLoadScriptableObject<InkyStoryDataObject>(
                        ASSET_PATH
                    );
            }
#endif

            // Update Knot Containers
            _storyDataObject.RepopulateKnotContainers(_stitchDitionary);
            _storyDataObject.RepopulateVariableContainers(_variableDictionary);
        }
        #endregion

        #region ---- < PROTECTED_METHODS > ( Story Event Handlers ) ---------------------------------
        protected virtual void HandleStoryError(string message, ErrorType errorType)
        {
            Debug.LogError($"{Prefix} Ink Error: {errorType} :: {message}");
        }

        protected virtual void HandleStoryInitialized()
        {
            Debug.Log($"{Prefix} Story Initialized");
        }

        protected virtual void HandleStoryStart()
        {
            Debug.Log($"{Prefix} Story Started");
        }

        protected virtual void HandleStoryDialogue(string dialogue)
        {
            _currentStoryDialogue = dialogue;
            Debug.Log($"{Prefix} Dialogue: {dialogue}");
        }

        protected virtual void HandleStoryChoices(List<Choice> choices)
        {
            Debug.Log($"{Prefix} Choices: {choices.Count}");
        }

        protected virtual void HandleStoryEnd()
        {
            Debug.Log($"{Prefix} End of Knot");
        }
        #endregion

        #region ---- < PUBLIC_METHODS > ( Interface Methods ) ---------------------------------
        // ---- ( MonoBehaviourSingleton ) ---------------------------------
        protected override void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = false;

            // << BASE CHECKS >> ------------------------------------ >>
            if (_storyAsset == null)
            {
                Debug.LogError($"{Prefix} Ink story not set.");
                return;
            }

            string initLog = $"{Prefix} Initialized";
            void AddToLog(string message)
            {
                initLog += $"\n>> {message}";
            }

            // << CREATE STORY >> ------------------------------------ >>
            this._story = CreateStory(_storyAsset);
            AddToLog($"Story: {_storyAsset.name}");

            // << GET KNOTS >> ------------------------------------ >>
            this._knots = GetAllKnots().ToArray();
            AddToLog($"Knots: {_knots.Length}");

            // << GET STITCHES >> ------------------------------------ >>
            if (_stitchDitionary == null)
                _stitchDitionary = new Dictionary<string, List<string>>();

            // Remove any knots that are not in the current story
            if (_stitchDitionary.Count > 0)
            {
                List<string> knotsToRemove = new List<string>();
                foreach (string key in _stitchDitionary.Keys)
                {
                    if (!_knots.Contains(key))
                    {
                        knotsToRemove.Add(key);
                    }
                }
                foreach (string key in knotsToRemove)
                {
                    _stitchDitionary.Remove(key);
                }
                AddToLog($"Removed Knots from _stitchDictionary: {knotsToRemove.Count}");
            }

            // Add any new knots to the dictionary
            int stitchCount = 0;
            int newKnotCount = 0;
            int updatedKnotCount = 0;
            foreach (string knot in _knots)
            {
                if (_stitchDitionary.ContainsKey(knot))
                {
                    _stitchDitionary[knot] = GetAllStitchesInKnot(knot);
                    updatedKnotCount++;
                    stitchCount += _stitchDitionary[knot].Count;
                }
                else
                {
                    _stitchDitionary.Add(knot, GetAllStitchesInKnot(knot));
                    newKnotCount++;
                    stitchCount += _stitchDitionary[knot].Count;
                }
            }
            AddToLog(
                $"Stitches: {stitchCount} (NewKnots: {newKnotCount}, UpdatedKnots: {updatedKnotCount})"
            );

            // << GET VARIABLES >> ------------------------------------ >>
            if (_variableDictionary == null)
                _variableDictionary = new Dictionary<string, object>();
            if (_variableDictionary.Count > 0)
                _variableDictionary.Clear();
            foreach (string variableName in GlobalStory.variablesState)
            {
                object variableValue = GlobalStory.variablesState[variableName];
                if (variableValue is not null)
                    _variableDictionary.Add(variableName, variableValue);
            }

            // << GET GLOBAL TAGS >> ------------------------------------ >>
            this._globalTags = new string[0];
            if (_story.globalTags != null && _story.globalTags.Count > 0)
            {
                this._globalTags = _story.globalTags.ToArray();
                AddToLog($"Global Tags: {_globalTags.Length}");
            }

            // << REFRESH DATA OBJECT >> ------------------------------------ >>
            RefreshDataObject();

            // << OBSERVE EVENTS >>
            if (Application.isPlaying)
            {
                _story.onError += HandleStoryError;
                OnStoryInitialized += HandleStoryInitialized;
                OnStartKnot += HandleStoryStart;
                OnNewDialogue += HandleStoryDialogue;
                OnNewChoices += HandleStoryChoices;
                OnEndKnot += HandleStoryEnd;
                AddToLog("Subscribed to Story Events in PlayMode");
            }

            // << CREATE ITERATOR >> ------------------------------------ >>
            _iterator = new StoryIterator();

            // << CONFIRM INITIALIZATION >> ------------------------------------ >>
            _isInitialized = true;
            if (Application.isPlaying)
                OnStoryInitialized?.Invoke();

            Debug.Log(initLog);
        }
        #endregion

        #region ---- < PUBLIC_METHODS > ( Getters ) ---------------------------------
        public void TryGetKnotsWithSubstring(string substring, out List<string> knots)
        {
            substring = substring.ToLower();
            knots = new List<string>();

            if (this._knots != null && this._knots.Length > 0)
            {
                foreach (string knot in this._knots)
                {
                    knot.ToLower();
                    if (knot.Contains(substring))
                    {
                        knots.Add(knot);
                    }
                }
            }
            else if (this._storyDataObject != null && _storyDataObject.KnotContainers.Count > 0)
            {
                foreach (StoryKnotContainer container in _storyDataObject.KnotContainers)
                {
                    if (container.Knot.Contains(substring))
                    {
                        knots.Add(container.Knot);
                    }
                }
            }
            return;
        }
        #endregion

        #region ---- < STATIC_METHODS > ( InkyStory Utility Methods ) ---------------------------------

        /// <summary>
        /// Creates an Ink story from a TextAsset.
        /// </summary>
        /// <param name="inkTextAsset">
        ///     The TextAsset containing the Ink story. Typically, this is a generated .json file.
        /// </param>
        static Story CreateStory(TextAsset inkTextAsset)
        {
            return new Story(inkTextAsset.text);
        }

        /// <summary>
        /// Retrieves all knots in an Ink story.
        /// </summary>
        /// <param name="story">
        ///     The Ink story from which to extract knots.
        /// </param>
        /// <returns>
        ///     A list of knot names.
        /// </returns>
        public static List<string> GetAllKnots()
        {
            return GlobalStory.mainContentContainer.namedContent.Keys.ToList();
        }

        /// <summary>
        /// Retrieves all variables from an Ink story and wraps them in a dictionary.
        /// </summary>
        /// <param name="story">
        ///    The Ink story from which to extract variables.
        /// </param>
        /// <returns>
        ///    A dictionary of variable names and their values.
        ///    The key is the variable name and the value is the variable value.
        ///    The variable value is an object, so it must be cast to the appropriate type.
        ///    For example, if the variable is an integer, cast it to an integer.
        /// </returns>
        public static void GetAllVariables(out Dictionary<string, object> variableDictionary)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            foreach (string variableName in GlobalStory.variablesState)
            {
                object variableValue = GlobalStory.variablesState[variableName];
                if (variableValue is not null)
                    output.Add(variableName, variableValue);
            }
            variableDictionary = output;
        }

        /// <summary>
        /// Retrieves all stitches in a knot from an Ink story.
        /// </summary>
        /// <param name="story">
        ///     The Ink story from which to extract stitches.
        /// </param>
        /// <param name="knot">
        ///     The knot from which to extract stitches.
        /// </param>
        /// <returns>
        ///     A list of stitch names.
        /// </returns>
        public static List<string> GetAllStitchesInKnot(string knot)
        {
            if (GlobalStory == null)
                return new List<string>();

            try
            {
                Container container = GlobalStory.KnotContainerWithName(knot);
                List<string> stitches = new List<string>();
                foreach (string stitch in container.namedContent.Keys.ToList())
                {
                    stitches.Add($"{knot}.{stitch}");
                }
                return stitches;
            }
            catch (Exception e)
            {
                Debug.LogError($"{Prefix} Knot: {knot} :: Error: {e.Message}");
                return new List<string>();
            }
        }

        static void RepopulateChoiceMap()
        {
            Instance._choiceMap.Clear();
            foreach (Choice choice in GlobalStory.currentChoices)
            {
                Instance._choiceMap.Add(choice, choice.index);
            }
        }

        public static void TryGetVariableValue(string variableName, out object value)
        {
            if (Instance._variableDictionary.ContainsKey(variableName))
                value = Instance._variableDictionary[variableName];
            else
                value = null;
        }

        public static void TryGetVariableContainer(
            string variableName,
            out StoryVariableContainer container
        )
        {
            if (Instance._variableDictionary.ContainsKey(variableName))
            {
                object value = Instance._variableDictionary[variableName];
                container = new StoryVariableContainer(variableName, value);
            }
            else
            {
                container = null;
            }
        }

        public static void SetVariable(string variableName, object value)
        {
            Debug.Log($"{Prefix} Set Variable: {variableName} = {value}");
            if (Instance._variableDictionary.ContainsKey(variableName))
            {
                Instance._variableDictionary[variableName] = value;
            }
            else
            {
                Instance._variableDictionary.Add(variableName, value);
            }
            Instance.RefreshDataObject();
        }

        public static void GoToPath(string path)
        {
            try
            {
                GlobalStory.ChoosePathString(path);
                GetStitchFromKnotPath(
                    path,
                    out Instance._currentStoryKnot,
                    out Instance._currentStoryStitch
                );
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{Prefix} Error: {e.Message}, this");
            }
            finally
            {
                Iterator.GoToState(StoryState.START);
                Instance._currentStoryPath = path;
                Debug.Log($"{Prefix} Moved to Path: {path}");
            }
        }

        public static void ContinueStory()
        {
            Iterator.ContinueIterator();

            Debug.Log($"{Prefix} Continue Story at {Instance._currentStoryKnot}");
        }

        public static void ChooseChoice(Choice choice) => ChooseChoice(choice.index);

        public static void ChooseChoice(int index)
        {
            TryGetChoices(out List<Choice> choices);
            Debug.Log($"{Prefix} Choice Selected: {choices[index].text}");

            GlobalStory.ChooseChoiceIndex(index);
            Instance._choiceMap.Clear();
            ContinueStory();
        }

        public static void TryGetChoices(out List<Choice> choices)
        {
            choices = GlobalStory.currentChoices;
        }

        public static void TryGetTags(out IEnumerable<string> tags)
        {
            tags = GlobalStory.currentTags;
        }

        static void GetStitchFromKnotPath(string path, out string knot, out string stitch)
        {
            if (path.Contains("."))
            {
                string[] splitPath = path.Split('.');
                knot = splitPath[0];
                stitch = splitPath[1];
            }
            else
            {
                knot = path;
                stitch = null;
            }
        }
        #endregion

        #region < CLASS > [[ STORY KNOT CONTAINER ]] ================================================================
        [Serializable]
        public class StoryKnotContainer
        {
            [SerializeField, ShowOnly]
            string _knot;

            [SerializeField, ShowOnly, NonReorderable]
            List<string> _stitches;

            public string Knot
            {
                get => _knot;
                set => _knot = value;
            }
            public List<string> Stitches
            {
                get => _stitches;
                set => _stitches = value;
            }

            public StoryKnotContainer(string knot)
            {
                _knot = knot;
                _stitches = GetAllStitchesInKnot(knot);
            }

            public StoryKnotContainer(string knot, List<string> stitches)
            {
                _knot = knot;
                _stitches = stitches;
            }

            public StoryKnotContainer(KeyValuePair<string, List<string>> pair)
            {
                _knot = pair.Key;
                _stitches = pair.Value;
            }
        }
        #endregion

        #region < CLASS > [[ STORY VARIABLE CONTAINER ]] ============================================================

        [System.Serializable]
        public class StoryVariableContainer
        {
            object _value;

            [SerializeField, ShowOnly]
            string _key;

            [SerializeField, ReadOnly]
            string _valueAsString;

            [SerializeField, ReadOnly]
            List<string> _valueAsStringList;

            public string Key
            {
                get => _key;
                set => _key = value;
            }
            public object Value
            {
                get => _value;
                set => _value = value;
            }
            public string ValueAsString => ToString();
            public List<string> ValueAsStringList => ToStringList();

            // Constructor for general use
            public StoryVariableContainer(string key, object value)
            {
                _key = key;
                _value = value;

                _valueAsString = ToString();
                _valueAsStringList = ToStringList();
            }

            public List<string> ToStringList()
            {
                if (_value is InkList inkList)
                {
                    List<string> list = new List<string>();
                    foreach (KeyValuePair<InkListItem, int> item in inkList)
                    {
                        list.Add(item.Key.ToString());
                    }
                    return list;
                }
                return new List<string> { _valueAsString };
            }

            public override string ToString()
            {
                if (_value is InkList inkList)
                {
                    return _valueAsString = inkList.ToString().Trim();
                }
                else
                {
                    return _valueAsString = _value?.ToString() ?? "null";
                }
            }
        }
        #endregion

        #region < CLASS > [[ STORY ITERATOR ]] ================================================================
        /// <summary>
        /// This class is responsible for iterating through the Ink story and handling the different states of the story.
        /// It implements a simple StoryState machine to track the current StoryState of the story.
        /// </summary>
        [Serializable]
        public class StoryIterator : SimpleStateMachine<StoryState>
        {
            // ======== [[ PROPERTIES ]] ================================ >>
            protected Story story => GlobalStory;

            // ------------------- [[ CONSTRUCTORS ]] -------------------
            public StoryIterator()
                : base(StoryState.NULL) { }

            #region ---- ( Story Handlers ) ---------------------------------
            void HandleIteratorDialogue()
            {
                GoToState(StoryState.DIALOGUE);
                story.Continue();

                // Check if empty, if so, continue again
                string currentDialogue = story.currentText.Trim();
                if (currentDialogue == null || currentDialogue == "" || currentDialogue == "\n")
                {
                    ContinueIterator();
                    return;
                }

                // Invoke the Dialogue Event
                OnNewDialogue?.Invoke(currentDialogue);
            }

            void HandleIteratorChoices()
            {
                // Return if already in choice StoryState
                if (CurrentState == StoryState.CHOICE)
                    return;

                // Go To Choice StoryState
                GoToState(StoryState.CHOICE);

                // Set the choiceMap
                RepopulateChoiceMap();

                // Invoke the Choice Event
                OnNewChoices?.Invoke(story.currentChoices);
            }

            void HandleStoryEnd()
            {
                GoToState(StoryState.END);
                OnEndKnot?.Invoke();
            }

            void HandleIteratorTags()
            {
                List<string> currentTags = story.currentTags;

                // loop through each tag and handle it accordingly
                foreach (string tag in currentTags)
                {
                    // parse the tag
                    string[] splitTag = tag.Split(':');
                    if (splitTag.Length != 2)
                    {
                        Debug.LogWarning($"{Prefix} Error: Tag is not formatted correctly: {tag}");
                    }
                    else
                    {
                        string tagKey = splitTag[0].Trim();
                        string tagValue = splitTag[1].Trim();

                        // handle the tag
                        switch (tagKey)
                        {
                            default:
                                Debug.LogWarning($"{Prefix} Tag not handled: {tag}");
                                break;
                        }
                    }
                }
            }
            #endregion

            // ======== <PUBLIC_METHODS> ================================ >>


            public void ContinueIterator()
            {
                // Check if end
                if (CurrentState == StoryState.END)
                {
                    Debug.LogWarning($"{Prefix} Knot has ended");
                    return;
                }

                // << HANDLE STORY StoryState >> ------------------------------------ >>
                // -- ( DIALOGUE StoryState ) ----
                if (story.canContinue)
                    HandleIteratorDialogue();
                // -- ( CHOICE StoryState ) ----
                else if (story.currentChoices.Count > 0)
                    HandleIteratorChoices();
                // -- ( END StoryState ) ----
                else
                    HandleStoryEnd();

                // << HANDLE TAGS >> ------------------------------------ >>
                HandleIteratorTags();
            }
        }
        #endregion

        public enum StoryState
        {
            NULL,
            START,
            DIALOGUE,
            CHOICE,
            END
        }
    }
}
