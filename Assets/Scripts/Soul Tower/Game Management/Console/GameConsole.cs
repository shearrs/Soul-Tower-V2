using Shears;
using Shears.Input;
using Shears.Logging;
using Shears.Signals;
using SoulTower.Players;
using SoulTower.Towers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public class GameConsole : PersistentProtectedSingleton<GameConsole>
    {
        #region Variables
        private const int MAX_CHARACTERS = 120;
        private const string INPUT_MAP_PATH = "Soul Tower/Game Console/GameConsole_InputMap";
        private const string UI_PATH = "Soul Tower/Game Console/Game Console UI";

        private static readonly Color ERROR_COLOR = new(0.8f, 0.1f, 0.1f);
        private static readonly IConsoleCommand[] commands = new IConsoleCommand[]
        {
            new HelpCommand(),
            new SetSpeedCommand(),
            new SetCatalystHealthCommand(),
            new DamageCatalystCommand(),
            new HealCatalystCommand(),
            new LockDefenderShieldsCommand(),
            new UnlockDefenderShieldsCommand(),
            new SetDefenderShieldsCommand()
        };
        
        private readonly List<string> previousInputs = new();
        private bool isEnabled = false;
        private int previousInputIndex = 0;
        private ManagedInputMap inputMap;
        private IManagedInput toggleInput;
        private IManagedInput previousCommandInput;
        private IManagedInput nextCommandInput;
        private Catalyst catalyst;

        internal static IReadOnlyCollection<IConsoleCommand> Commands => commands;
        internal static Catalyst Catalyst
        {
            get
            {
                if (Instance.catalyst == null)
                    Instance.catalyst = FindAnyObjectByType<Catalyst>();

                return Instance.catalyst;
            }
        }

        public static event Action Enabled;
        public static event Action Disabled;
        public static event Action<string> ConsoleTextChanged;
        public static event Action<string> InputRequested;
        #endregion

        #region Initialization
        protected override void Awake()
        {
            base.Awake();

            if (this != Instance)
                return;

            inputMap = Resources.Load<ManagedInputMap>(INPUT_MAP_PATH);

            var uiPrefab = Resources.Load<GameConsoleUI>(UI_PATH);
            var ui = Instantiate(uiPrefab);
            ui.transform.SetParent(transform);

            toggleInput = inputMap.GetInput("Toggle");
            previousCommandInput = inputMap.GetInput("Previous Command");
            nextCommandInput = inputMap.GetInput("Next Command");
        }

        private void OnEnable()
        {
            toggleInput.Performed += OnToggleInput;
            previousCommandInput.Performed += OnPreviousInput;
            nextCommandInput.Performed += OnNextInput;
        }

        private void OnDisable()
        {
            toggleInput.Performed -= OnToggleInput;
            previousCommandInput.Performed -= OnPreviousInput;
            nextCommandInput.Performed -= OnNextInput;
        }

        private void Enable()
        {
            if (isEnabled)
                return;

            SignalShuttle.Emit(new ToggleInputSignal(false));

            isEnabled = true;

            Enabled?.Invoke();
        }

        private void Disable()
        {
            if (!isEnabled)
                return;

            SignalShuttle.Emit(new ToggleInputSignal(true));

            isEnabled = false;

            Disabled?.Invoke();
        }
        #endregion

        public static void SubmitInput(string inputText) => Instance.InstSubmitInput(inputText);
        private void InstSubmitInput(string inputText)
        {
            if (inputText.Length == 0)
                return;

            ConsoleMessage(inputText);
            bool foundValidCommand = false;

            foreach (var command in commands)
            {
                if (inputText.StartsWith(command.Command))
                {
                    command.TryExecuteCommand(inputText, ConsoleMessage, ConsoleError);
                    foundValidCommand = true;

                    break;
                }
            }

            if (!foundValidCommand)
                ConsoleError($"Could not parse command '{inputText}'. Use 'help' to see a list of commands.");

            previousInputs.Add(inputText);
            previousInputIndex = previousInputs.Count;
        }

        private void OnToggleInput(ManagedInputInfo info)
        {
            if (isEnabled)
                Disable();
            else
                Enable();
        }
    
        private void OnPreviousInput(ManagedInputInfo info)
        {
            if (previousInputs.Count == 0)
                return;

            previousInputIndex = Mathf.Clamp(previousInputIndex - 1, 0, previousInputs.Count - 1);

            string input = previousInputs[previousInputIndex];
            InputRequested?.Invoke(input);
        }

        private void OnNextInput(ManagedInputInfo info)
        {
            if (previousInputIndex == previousInputs.Count)
                return;
            else if (previousInputIndex == previousInputs.Count - 1)
            {
                previousInputIndex++;
                InputRequested?.Invoke(string.Empty);

                return;
            } 

            previousInputIndex = Mathf.Clamp(previousInputIndex + 1, 0, previousInputs.Count - 1);

            string input = previousInputs[previousInputIndex];
            InputRequested?.Invoke(input);
        }

        private void ConsoleError(string text)
        {
            ConsoleMessage(text, ERROR_COLOR);
            SHLogger.Log(text, SHLogLevels.Error);
        }

        private void ConsoleMessage(string text) => ConsoleMessage(text, Color.white);

        private void ConsoleMessage(string text, Color color)
        {
            string colorString = ColorUtility.ToHtmlStringRGB(color);

            ConsoleTextChanged?.Invoke($"<color=#{colorString}>{text}</color>");
        }
    }
}
