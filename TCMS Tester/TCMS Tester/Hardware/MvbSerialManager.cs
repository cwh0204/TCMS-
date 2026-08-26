using System;
using System.IO.Ports;
using TCMSTester.Protocol;

namespace TCMSTester.Hardware
{
    public class MvbSerialManager : IDisposable
    {
        private SerialPort _serialPort;
        private readonly MvbReceiver _mvbReceiver;

        public bool IsOpen
        {
            get { return _serialPort != null && _serialPort.IsOpen; }
        }

        // UI 로그 출력을 위한 콜백 이벤트
        public Action<string> OnLog { get; set; }
        public Action<string> OnError { get; set; }

        public MvbSerialManager(MvbReceiver mvbReceiver)
        {
            _mvbReceiver = mvbReceiver;
        }

        /// <summary>
        /// 시리얼 포트 연결 오픈
        /// </summary>
        public bool OpenPort(string portName, int baudRate = 115200)
        {
            try
            {
                if (IsOpen) ClosePort();

                _serialPort = new SerialPort
                {
                    PortName = portName,
                    BaudRate = baudRate,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None
                };

                _serialPort.DataReceived += OnDataReceived;
                _serialPort.Open();

                OnLog?.Invoke($"[시리얼] {portName} 포트 연결 성공 ({baudRate} bps)");
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"[시리얼 에러] {portName} 연결 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 시리얼 포트 연결 해제
        /// </summary>
        public void ClosePort()
        {
            if (_serialPort != null)
            {
                try
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.DataReceived -= OnDataReceived;
                        _serialPort.Close();
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke($"[시리얼 에러] 포트 닫기 실패: {ex.Message}");
                }
                finally
                {
                    _serialPort.Dispose();
                    _serialPort = null;
                    OnLog?.Invoke("[시리얼] 포트 연결 해제됨");
                }
            }
        }

        /// <summary>
        /// 시리얼 데이터 수신 이벤트 발생 시 MvbReceiver 버퍼로 자동 전달
        /// </summary>
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort == null || !_serialPort.IsOpen) return;

            try
            {
                int bytesToRead = _serialPort.BytesToRead;
                if (bytesToRead <= 0) return;

                byte[] buffer = new byte[bytesToRead];
                int readBytes = _serialPort.Read(buffer, 0, bytesToRead);

                // MvbReceiver 버퍼에 수신된 Raw 데이터 푸시
                _mvbReceiver?.PushRawData(buffer, readBytes);
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"[시리얼 수신 에러] {ex.Message}");
            }
        }

        public void Dispose()
        {
            ClosePort();
        }
    }
}