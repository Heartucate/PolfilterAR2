// MicrocontrollerManager.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using System.Text;
using TMPro;
using System;
using System.Collections.Generic;

namespace Heartucate.TUD
{
    public class MicrocontrollerManager : MonoBehaviour
    {
        public static MicrocontrollerManager Instance;
        public static string DeviceName = "ESP32_01";
        public string baseUUID = "12345678-1234-5678-1234-56789abc";//missing last 4 digits
        public string serviceUUIDSuffix = "00";
        public string readUUIDSuffix = "01";
        public string writeUUIDSuffix = "02";

        public TextMeshProUGUI StatusText;
        public TextMeshProUGUI TextReceived; // UI text field to display received strings

        // New: notify subscribers when either status or received text changes
        public static Action<string> OnTextOrStatusChanged;

        enum States
        {
            None,
            Scan,
            ScanRSSI,
            ReadRSSI,
            Connect,
            RequestMTU,
            Subscribe,
            Unsubscribe,
            Disconnect,
        }

        private bool _connected = false;
        private float _timeout = 0f;
        private States _state = States.None;
        private string _deviceAddress;
        private bool _foundCharacteristic = false;
        private bool _rssiOnly = false;
        private int _rssi = 0;

        // HM10 specifics
        private const string HM10ServiceUUID = "FFE0";
        private const string HM10CharacteristicUUID = "FFE1";
        private readonly List<byte> _receiveBuffer = new List<byte>();
        private readonly object _bufferLock = new object();

        // Store discovered UUIDs exactly as reported by the platform (used for writes/subscribes)
        private string _discoveredServiceUUID;
        private string _discoveredCharacteristicUUID;

        // Helper: decide whether to use HM10 API (if name does NOT start with "ESP32_")
        private bool UseHM10Api()
        {
            return !DeviceName.StartsWith("ESP32_", StringComparison.OrdinalIgnoreCase);
        }

        // Central helper that sets the status text and notifies subscribers
        private void UpdateStatusText(string value)
        {
            BluetoothLEHardwareInterface.Log(value);
            if (StatusText != null)
            {
                StatusText.text = value;
                OnTextOrStatusChanged?.Invoke(value);
            }
        }

        private string StatusMessage
        {
            set
            {
                // use centralized updater so every change invokes the event
                UpdateStatusText(value);
            }
        }

        void Reset()
        {
            _connected = false;
            _timeout = 0f;
            _state = States.None;
            _deviceAddress = null;
            _foundCharacteristic = false;
            _rssi = 0;
            lock (_bufferLock)
                _receiveBuffer.Clear();

            _discoveredServiceUUID = null;
            _discoveredCharacteristicUUID = null;
        }

        void SetState(States newState, float timeout)
        {
            _state = newState;
            _timeout = timeout;
        }

        void StartProcess()
        {
            Reset();
            BluetoothLEHardwareInterface.Initialize(true, false, () =>
            {

                SetState(States.Scan, 0.1f);

            }, (error) =>
            {

                StatusMessage = "Error during initialize: " + error;
            });
        }

        private void Awake()
        {
            Instance = this;
        }

        // Use this for initialization
        void Start()
        {
            StartProcess();
        }

        // Public helper to start scanning for the current DeviceName (can be wired to a UI button)
        public void StartScanForCurrentDevice()
        {
            BluetoothLEHardwareInterface.StopScan();
            _deviceAddress = null;
            _foundCharacteristic = false;
            // Kick the state machine into Scan so Update() will start scanning
            SetState(States.Scan, 0.1f);
            StatusMessage = "Started scan for: " + DeviceName;
            BluetoothLEHardwareInterface.Log("StartScanForCurrentDevice called. Looking for: " + DeviceName);
        }

        enum ServiceUUIDType
        {
            Service,
            ReadCharacteristic,
            WriteCharacteristic
        }

        [ContextMenu("Print UUIDs")]
        public void PrintUUIDs()
        {
            Debug.Log("Service UUID: " + GetUUID(ServiceUUIDType.Service));
            Debug.Log("Read Characteristic UUID: " + GetUUID(ServiceUUIDType.ReadCharacteristic));
            Debug.Log("Write Characteristic UUID: " + GetUUID(ServiceUUIDType.WriteCharacteristic));
        }

        string GetUUID(ServiceUUIDType type)
        {
            string suffix = DeviceName.Substring(DeviceName.Length - 2);

            switch (type)
            {
                case ServiceUUIDType.Service:
                    return baseUUID + serviceUUIDSuffix + suffix;
                case ServiceUUIDType.ReadCharacteristic:
                    return baseUUID + readUUIDSuffix + suffix;
                case ServiceUUIDType.WriteCharacteristic:
                    return baseUUID + writeUUIDSuffix + suffix;
                default:
                    return baseUUID + serviceUUIDSuffix + suffix;
            }
        }

        // Generic handler for incoming data - treat all payloads as UTF-8 text
        private void ProcessData(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return;

            ProcessText(bytes);
        }

        // decode UTF-8 bytes and display in TextReceived
        private void ProcessText(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return;

            try
            {
                string text = Encoding.UTF8.GetString(bytes).Trim('\0');
                if (TextReceived != null)
                {
                    TextReceived.text = text;
                    OnTextOrStatusChanged?.Invoke(text);
                }

                // use centralized updater so subscribers are always notified
                UpdateStatusText("Received text: " + text);

                // Notify any subscribers in RealtimeDataProcessor
                RealtimeDataProcessor.OnTextReceived?.Invoke(text);
            }
            catch
            {
                BluetoothLEHardwareInterface.Log("Failed to decode text");
                if (TextReceived != null)
                {
                    TextReceived.text = "<invalid>";
                    OnTextOrStatusChanged?.Invoke(TextReceived.text);
                }
            }
        }

        public static bool SendResetCommand()
        {
            if (Instance == null)
            {
                Debug.LogWarning("MicrocontrollerManager instance is null. Cannot send reset command.");
                return false;
            }

            // require actual connected state before reporting success
            if (!Instance._connected || string.IsNullOrEmpty(Instance._deviceAddress))
            {
                Debug.LogWarning("MicrocontrollerManager: not connected. Cannot send reset command.");
                return false;
            }

            // send exact same payload as old test
            Instance.SendString("Reset");
            return true;
        }

        public static bool StartScan()
        {
            if (Instance == null)
            {
                Debug.LogWarning("MicrocontrollerManager instance is null. Cannot start scan.");
                return false;
            }

            Instance.StartScanForCurrentDevice();
            return true;
        }

        // send UTF-8 string over the combined characteristic
        public void SendString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                BluetoothLEHardwareInterface.Log("SendString: text is empty");
                return;
            }

            if (!_connected || string.IsNullOrEmpty(_deviceAddress))
            {
                BluetoothLEHardwareInterface.Log("SendString: not connected");
                return;
            }

            text += "\n"; // append newline to match typical line-based protocols

            byte[] data = Encoding.UTF8.GetBytes(text);

            if (UseHM10Api())
            {
                // prefer exact discovered UUIDs (platform may return full-128 UUID strings)
                string serviceToUse = _discoveredServiceUUID ?? HM10ServiceUUID;
                string charToUse = _discoveredCharacteristicUUID ?? HM10CharacteristicUUID;

                StatusMessage = $"HM10: sending {data.Length} bytes to {_deviceAddress} service={serviceToUse} char={charToUse}";

                BluetoothLEHardwareInterface.WriteCharacteristic(_deviceAddress, serviceToUse, charToUse, data, data.Length, false, (characteristicUUID) =>
                {
                    StatusMessage = $"Write Succeeded (HM10) characteristic: {characteristicUUID}";
                });
            }
            else
            {
                BluetoothLEHardwareInterface.WriteCharacteristic(_deviceAddress, GetUUID(ServiceUUIDType.Service), GetUUID(ServiceUUIDType.WriteCharacteristic), data, data.Length, true, (characteristicUUID) =>
                {
                    BluetoothLEHardwareInterface.Log("Write Succeeded");
                });
            }
        }

        // Update is called once per frame
        void Update()
        {
            //should always connect because we dont know the state in the menu already
            //if (GameState.CurrentMode != GameState.GameMode.HYBRID)
            //return;

            if (_timeout > 0f)
            {
                _timeout -= Time.deltaTime;
                if (_timeout <= 0f)
                {
                    _timeout = 0f;

                    switch (_state)
                    {
                        case States.None:
                            break;

                        case States.Scan:
                            StatusMessage = "Scanning for " + DeviceName;

                            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>
                            {
                                // if your device does not advertise the rssi and manufacturer specific data
                                // then you must use this callback because the next callback only gets called
                                // if you have manufacturer specific data

                                if (!_rssiOnly)
                                {
                                    // use case-insensitive comparison and log discovered name + target
                                    BluetoothLEHardwareInterface.Log($"Discovered device: '{name}' (looking for '{DeviceName}')");
                                    if (!string.IsNullOrEmpty(name) && name.IndexOf(DeviceName, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        StatusMessage = "Found " + name;

                                        // found a device with the name we want
                                        // this example does not deal with finding more than one
                                        _deviceAddress = address;
                                        SetState(States.Connect, 0.5f);
                                    }
                                }

                            }, (address2, name2, rssiValue, bytes2) =>
                            {

                                // use this one if the device responses with manufacturer specific data and the rssi
                                BluetoothLEHardwareInterface.Log($"Discovered device (adv): '{name2}' (looking for '{DeviceName}') rssi={rssiValue}");

                                if (!string.IsNullOrEmpty(name2) && name2.IndexOf(DeviceName, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    StatusMessage = "Found " + name2;

                                    if (_rssiOnly)
                                    {
                                        _rssi = rssiValue;
                                    }
                                    else
                                    {
                                        // found a device with the name we want
                                        // this example does not deal with finding more than one
                                        _deviceAddress = address2;
                                        SetState(States.Connect, 0.5f);
                                    }
                                }

                            }, _rssiOnly); // this last setting allows RFduino to send RSSI without having manufacturer data

                            if (_rssiOnly)
                                SetState(States.ScanRSSI, 0.5f);
                            break;

                        case States.ScanRSSI:
                            break;

                        case States.ReadRSSI:
                            StatusMessage = $"Call Read RSSI";
                            BluetoothLEHardwareInterface.ReadRSSI(_deviceAddress, (address, rssi) =>
                            {
                                StatusMessage = $"Read RSSI: {rssi}";
                            });

                            SetState(States.ReadRSSI, 2f);
                            break;

                        case States.Connect:
                            StatusMessage = "Connecting...";

                            // set this flag
                            _foundCharacteristic = false;

                            // note that the first parameter is the address, not the name. I have not fixed this because
                            // of backwards compatiblity.
                            // also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
                            // the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
                            // large enough that it will be finished enumerating before you try to subscribe or do any other operations.
                            BluetoothLEHardwareInterface.ConnectToPeripheral(_deviceAddress,
                                (address) =>
                                {
                                    // direct connect callback: link up event
                                    StatusMessage = "Peripheral connected: " + address;
                                    // stop scanning immediately when link is up
                                    BluetoothLEHardwareInterface.StopScan();
                                },
                                null,
                                (address, serviceUUID, characteristicUUID) =>
                                {
                                    BluetoothLEHardwareInterface.Log("Discovered service/characteristic: " + serviceUUID + " / " + characteristicUUID);

                                    if (UseHM10Api())
                                    {
                                        // detect HM10 service/char regardless of full/short form and remember exact strings reported
                                        if (IsEqual(serviceUUID, HM10ServiceUUID))
                                        {
                                            if (IsEqual(characteristicUUID, HM10CharacteristicUUID))
                                            {
                                                _foundCharacteristic = true;
                                                _discoveredServiceUUID = serviceUUID;
                                                _discoveredCharacteristicUUID = characteristicUUID;
                                                BluetoothLEHardwareInterface.Log($"Stored discovered HM10 UUIDs service={_discoveredServiceUUID} char={_discoveredCharacteristicUUID}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (IsEqual(serviceUUID, GetUUID(ServiceUUIDType.Service)))
                                        {
                                            // mark found if either our read or write characteristic is present
                                            bool foundRead = IsEqual(characteristicUUID, GetUUID(ServiceUUIDType.ReadCharacteristic));
                                            bool foundWrite = IsEqual(characteristicUUID, GetUUID(ServiceUUIDType.WriteCharacteristic));

                                            if (foundRead)
                                                BluetoothLEHardwareInterface.Log("Found read characteristic: " + characteristicUUID);
                                            if (foundWrite)
                                                BluetoothLEHardwareInterface.Log("Found write characteristic: " + characteristicUUID);

                                            _foundCharacteristic = _foundCharacteristic || foundRead || foundWrite;
                                        }
                                    }

                                    // if we have found the characteristic(s) we are waiting for
                                    // set the state.
                                    if (_foundCharacteristic)
                                    {
                                        _connected = true;
                                        SetState(States.RequestMTU, 2f);
                                    }
                                },
                                (disconnectDeviceAddress) =>
                                {
                                    // if this is called, the device identitied with the passed in parameter
                                    // just disconnected.
                                    // You can start scanning again by setting the state to Scan like so:
                                    _connected = false;
                                    SetState(States.Scan, 0.1f);
                                });
                            break;

                        case States.RequestMTU:
                            StatusMessage = "Requesting MTU";

                            BluetoothLEHardwareInterface.RequestMtu(_deviceAddress, 185, (address, newMTU) =>
                            {
                                StatusMessage = "MTU set to " + newMTU.ToString();

                                SetState(States.Subscribe, 0.1f);
                            });
                            break;

                        case States.Subscribe:
                            StatusMessage = "Subscribing to characteristic...";

                            if (UseHM10Api())
                            {
                                // prefer discovered UUIDs (platform may return full UUIDs) but fall back to 16-bit
                                string serviceToUse = _discoveredServiceUUID ?? HM10ServiceUUID;
                                string charToUse = _discoveredCharacteristicUUID ?? HM10CharacteristicUUID;

                                BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, serviceToUse, charToUse, (notifyAddress, notifyCharacteristic) =>
                                {
                                    StatusMessage = "Waiting for data (HM10)...";
                                    _state = States.None;
                                }, (address, characteristicUUID, bytes) =>
                                {
                                    lock (_bufferLock)
                                    {
                                        _receiveBuffer.AddRange(bytes);
                                        ProcessData(_receiveBuffer.ToArray());
                                        _receiveBuffer.Clear();
                                    }
                                });
                            }
                            else
                            {
                                // subscribe to the single characteristic used for both notify and write
                                BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(_deviceAddress, GetUUID(ServiceUUIDType.Service), GetUUID(ServiceUUIDType.ReadCharacteristic), (notifyAddress, notifyCharacteristic) =>
                                {
                                    StatusMessage = "Waiting for data (1)...";
                                    _state = States.None;

                                    // read the initial value
                                    BluetoothLEHardwareInterface.ReadCharacteristic(_deviceAddress, GetUUID(ServiceUUIDType.Service), GetUUID(ServiceUUIDType.ReadCharacteristic), (characteristic, bytes) =>
                                    {
                                        // process incoming text payload
                                        ProcessData(bytes);
                                    });

                                    SetState(States.ReadRSSI, 1f);

                                }, (address, characteristicUUID, bytes) =>
                                {
                                    if (_state != States.None)
                                    {
                                        StatusMessage = "Waiting for data (2)...";
                                        SetState(States.ReadRSSI, 1f);
                                    }

                                    // we received some data from the device
                                    ProcessData(bytes);
                                });
                            }

                            break;

                        case States.Unsubscribe:
                            if (UseHM10Api())
                            {
                                string serviceToUse = _discoveredServiceUUID ?? HM10ServiceUUID;
                                string charToUse = _discoveredCharacteristicUUID ?? HM10CharacteristicUUID;
                                BluetoothLEHardwareInterface.UnSubscribeCharacteristic(_deviceAddress, serviceToUse, charToUse, null);
                            }
                            else
                            {
                                BluetoothLEHardwareInterface.UnSubscribeCharacteristic(_deviceAddress, GetUUID(ServiceUUIDType.Service), GetUUID(ServiceUUIDType.ReadCharacteristic), null);
                            }
                            SetState(States.Disconnect, 4f);
                            break;

                        case States.Disconnect:
                            StatusMessage = "Commanded disconnect.";

                            if (_connected)
                            {
                                BluetoothLEHardwareInterface.DisconnectPeripheral(_deviceAddress, (address) =>
                                {
                                    StatusMessage = "Device disconnected";
                                    BluetoothLEHardwareInterface.DeInitialize(() =>
                                    {
                                        _connected = false;
                                        _state = States.None;
                                    });
                                });
                            }
                            else
                            {
                                BluetoothLEHardwareInterface.DeInitialize(() =>
                                {
                                    _state = States.None;
                                });
                            }
                            break;
                    }
                }
            }
        }

        string FullUUID(string uuid)
        {
            string fullUUID = uuid;
            if (fullUUID.Length == 4)
                fullUUID = "0000" + uuid + "-0000-1000-8000-00805f9b34fb";

            return fullUUID;
        }

        bool IsEqual(string uuid1, string uuid2)
        {
            if (uuid1.Length == 4)
                uuid1 = FullUUID(uuid1);
            if (uuid2.Length == 4)
                uuid2 = FullUUID(uuid2);

            return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
        }
    }
}