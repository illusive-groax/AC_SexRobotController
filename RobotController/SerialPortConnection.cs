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
            AC_SexRobotControllerPlugin.LogInfo("Serial COM port changed to: " + AC_SexRobotControllerPlugin.SerialPortConfig.Value);

            if (AC_SerialPort != null)
            {
                if (AC_SerialPort.IsOpen)
                {
                    try
                    {
                        // Close the serial port connection
                        AC_SerialPort.Close();
                        AC_SexRobotControllerPlugin.LogInfo("Serial port " + AC_SerialPort.PortName + " has been disconnected.");
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
                        AC_SexRobotControllerPlugin.LogInfo("Serial port " + AC_SerialPort.PortName + " has been disconnected.");
                        _ = UpdateDisconnectRobotButton();
                    }
                    catch (Exception ex)
                    {
                        AC_SexRobotControllerPlugin.SerialPortStatus.Value = AC_SerialPort.PortName + " port is disconnected.";
                        AC_SexRobotControllerPlugin.LogInfo("Serial port " + AC_SerialPort.PortName + " has been disconnected.");
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
                        AC_SexRobotControllerPlugin.LogInfo("Connected to serial port " + AC_SerialPort.PortName + ".");
                    }
                    else
                    {
                        AC_SexRobotControllerPlugin.SerialPortConnected.Value = false;
                        AC_SexRobotControllerPlugin.SerialPortStatus.Value = "Error connecting to serial port " + AC_SexRobotControllerPlugin.SerialPortConfig.Value + ".";
                        AC_SexRobotControllerPlugin.LogInfo("Error connecting to serial port " + AC_SerialPort.PortName + ".");
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

        internal async Task UpdateConnectRobotButton()
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

        internal async Task UpdateDisconnectRobotButton()
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

        internal static async Task UpdateLimitRobotStrokeLengthButton()
        {
            await Task.Run(async () =>
            {
                if (!AC_SexRobotControllerPlugin.LimitRobotL0Length.Value)
                {
                    AC_SexRobotControllerPlugin.LimitRobotL0Length.Value = true;
                    AC_SexRobotControllerPlugin.buttonLimitRobotStrokeLengthText.text = StringConstants.ButtonStrokeLengthLimiter_Enabled;
                }
                else
                {
                    AC_SexRobotControllerPlugin.LimitRobotL0Length.Value = false;
                    AC_SexRobotControllerPlugin.buttonLimitRobotStrokeLengthText.text = StringConstants.ButtonStrokeLengthLimiter_Disabled;
                }
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonLimitRobotStrokeLengthText.text = StringConstants.ButtonStrokeLengthLimiter_Text;
            });
        }

        internal static async Task UpdateStrokeMultiplierIncreaseButton()
        {
            await Task.Run(async () =>
            {
                AC_SexRobotControllerPlugin.buttonStrokeMultiplierIncreaseText.text = AC_SexRobotControllerPlugin.RobotL0Multiplier.Value.ToString();
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonStrokeMultiplierIncreaseText.text = StringConstants.ButtonIncreaseStrokeLength_Text;
            });
        }

        internal static async Task UpdateStrokeMultiplierDecreaseButton()
        {
            await Task.Run(async () =>
            {
                AC_SexRobotControllerPlugin.buttonStrokeMultiplierDecreaseText.text = AC_SexRobotControllerPlugin.RobotL0Multiplier.Value.ToString();
                await Task.Delay(1000);
                AC_SexRobotControllerPlugin.buttonStrokeMultiplierDecreaseText.text = StringConstants.ButtonDecreaseStrokeLength_Text;
            });
        }

        internal void CheckButtonAndSerialConnState()
        {
            // Check if connect robot button was clicked
            if (AC_SexRobotControllerPlugin.buttonConnectRobotClicked)
            {
                AC_SexRobotControllerPlugin.buttonConnectRobotClicked = false;

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

            // Check if connect robot button was clicked
            if (AC_SexRobotControllerPlugin.buttonDisconnectRobotClicked)
            {
                AC_SexRobotControllerPlugin.buttonDisconnectRobotClicked = false;

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

            // Check if increase stroke multiplier button was clicked
            if (AC_SexRobotControllerPlugin.buttonStrokeMultiplierIncreaseClicked)
            {
                AC_SexRobotControllerPlugin.buttonStrokeMultiplierIncreaseClicked = false;

                AC_SexRobotControllerPlugin.RobotL0Multiplier.Value += AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;

                _ = UpdateStrokeMultiplierIncreaseButton();

                AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_CurrentStrokeMultiplierValue + AC_SexRobotControllerPlugin.RobotL0Multiplier.Value);
            }

            // Check if decrease stroke multiplier button was clicked
            if (AC_SexRobotControllerPlugin.buttonStrokeMultiplierDecreaseClicked)
            {
                AC_SexRobotControllerPlugin.buttonStrokeMultiplierDecreaseClicked = false;

                AC_SexRobotControllerPlugin.RobotL0Multiplier.Value -= AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;

                _ = UpdateStrokeMultiplierDecreaseButton();

                AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_CurrentStrokeMultiplierValue + AC_SexRobotControllerPlugin.RobotL0Multiplier.Value);
            }

            // Check if speed limit button was clicked
            if (AC_SexRobotControllerPlugin.buttonLimitRobotStrokeLengthClicked)
            {
                AC_SexRobotControllerPlugin.buttonLimitRobotStrokeLengthClicked = false;

                AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_SpeedLimited + !AC_SexRobotControllerPlugin.LimitRobotL0Length.Value);

                _ = UpdateLimitRobotStrokeLengthButton();
            }

            // Check if increase stroke multiplier hotkey was pressed
            if (AC_SexRobotControllerPlugin.StrokeLengthMultiplierIncrease.Value.IsDown())
            {
                AC_SexRobotControllerPlugin.RobotL0Multiplier.Value += AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;
                _ = UpdateStrokeMultiplierIncreaseButton();

                AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_CurrentStrokeMultiplierValue + AC_SexRobotControllerPlugin.RobotL0Multiplier.Value);
            }


            // Check if decrease stroke multiplier hotkey was pressed
            if (AC_SexRobotControllerPlugin.StrokeLengthMultiplierDecrease.Value.IsDown())
            {
                AC_SexRobotControllerPlugin.RobotL0Multiplier.Value -= AC_SexRobotControllerPlugin.RobotL0MultiplierStepValue.Value;

                _ = UpdateStrokeMultiplierDecreaseButton();

                AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_CurrentStrokeMultiplierValue + AC_SexRobotControllerPlugin.RobotL0Multiplier.Value);
            }

            // Check if speed limiter hotkey was pressed
            if (AC_SexRobotControllerPlugin.TogglelimitRobotStrokeLength.Value.IsDown())
            {

                AC_SexRobotControllerPlugin.LogInfo(StringConstants.Status_SpeedLimited + !AC_SexRobotControllerPlugin.LimitRobotL0Length.Value);

                _ = UpdateLimitRobotStrokeLengthButton();
            }

            // Check if serial port connection toggle hotkey was pressed and toggle the serial port on/off if so
            if (AC_SexRobotControllerPlugin.ToggleSerialPortConnection.Value.IsDown())
            {
                AC_SexRobotControllerPlugin.SerialPortConnected.Value = !AC_SexRobotControllerPlugin.SerialPortConnected.Value;
            }
        }
    }
}
