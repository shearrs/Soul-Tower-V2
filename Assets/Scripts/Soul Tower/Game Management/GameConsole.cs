using Shears;
using Shears.Input;
using Shears.Signals;
using SoulTower.Players;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public class GameConsole : PersistentProtectedSingleton<GameConsole>
    {
        private const int MAX_CHARACTERS = 120;
        private const string INPUT_MAP_PATH = "Soul Tower/Game Console/GameConsole_InputMap";
        private const string UI_PATH = "Soul Tower/Game Console/Game Console UI";

        private static readonly Color ERROR_COLOR = new(0.8f, 0.1f, 0.1f);

        private bool isEnabled = false;
        private string inputText = string.Empty;

        private ManagedInputMap inputMap;
        private IManagedInput toggleInput;

        private readonly List<ManagedKey> keys = new();

        public static event Action Enabled;
        public static event Action Disabled;
        public static event Action<string> InputTextChanged;
        public static event Action<string> ConsoleTextChanged;

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
        }

        private void OnEnable()
        {
            toggleInput.Performed += OnToggleInput;
        }

        private void OnDisable()
        {
            toggleInput.Performed -= OnToggleInput;
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

        private void OnToggleInput(ManagedInputInfo info)
        {
            if (isEnabled)
                Disable();
            else
                Enable();
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

            if (inputText.StartsWith("set_speed"))
            {
                if (inputText.Length == 9)
                {
                    ConsoleError("set_speed requires a number value to be set");
                }
                else if (inputText[9] != ' ')
                {
                    ConsoleError($"Could not parse command '{inputText}'");
                }
                else
                {
                    try
                    {
                        float speed = float.Parse(inputText[10..]);

                        if (speed > 100.0f)
                            ConsoleError("set_speed can only be used with values less than or equal to 100.0");
                        else
                            Time.timeScale = speed;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        ConsoleError("set_speed requires a number value to be set");
                    }
                    catch (FormatException)
                    {
                        ConsoleError($"Could not parse number value from {inputText[8..]}");
                    }
                }
            }
            else
                ConsoleError($"Could not parse command '{inputText}");

            inputText = string.Empty;
            InputTextChanged?.Invoke(inputText);
        }

        private void ConsoleError(string text)
        {
            ConsoleMessage(text, ERROR_COLOR);
        }

        private void ConsoleMessage(string text, Color? color = null)
        {
            string colorString;

            if (color != null)
                colorString = ColorUtility.ToHtmlStringRGB(color.Value);
            else
                colorString = ColorUtility.ToHtmlStringRGB(Color.white);

            ConsoleTextChanged?.Invoke($"<color=#{colorString}>{text}</color>");
        }
    }
}
