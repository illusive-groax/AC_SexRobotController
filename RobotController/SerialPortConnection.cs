using AC_SexRobotController.Helpers;
using AC_SexRobotController.Plugin;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace AC_SexRobotController.RobotController
{
    internal sealed class SerialPortConnection
    {
        internal SerialPort AC_SerialPort { get; set; }
        private static readonly Lazy<SerialPortConnection> _instance = new(() => new SerialPortConnection());

        private SerialPortConnection()
        {
            AC_SerialPort = new SerialPort();
        }

        internal static SerialPortConnection GetInstance()
        {
            return _instance.Value;
        }

        internal void UpdateSerialPort()
        {
            AC_SexRobotControllerPlugin.SerialPortConnected.Value = false;
            AC_SexRobotControllerPlugin.LogInfo($"Serial COM port changed to: {AC_SexRobotControllerPlugin.SerialPortConfig.Value}.");

            if (AC_SerialPort != null)
            {
                if (AC_SerialPort.IsOpen)
                {
                    try
                    {
                        // Close the serial port connection
                        AC_SerialPort.Close();
                        AC_SexRobotControllerPlugin.LogInfo($"Serial port {AC_SerialPort.PortName} has been disconnected.");
                    }
                    catch (Exception ex)
                    {
                        AC_SexRobotControllerPlugin.LogDebug("Error: " + ex.ToString());
                    }
                }
            }
            AC_SexRobotControllerPlugin.SerialPortStatus.Value = AC_SexRobotControllerPlugin.SerialPortStatus.Value + " port is disconnected.";
        }

        internal void UpdateSerialPortConnection()
        {
            // Disconnect serial port if currently connected
            if (AC_SerialPort != null)
            {
                if (AC_SerialPort.IsOpen)
                {
                    try
                    {
                        // Close the serial port connection
                        AC_SerialPort.Close();
                        AC_SexRobotControllerPlugin.SerialPortStatus.Value = AC_SerialPort.PortName + " port is disconnected.";
                        AC_SexRobotControllerPlugin.LogInfo($"Serial port {AC_SerialPort.PortName} has been disconnected.");
                        _ = UpdateDisconnectRobotButton();
                    }
                    catch (Exception ex)
                    {
                        AC_SexRobotControllerPlugin.SerialPortStatus.Value = AC_SerialPort.PortName + " port is disconnected.";
                        AC_SexRobotControllerPlugin.LogInfo($"Serial port {AC_SerialPort.PortName} has been disconnected.");
                        AC_SexRobotControllerPlugin.LogDebug("Error: " + ex.ToString());
                        _ = UpdateDisconnectRobotButton();
                    }
                }
            }

            // Connect to serial port
            if (AC_SexRobotControllerPlugin.SerialPortConnected.Value)
            {
                // Setup COM port based on updated config selection
                AC_SerialPort = new SerialPort(AC_SexRobotControllerPlugin.SerialPortConfig.Value, 115200);

                try
                {
                    // Open the serial port connection
                    AC_SerialPort.Open();

                    if (AC_SerialPort.IsOpen)
                    {

                        AC_SexRobotControllerPlugin.SerialPortConnected.Value = true;
                        AC_SexRobotControllerPlugin.SerialPortStatus.Value = "Connected to serial port " + AC_SexRobotControllerPlugin.SerialPortConfig.Value + ".";
                        AC_SexRobotControllerPlugin.LogInfo($"Connected to serial port {AC_SerialPort.PortName}.");
                    }
                    else
                    {
                        AC_SexRobotControllerPlugin.SerialPortConnected.Value = false;
                        AC_SexRobotControllerPlugin.SerialPortStatus.Value = "Error connecting to serial port " + AC_SexRobotControllerPlugin.SerialPortConfig.Value + ".";
                        AC_SexRobotControllerPlugin.LogInfo($"Error connecting to serial port {AC_SerialPort.PortName}.");
                    }
                    _ = UpdateConnectRobotButton();
                }
                catch (Exception ex)
                {
                    AC_SexRobotControllerPlugin.SerialPortConnected.Value = false;
                    AC_SexRobotControllerPlugin.SerialPortStatus.Value = "Error connecting to serial port " + AC_SexRobotControllerPlugin.SerialPortConfig.Value + ".";
                    AC_SexRobotControllerPlugin.LogDebug("Error: " + ex.ToString());
                    _ = UpdateConnectRobotButton();
                }
            }
        }

        internal void CheckButtonAndSerialConnState()
        {
            // Check if connect robot button was clicked
            if (AC_SexRobotControllerPlugin.btnConnectRobotClicked)
            {
                AC_SexRobotControllerPlugin.btnConnectRobotClicked = false;

                if (AC_SexRobotControllerPlugin.SerialPortConnected.Value)
                {
                    UpdateSerialPortConnection();
                }
                else
                {
                    AC_SexRobotControllerPlugin.SerialPortConnected.Value = true;
                }

                _ = UpdateConnectRobotButton();
            }

            // Check if disconnect robot button was clicked
            if (AC_SexRobotControllerPlugin.btnDisconnectRobotClicked)
            {
                AC_SexRobotControllerPlugin.btnDisconnectRobotClicked = false;

                if (!AC_SexRobotControllerPlugin.SerialPortConnected.Value)
                {
                    UpdateSerialPortConnection();
                }
                else
                {
                    AC_SexRobotControllerPlugin.SerialPortConnected.Value = false;
                }

                _ = UpdateDisconnectRobotButton();
            }

            // Check if increase stroke length multiplier button was clicked
            if (AC_SexRobotControllerPlugin.btnStrokeLengthMultiplierIncreaseClicked)
            {
                AC_SexRobotControllerPlugin.btnStrokeLengthMultiplierIncreaseClicked = false;
                IncreaseStrokeLength();
            }

            // Check if decrease stroke length multiplier button was clicked
            if (AC_SexRobotControllerPlugin.btnStrokLengthMultiplierDecreaseClicked)
            {
                AC_SexRobotControllerPlugin.btnStrokLengthMultiplierDecreaseClicked = false;
                DecreaseStrokeLength();
            }

            // Check if increase stroke speed multiplier button was clicked
            if (AC_SexRobotControllerPlugin.btnStrokeSpeedMultiplierIncreaseClicked)
            {
                AC_SexRobotControllerPlugin.btnStrokeSpeedMultiplierIncreaseClicked = false;
                IncreaseStrokeSpeed();
            }

            // Check if decrease stroke speed multiplier button was clicked
            if (AC_SexRobotControllerPlugin.btnStrokeSpeedMultiplierDecreaseClicked)
            {
                AC_SexRobotControllerPlugin.btnStrokeSpeedMultiplierDecreaseClicked = false;
                DecreaseStrokeSpeed();
            }

            // Check if speed limit button was clicked
            if (AC_SexRobotControllerPlugin.btnLimitRobotStrokeLengthClicked)
            {
                AC_SexRobotControllerPlugin.btnLimitRobotStrokeLengthClicked = false;

                AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_LimitStroke + !AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value);

                _ = UpdateLimitStrokeButton();
            }

            // Check if increase stroke length multiplier hotkey was pressed
            if (AC_SexRobotControllerPlugin.StrokeLengthMultiplierIncrease.Value.IsDown())
            {
                IncreaseStrokeLength();
            }


            // Check if decrease stroke length multiplier hotkey was pressed
            if (AC_SexRobotControllerPlugin.StrokeLengthMultiplierDecrease.Value.IsDown())
            {
                DecreaseStrokeLength();
            }

            // Check if increase stroke speed multiplier hotkey was pressed
            if (AC_SexRobotControllerPlugin.StrokeSpeedMultiplierIncrease.Value.IsDown())
            {
                IncreaseStrokeSpeed();
            }

            // Check if decrease stroke speed multiplier hotkey was pressed
            if (AC_SexRobotControllerPlugin.StrokeSpeedMultiplierDecrease.Value.IsDown())
            {
                DecreaseStrokeSpeed();
            }

            // Check if speed limiter hotkey was pressed
            if (AC_SexRobotControllerPlugin.TogglelimitRobotStrokeLength.Value.IsDown())
            {

                AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_LimitStroke + !AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value);

                _ = UpdateLimitStrokeButton();
            }

            // Check if serial port connection toggle hotkey was pressed and toggle the serial port on/off if so
            if (AC_SexRobotControllerPlugin.ToggleSerialPortConnection.Value.IsDown())
            {
                AC_SexRobotControllerPlugin.SerialPortConnected.Value = !AC_SexRobotControllerPlugin.SerialPortConnected.Value;
                // if the serial connection shortcut was clicked, enable the plugin if disabled
                if (AC_SexRobotControllerPlugin.DisablePlugin.Value)
                    AC_SexRobotControllerPlugin.DisablePlugin.Value = false;
            }
        }

        private async Task UpdateConnectRobotButton()
        {
            await Task.Run(async () =>
            {
                if (AC_SerialPort.IsOpen)
                {
                    AC_SexRobotControllerPlugin.buttonConnectRobotText.text = StringConstants.ButtonConnectRobot_Connected;
                }
                else
                {
                    AC_SexRobotControllerPlugin.buttonConnectRobotText.text = StringConstants.ButtonConnectRobot_NotConnected;
                }
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonConnectRobotText.text = StringConstants.ButtonConnectRobot_Text;
                AC_SexRobotControllerPlugin.buttonConnectRobotText.text = StringConstants.ButtonDisconnectRobot_Text;
            });
        }

        private async Task UpdateDisconnectRobotButton()
        {
            await Task.Run(async () =>
            {
                if (!AC_SerialPort.IsOpen)
                {
                    AC_SexRobotControllerPlugin.buttonConnectRobotText.text = StringConstants.ButtonDisconnectRobot_Disconnected;
                }
                else
                {
                    AC_SexRobotControllerPlugin.buttonConnectRobotText.text = StringConstants.ButtonDisconnectRobot_NotDisconnected;
                }
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonConnectRobotText.text = StringConstants.ButtonConnectRobot_Text;
            });
        }

        private static async Task UpdateLimitStrokeButton()
        {
            await Task.Run(async () =>
            {
                if (!AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value)
                {
                    AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value = true;
                    AC_SexRobotControllerPlugin.buttonLimitStrokeMultipliersText.text = StringConstants.ButtonLimitStrokeMultiplier_Enabled;
                }
                else
                {
                    AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value = false;
                    AC_SexRobotControllerPlugin.buttonLimitStrokeMultipliersText.text = StringConstants.ButtonLimitStrokeMultiplier_Disabled;
                }
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonLimitStrokeMultipliersText.text = StringConstants.ButtonLimitStrokeMultiplier_Text;
            });
        }

        private static async Task UpdateStrokeLengthMultiplierIncreaseButton()
        {
            await Task.Run(async () =>
            {
                float newValue = AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value
                    ? AC_SexRobotControllerPlugin.LimitRobotL0LengthMultiplier.Value
                    : AC_SexRobotControllerPlugin.RobotL0LengthMultiplier.Value;
                AC_SexRobotControllerPlugin.buttonStrokeLengthMultiplierIncreaseText.text = newValue.ToString();
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonStrokeLengthMultiplierIncreaseText.text = StringConstants.ButtonIncreaseStrokeLength_Text;
            });
        }

        private static async Task UpdateStrokeLengthMultiplierDecreaseButton()
        {
            await Task.Run(async () =>
            {
                float newValue = AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value
                    ? AC_SexRobotControllerPlugin.LimitRobotL0LengthMultiplier.Value
                    : AC_SexRobotControllerPlugin.RobotL0LengthMultiplier.Value;
                AC_SexRobotControllerPlugin.buttonStrokeLengthMultiplierDecreaseText.text = newValue.ToString();
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonStrokeLengthMultiplierDecreaseText.text = StringConstants.ButtonDecreaseStrokeLength_Text;
            });
        }

        private static async Task UpdateStrokeSpeedMultiplierIncreaseButton()
        {
            await Task.Run(async () =>
            {
                float newValue = AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value
                    ? AC_SexRobotControllerPlugin.LimitRobotL0SpeedMultiplier.Value
                    : AC_SexRobotControllerPlugin.RobotL0SpeedMultiplier.Value;
                AC_SexRobotControllerPlugin.buttonStrokeSpeedMultiplierIncreaseText.text = newValue.ToString();
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonStrokeSpeedMultiplierIncreaseText.text = StringConstants.ButtonIncreaseStrokeSpeed_Text;
            });
        }

        private static async Task UpdateStrokeSpeedMultiplierDecreaseButton()
        {
            await Task.Run(async () =>
            {
                float newValue = AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value
                    ? AC_SexRobotControllerPlugin.LimitRobotL0SpeedMultiplier.Value
                    : AC_SexRobotControllerPlugin.RobotL0SpeedMultiplier.Value;
                AC_SexRobotControllerPlugin.buttonStrokeSpeedMultiplierDecreaseText.text = newValue.ToString();
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonStrokeSpeedMultiplierDecreaseText.text = StringConstants.ButtonDecreaseStrokeSpeed_Text;
            });
        }

        private static void IncreaseStrokeLength()
        {
            float newValue;
            if (AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value)
            {
                AC_SexRobotControllerPlugin.LimitRobotL0LengthMultiplier.Value += AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;
                newValue = AC_SexRobotControllerPlugin.LimitRobotL0LengthMultiplier.Value;
            }
            else
            {
                AC_SexRobotControllerPlugin.RobotL0LengthMultiplier.Value += AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;
                newValue = AC_SexRobotControllerPlugin.RobotL0LengthMultiplier.Value;
            }

            _ = UpdateStrokeLengthMultiplierIncreaseButton();

            AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_CurrentStrokeMultiplierValue + newValue);
        }

        private static void DecreaseStrokeLength()
        {
            float newValue;
            if (AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value)
            {
                AC_SexRobotControllerPlugin.LimitRobotL0LengthMultiplier.Value -= AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;
                newValue = AC_SexRobotControllerPlugin.LimitRobotL0LengthMultiplier.Value;
            }
            else
            {
                AC_SexRobotControllerPlugin.RobotL0LengthMultiplier.Value -= AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;
                newValue = AC_SexRobotControllerPlugin.RobotL0LengthMultiplier.Value;
            }

            _ = UpdateStrokeLengthMultiplierDecreaseButton();

            AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_CurrentStrokeMultiplierValue + newValue);
        }

        private static void IncreaseStrokeSpeed()
        {
            float newValue;
            if (AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value)
            {
                AC_SexRobotControllerPlugin.LimitRobotL0SpeedMultiplier.Value += AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;
                newValue = AC_SexRobotControllerPlugin.LimitRobotL0SpeedMultiplier.Value;
            }
            else
            {
                AC_SexRobotControllerPlugin.RobotL0SpeedMultiplier.Value += AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;
                newValue = AC_SexRobotControllerPlugin.RobotL0SpeedMultiplier.Value;
            }

            _ = UpdateStrokeSpeedMultiplierIncreaseButton();

            AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_CurrentStrokeMultiplierValue + newValue);
        }

        private static void DecreaseStrokeSpeed()
        {
            float newValue;
            if (AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value)
            {
                AC_SexRobotControllerPlugin.LimitRobotL0SpeedMultiplier.Value -= AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;
                newValue = AC_SexRobotControllerPlugin.LimitRobotL0SpeedMultiplier.Value;
            }
            else
            {
                AC_SexRobotControllerPlugin.RobotL0SpeedMultiplier.Value -= AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;
                newValue = AC_SexRobotControllerPlugin.RobotL0SpeedMultiplier.Value;
            }

            _ = UpdateStrokeSpeedMultiplierDecreaseButton();

            AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_CurrentStrokeMultiplierValue + newValue);
        }
    }
}
