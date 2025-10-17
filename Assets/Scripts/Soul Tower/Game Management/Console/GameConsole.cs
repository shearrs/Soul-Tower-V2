using Shears;
using Shears.Input;
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

        private const string CMD_SET_SPEED = "set_speed";
        private const string CMD_SET_CATALYST_HEALTH = "set_catalyst_health";
        private const string CMD_DAMAGE_CATALYST = "damage_catalyst";
        private const string CMD_HEAL_CATALYST = "heal_catalyst";

        private static readonly Color ERROR_COLOR = new(0.8f, 0.1f, 0.1f);

        private bool isEnabled = false;
        private string inputText = string.Empty;

        private ManagedInputMap inputMap;
        private IManagedInput toggleInput;
        private Catalyst catalyst;

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
        #endregion

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

            if (inputText.StartsWith(CMD_SET_SPEED))
                SetSpeedCommand(inputText);
            else if (inputText.StartsWith(CMD_SET_CATALYST_HEALTH))
                SetCatalystHealthCommand(inputText);
            else if (inputText.StartsWith(CMD_DAMAGE_CATALYST))
                DamageCatalystCommand(inputText);
            else if (inputText.StartsWith(CMD_HEAL_CATALYST))
                HealCatalystCommand(inputText);
            else
                ConsoleError($"Could not parse command '{inputText}");

            inputText = string.Empty;
            InputTextChanged?.Invoke(inputText);
        }

        private void SetSpeedCommand(string input)
        {
            int cmdLength = CMD_SET_SPEED.Length;

            if (input.Length == cmdLength)
            {
                ConsoleError($"{CMD_SET_SPEED} requires a number value to be set");
                return;
            }
            else if (input[cmdLength] != ' ')
            {
                ConsoleError($"Could not parse command '{input}'");
                return;
            }

            try
            {
                int offset = cmdLength + 1;
                float speed = float.Parse(input[offset..]);

                if (speed > 100.0f)
                    ConsoleError($"{CMD_SET_SPEED} can only be used with values less than or equal to 100.0");
                else
                    Time.timeScale = speed;
            }
            catch (ArgumentOutOfRangeException)
            {
                ConsoleError($"{CMD_SET_SPEED} requires a number value to be set");
            }
            catch (FormatException)
            {
                ConsoleError($"Could not parse float value from {input[8..]}");
            }
        }

        private void SetCatalystHealthCommand(string input)
        {
            int cmdLength = CMD_SET_CATALYST_HEALTH.Length;

            if (input.Length == cmdLength)
            {
                ConsoleError($"{CMD_SET_CATALYST_HEALTH} requires a number value to be set");
                return;
            }
            else if (input[cmdLength] != ' ')
            {
                ConsoleError($"Could not parse command '{input}'");
                return;
            }

            try
            {
                if (catalyst == null)
                    catalyst = FindAnyObjectByType<Catalyst>();

                if (catalyst == null)
                {
                    ConsoleError("Could not find Catalyst in scene");
                    return;
                }

                int offset = cmdLength + 1;
                int health = int.Parse(input[offset..]);

                if (health > catalyst.MaxHealth)
                    ConsoleError($"{CMD_SET_CATALYST_HEALTH} can only be used with values less than or equal to 100.0");
                else
                    catalyst.SetHealth(health);
            }
            catch (ArgumentOutOfRangeException)
            {
                ConsoleError($"{CMD_SET_CATALYST_HEALTH} requires a number value to be set");
            }
            catch (FormatException)
            {
                ConsoleError($"Could not parse integer value from {input[8..]}");
            }
        }

        private void DamageCatalystCommand(string input)
        {
            int cmdLength = CMD_DAMAGE_CATALYST.Length;

            if (input.Length > cmdLength)
            {
                ConsoleError($"Could not parse ${input}, did you mean '{CMD_DAMAGE_CATALYST}'?");
                return;
            }

            if (catalyst == null)
                catalyst = FindAnyObjectByType<Catalyst>();

            if (catalyst == null)
            {
                ConsoleError("Could not find Catalyst in scene");
                return;
            }
            else if (catalyst.Health == 0)
            {
                ConsoleError("Catalyst health is already 0");
                return;
            }

            catalyst.Damage();
        }

        private void HealCatalystCommand(string input)
        {
            int cmdLength = CMD_HEAL_CATALYST.Length;

            if (input.Length > cmdLength)
            {
                ConsoleError($"Could not parse ${input}, did you mean '{CMD_HEAL_CATALYST}'?");
                return;
            }

            if (catalyst == null)
                catalyst = FindAnyObjectByType<Catalyst>();

            if (catalyst == null)
            {
                ConsoleError("Could not find Catalyst in scene");
                return;
            }
            else if (catalyst.Health >= catalyst.MaxHealth)
            {
                ConsoleError("Catalyst is already at max health");
                return;
            }

            catalyst.Heal();
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
