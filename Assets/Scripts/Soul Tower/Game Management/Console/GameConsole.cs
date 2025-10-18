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
            new HealCatalystCommand()
        };

        private bool isEnabled = false;
        private string previousInputText = string.Empty;
        private string inputText = string.Empty;

        private ManagedInputMap inputMap;
        private IManagedInput toggleInput;
        private IManagedInput previousCommandInput;
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
        public static event Action<string> InputTextChanged;
        public static event Action<string> ConsoleTextChanged;
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
        }

        private void OnEnable()
        {
            toggleInput.Performed += OnToggleInput;
            previousCommandInput.Performed += OnPreviousInput;
        }

        private void OnDisable()
        {
            toggleInput.Performed -= OnToggleInput;
            previousCommandInput.Performed -= OnPreviousInput;
            ManagedKeyboard.OnTextInput -= OnTextInput;
        }

        private void Enable()
        {
            if (isEnabled)
                return;

            SignalShuttle.Emit(new ToggleInputSignal(false));
            ManagedKeyboard.OnTextInput += OnTextInput;

            isEnabled = true;

            Enabled?.Invoke();
        }

        private void Disable()
        {
            if (!isEnabled)
                return;

            SignalShuttle.Emit(new ToggleInputSignal(true));
            ManagedKeyboard.OnTextInput -= OnTextInput;

            inputText = string.Empty;
            InputTextChanged?.Invoke(inputText);
            isEnabled = false;

            Disabled?.Invoke();
        }
        #endregion

        private void OnToggleInput(ManagedInputInfo info)
        {
            if (isEnabled)
                Disable();
            else
                Enable();
        }
    
        private void OnPreviousInput(ManagedInputInfo info)
        {
            inputText = previousInputText;
            InputTextChanged?.Invoke(inputText);
        }

        private void OnTextInput(char character)
        {
            if (!IsCharacterValid(character))
                return;

            if (character == '\b')
            {
                if (inputText.Length == 0)
                    return;
                else
                    inputText = inputText[..^1];
            }
            else if (character == '\r')
            {
                SubmitInput();
                return;
            }
            else if (inputText.Length >= MAX_CHARACTERS)
                return;
            else
                inputText += character;

            InputTextChanged?.Invoke(inputText);
        }

        private bool IsCharacterValid(char c)
        {
            return c != '`' && !(Char.IsControl(c) && !(c == '\b' || c == '\r'));
        }

        private void SubmitInput()
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

            previousInputText = inputText;
            inputText = string.Empty;
            InputTextChanged?.Invoke(inputText);
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
