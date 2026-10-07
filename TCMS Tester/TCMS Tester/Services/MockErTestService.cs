using System;
using System.Drawing;
using System.Threading.Tasks;

namespace TCMSTester.Services
{
    public class MockErTestService
    {
        public string TargetIp { get; set; } = "10.0.1.11";
        public int TargetPort { get; set; } = 5060;

        public Action<string, Color> OnLog { get; set; }
        public Action<string, Color> OnFailLog { get; set; }

        // 테스트 편의를 위한 가상 하드웨어 상태
        private readonly bool[] _mockDiPins = new bool[8];
        private byte _mockDoMask = 0x00;

        // 특정 항목 강제 실패 시뮬레이션 플래그 (디버깅용)
        public bool SimulateFail { get; set; } = false;

        public MockErTestService()
        {
            // 기본 DI 핀 상태를 모두 true(정상 도통)로 초기화
            for (int i = 0; i < 8; i++) _mockDiPins[i] = true;
        }

        #region [1] 디지털 입·출력 Mock

        public async Task<bool[]> ReadDigitalInputsAsync(int timeoutMs = 2000)
        {
            OnLog?.Invoke($"[TX] EDI-REQ > 02 45 44 49 01 00 00 A3 03 (REQ_DI_STATE)", Color.DimGray);
            await Task.Delay(190); // 하드웨어 시상수 시뮬레이션

            // 8채널 비트 마스크 패킹 (모두 true면 0xFF)
            byte diMask = 0x00;
            for (int i = 0; i < 8; i++)
            {
                if (_mockDiPins[i]) diMask |= (byte)(1 << i);
            }

            OnLog?.Invoke($"[RX] EDI-ACK < 02 45 44 49 01 01 {diMask:X2} {(byte)(diMask ^ 0x55):X2} 03 (DI_PINS: 0b_{Convert.ToString(diMask, 2).PadLeft(8, '0')})", Color.DarkSlateGray);
            return (bool[])_mockDiPins.Clone();
        }

        public async Task<bool> SetDigitalOutputAsync(int channel, bool state, int timeoutMs = 2000)
        {
            if (channel < 1 || channel > 8) return false;

            int bit = channel - 1;
            if (state) _mockDoMask |= (byte)(1 << bit);
            else _mockDoMask &= (byte)~(1 << bit);

            OnLog?.Invoke($"[TX] VROJ-SET > 02 56 52 4F {channel:X2} {(state ? 0x01 : 0x00):X2} {_mockDoMask:X2} 03 (CH{channel} {(state ? "ON" : "OFF")})", Color.DarkBlue);
            await Task.Delay(170);

            OnLog?.Invoke($"[RX] VROJ-ACK < 02 06 56 52 4F 00 03 (ACK_OK, CurrentMask: 0x{_mockDoMask:X2})", Color.ForestGreen);
            return true;
        }

        public async Task<bool> SetDigitalOutputMaskAsync(byte outputMask, int timeoutMs = 2000)
        {
            _mockDoMask = outputMask;

            OnLog?.Invoke($"[TX] VROJ-MSK > 02 56 4D 53 4B {_mockDoMask:X2} 55 03 (SET_ALL_MASK)", Color.DarkBlue);
            await Task.Delay(150);

            OnLog?.Invoke($"[RX] VROJ-ACK < 02 06 56 4D 00 {_mockDoMask:X2} 03 (MASK_APPLIED: 0x{_mockDoMask:X2})", Color.ForestGreen);
            return true;
        }

        #endregion

        #region [2] 통신 시험 Mock (5종)

        public async Task<bool> TestLanPingAsync(int timeoutMs = 2000)
        {
            OnLog?.Invoke($"[TX] ICMP-ECHO > {TargetIp} Type:8 Code:0 Seq:0x0001 Len:32", Color.DimGray);
            await Task.Delay(1000);

            if (SimulateFail)
            {
                OnFailLog?.Invoke($"[RX] ICMP-TIMEOUT < {TargetIp} No reply within {timeoutMs}ms (Request Timed Out)", Color.Red);
                return false;
            }

            OnLog?.Invoke($"[RX] ICMP-REPLY < {TargetIp} Type:0 Code:0 Seq:0x0001 Bytes:32 TTL:64 RTT:2ms", Color.DarkGreen);
            return true;
        }

        public async Task<bool> TestTargetProbeAsync(int timeoutMs = 3000)
        {
            OnLog?.Invoke($"[TX] SCM-CAN > ID:0x7E0 DLC:8 Data: 02 10 01 00 00 00 00 00 (DIAG_SESSION_PROBE)", Color.DimGray);
            await Task.Delay(1200);

            OnLog?.Invoke($"[RX] SCM-CAN < ID:0x7E8 DLC:8 Data: 06 50 01 00 01 01 2A 00 (SCM/EDI_ACK, Nodes:0x01)", Color.DarkGreen);
            return true;
        }

        public async Task<bool> TestMvbAsync(int timeoutMs = 3000)
        {
            OnLog?.Invoke($"[TX] MVB-LPBK > Port:0x43A0 Frame:0x42 Size:32B Data: 5A A5 00 01 ... [CRC16: 0xE24F]", Color.DimGray);
            await Task.Delay(1400);

            OnLog?.Invoke($"[RX] MVB-LPBK < Port:0x43A0 Frame:0x42 Size:32B Data: 5A A5 00 01 ... (Match: 100%, CRC: OK)", Color.DarkGreen);
            return true;
        }

        public async Task<bool> TestCpmAsync(int timeoutMs = 3000)
        {
            OnLog?.Invoke($"[TX] CPM-REQ > 02 43 50 4D 00 01 00 00 9A 03 (STATUS_QUERY)", Color.DimGray);
            await Task.Delay(1000);

            OnLog?.Invoke($"[RX] CPM-RSP < 02 43 50 4D 01 00 00 01 9B 03 (STATUS: 0x0001_OK, COLLISION_ARMED)", Color.DarkGreen);
            return true;
        }

        public async Task<bool> TestUsbAsync(int timeoutMs = 3000)
        {
            OnLog?.Invoke($"[TX] USB-IO > WRITE Sector:0x00002000 Size:4096B Checksum:0xABCD", Color.DimGray);
            await Task.Delay(1300);

            OnLog?.Invoke($"[RX] USB-IO < READBACK Sector:0x00002000 Checksum:0xABCD (Pattern Match PASS)", Color.DarkGreen);
            return true;
        }

        #endregion

        #region [3] 메모리 시험 Mock (5종)

        public async Task<bool> TestFlashMemoryAsync(int timeoutMs = 4000)
        {
            OnLog?.Invoke($"[TX] MEM-CMD > 0x2A44 FLASH_CHKSUM_CHECK Addr:0x08000000 Len:0x00100000", Color.DimGray);
            await Task.Delay(1400);

            OnLog?.Invoke($"[RX] MEM-RSP < 0x2A44 FLASH_ACK CRC32:0x4B3A82C1 Expected:0x4B3A82C1 (VALID)", Color.DarkBlue);
            return true;
        }

        public async Task<bool> TestSdramAsync(int timeoutMs = 6000)
        {
            OnLog?.Invoke($"[TX] MEM-CMD > 0x2A3C SDRAM_BUS_WALKING_1 Addr:0xC0000000 Len:0x00004000 Pattern:0x55AA", Color.DimGray);
            await Task.Delay(1600);

            OnLog?.Invoke($"[RX] MEM-RSP < 0x2A3C SDRAM_ACK ReadErrCount:0 StuckBit:None (BUS_INTEGRITY_OK)", Color.DarkBlue);
            return true;
        }

        public async Task<bool> TestFramAsync(int timeoutMs = 4000)
        {
            OnLog?.Invoke($"[TX] MEM-CMD > 0x2A4C FRAM_VERIFY_RW Addr:0x00000100 Seed:0x7F", Color.DimGray);
            await Task.Delay(1400);

            OnLog?.Invoke($"[RX] MEM-RSP < 0x2A4C FRAM_ACK WriteVerify: OK, RetainFlag: 1 (PASS)", Color.DarkBlue);
            return true;
        }

        public async Task<bool> TestRtcAsync(int timeoutMs = 3000)
        {
            OnLog?.Invoke($"[TX] RTC-I2C > READ_REG DevAddr:0x68 Reg:0x00 Len:7 (SEC/MIN/HOUR/DAY/MON/YR)", Color.DimGray);
            await Task.Delay(1300);

            var now = DateTime.Now;
            string bcdTime = $"{now:yy-MM-dd HH:mm:ss}";
            OnLog?.Invoke($"[RX] RTC-I2C < REG_DATA BCD:[{bcdTime}] Oscillator: RUNNING Flag: 0x00 (NORMAL)", Color.DarkBlue);
            return true;
        }

        public async Task<bool> TestHrsAsync(int timeoutMs = 3000)
        {
            OnLog?.Invoke($"[TX] HRS-IO  > READ_ROTARY_SW Port:GPIO_E Pin:12-15", Color.DimGray);
            await Task.Delay(1300);

            OnLog?.Invoke($"[RX] HRS-IO  < SW_VALUE Raw:0x03 BCD:3 (CAR_ID_CONFIG_SETTING_OK)", Color.DarkBlue);
            return true;
        }

        #endregion

        public void Dispose()
        {
        }
    }
}