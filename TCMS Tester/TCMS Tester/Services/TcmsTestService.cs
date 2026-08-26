using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CITester;
using Newtonsoft.Json;
using TCMSTester.Models;
using TCMSTester.Protocol;
using static CITester.FormMain;

namespace TCMSTester.Services
{
    public class TcmsTestService
    {
        private readonly MvbReceiver _mvbReceiver;

        // 파싱된 최신 패킷 보관
        private TcmsPacket _latestPacket;
        private readonly object _lockObj = new object();

        public Action<string, Color> OnLog { get; set; }
        public Action<string, Color> OnFailLog { get; set; }
        public Action OnGridInvalidate { get; set; }

        public Func<string, EChannelState[], int, int, int, List<string>, List<TestResultJson.PinResultItem>, Func<byte[]>, Task> RunChannelSequenceFunc { get; set; }
        public Func<int, int, List<string>, List<TestResultJson.PinResultItem>, Task> RunAnalogSequenceFunc { get; set; }

        public TcmsTestService(MvbReceiver mvbReceiver)
        {
            _mvbReceiver = mvbReceiver;
        }

        public async Task<bool> ExecuteTestSequenceAsync(
            string strUnitType,
            int nMaxLoop,
            Func<bool> checkIsTesting,
            ChannelContext context)
        {
            var testResult = new TestResultJson();

            testResult.Header.TCMSUnit = strUnitType;
            testResult.Header.SerialNo = ConfigJson.CurrentConfig?.Operation?.SerialNo ?? "0000";
            testResult.Header.TesterName = ConfigJson.CurrentConfig?.Operation?.TesterName ?? "Tester";
            testResult.Header.FleetNo = ConfigJson.CurrentConfig?.Operation?.FleetNo ?? "0000";
            testResult.Header.TrainNo = ConfigJson.CurrentConfig?.Operation?.TrainNo ?? "0000";
            testResult.Header.TotalRound = nMaxLoop;

            var objDigitalGridResult = new TestResultJson.GridTestResult { GridTitle = "디지털 입출력 시험" };
            var objAnalogGridResult = new TestResultJson.GridTestResult { GridTitle = "아날로그 입출력 시험" };

            bool bHasAnyFailure = false;
            //딜레이 시간
            int nAnimationDelay = 100;

            try
            {
                if (_mvbReceiver != null)
                {
                    // 유닛 타입에 따른 프레임 크기 지정 (TC = 40바이트, CC/DU = 34바이트)
                    _mvbReceiver.FrameSize = (strUnitType == "TC")
                        ? MvbReceiver.PACKET_SIZE_TC
                        : MvbReceiver.PACKET_SIZE_DEFAULT;

                    _mvbReceiver.PacketReceived -= OnMvbPacketReceived;
                    _mvbReceiver.PacketReceived += OnMvbPacketReceived;

                    _mvbReceiver.ErrorOccurred -= OnMvbErrorOccurred;
                    _mvbReceiver.ErrorOccurred += OnMvbErrorOccurred;

                    if (!_mvbReceiver.IsRunning)
                    {
                        _mvbReceiver.Start();
                        OnLog?.Invoke($"[통신] MVB 수신 스레드가 시작되었습니다. (유닛:{strUnitType}, FrameSize:{_mvbReceiver.FrameSize})", Color.DarkGreen);
                    }
                    else
                    {
                        OnLog?.Invoke($"[통신] MVB 이벤트 핸들러 연결 완료 (수신기 동작 중, FrameSize:{_mvbReceiver.FrameSize})", Color.DarkGreen);
                    }
                }
                else
                {
                    OnLog?.Invoke("[통신 경고] _mvbReceiver 객체가 null입니다!", Color.Red);
                    Debug.WriteLine("[DEBUG] _mvbReceiver가 null입니다.");
                }

                for (int nLoop = 1; nLoop <= nMaxLoop; nLoop++)
                {
                    if (!checkIsTesting()) break;

                    objDigitalGridResult.HeaderRounds.Add($"{nLoop}회차");
                    objAnalogGridResult.HeaderRounds.Add($"{nLoop}회차");

                    OnLog?.Invoke($"===== [{strUnitType} 유닛] {nLoop}회차 시험 시작 =====", Color.Purple);
                    List<string> listFailedPins = new List<string>();

                    ClearChannelStates(context);
                    OnGridInvalidate?.Invoke();

                    await Task.Delay(300);

                    // 디지털 입출력 시퀀스 수행
                    if (!checkIsTesting()) break;
                    if (context.ActiveDi1Count > 0 && RunChannelSequenceFunc != null)
                        await RunChannelSequenceFunc("DI1", context.ActiveDi1, context.ActiveDi1Count, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DI1"));

                    if (!checkIsTesting()) break;
                    if (context.ActiveDi2Count > 0 && RunChannelSequenceFunc != null)
                        await RunChannelSequenceFunc("DI2", context.ActiveDi2, context.ActiveDi2Count, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DI2"));

                    if (!checkIsTesting()) break;
                    if (context.ActiveDi3Count > 0 && RunChannelSequenceFunc != null)
                        await RunChannelSequenceFunc("DI3", context.ActiveDi3, context.ActiveDi3Count, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DI3"));

                    if (!checkIsTesting()) break;
                    if (context.ActiveDoCount > 0 && RunChannelSequenceFunc != null)
                        await RunChannelSequenceFunc("DO", context.ActiveDo, context.ActiveDoCount, nAnimationDelay, nLoop, listFailedPins, objDigitalGridResult.PinDetails, () => GetCurrentRawData("DO"));

                    // 아날로그 시험
                    if (!checkIsTesting()) break;
                    if (RunAnalogSequenceFunc != null)
                        await RunAnalogSequenceFunc(nAnimationDelay, nLoop, listFailedPins, objAnalogGridResult.PinDetails);

                    if (!checkIsTesting()) break;

                    // 회차 판정
                    if (listFailedPins.Count > 0)
                    {
                        bHasAnyFailure = true;
                        OnFailLog?.Invoke($"{nLoop}회차 - {string.Join(", ", listFailedPins)} 오류", Color.DarkRed);
                    }
                    else
                    {
                        OnFailLog?.Invoke($"{nLoop}회차 - 모든 채널 정상", Color.Green);
                    }

                    OnLog?.Invoke($"===== [{strUnitType} 유닛] {nLoop}회차 시험 종료 =====", Color.Purple);

                    await Task.Delay(500);
                }

                if (checkIsTesting())
                {
                    testResult.Header.FinalResult = bHasAnyFailure ? "불합격" : "합격";
                    testResult.GridResults.Add(objDigitalGridResult);
                    testResult.GridResults.Add(objAnalogGridResult);

                    OnLog?.Invoke($"[{strUnitType} 유닛] 디지털 및 아날로그 입출력 시험 완료.", Color.Blue);

                    TestResultManager objResultManager = new TestResultManager();
                    bool bSaveSuccess = objResultManager.SaveTestResult(testResult);

                    if (bSaveSuccess)
                    {
                        OnLog?.Invoke("[시스템] 모든 입출력 시험 결과 JSON 저장 완료.", Color.DarkBlue);
                    }
                }
            }
            catch (Exception ex)
            {
                OnLog?.Invoke($"[시스템 에러] 예외 발생: {ex.Message}", Color.Red);
            }
            finally
            {
                if (_mvbReceiver != null)
                {
                    _mvbReceiver.PacketReceived -= OnMvbPacketReceived;
                    _mvbReceiver.ErrorOccurred -= OnMvbErrorOccurred;
                }
            }

            return !bHasAnyFailure;
        }

        private void OnMvbPacketReceived(object sender, MvbPacketEventArgs e)
        {
            if (e == null) return;

            // 1. e.Packet 객체가 이미 생성되어 들어온 경우 최신 패킷으로 직접 할당
            if (e.Packet != null)
            {
                lock (_lockObj)
                {
                    _latestPacket = e.Packet;
                }
                return;
            }

            // 2. e.RawData (byte[])만 전달된 경우 Hex 문자열로 변환하여 기존 파서로 전달
            if (e.RawData != null && e.RawData.Length > 0)
            {
                string rawHex = BitConverter.ToString(e.RawData).Replace("-", "");
                OnDataReceived(rawHex);
            }
        }

        private void OnMvbErrorOccurred(object sender, string errorMessage)
        {
            OnLog?.Invoke($"[MVB 수신 에러] {errorMessage}", Color.Red);
        }

        public void OnDataReceived(string rawHex)
        {
            if (string.IsNullOrEmpty(rawHex)) return;

            // 공백 제거 및 대문자 정규화
            string cleanHex = rawHex.Replace(" ", "").Trim().ToUpper();

            // 헤더 검사는 제외하고 최소 길이만 체크 (34바이트 = 68 Hex 문자)
            if (cleanHex.Length < 68)
            {
                Debug.WriteLine($"[MVB Parser] 패킷 길이 부족으로 버려짐 (Length={cleanHex.Length}): {cleanHex}");
                return;
            }

            try
            {
                byte[] bytes = ConvertHexToBytes(cleanHex);
                if (bytes == null || bytes.Length < 31) return; // 7(Offset) + 24(Payload) = 최소 31바이트 필요

                byte[] di1 = new byte[6];
                byte[] di2 = new byte[6];
                byte[] di3 = new byte[6];
                byte[] doData = new byte[6];

                // 헤더 2바이트가 없으므로 9번이 아닌 7번 인덱스부터 복사
                Array.Copy(bytes, 7, di1, 0, 6);   // DI1 (Byte 7~12)
                Array.Copy(bytes, 13, di2, 0, 6);   // DI2 (Byte 13~18)
                Array.Copy(bytes, 19, di3, 0, 6);   // DI3 (Byte 19~24)
                Array.Copy(bytes, 25, doData, 0, 6); // DO  (Byte 25~30)

                lock (_lockObj)
                {
                    _latestPacket = new TcmsPacket
                    {
                        Di1Raw = di1,
                        Di2Raw = di2,
                        Di3Raw = di3,
                        DoRaw = doData
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MVB Parser] 데이터 파싱 중 예외 발생: {ex.Message}");
            }
        }

        private byte[] GetCurrentRawData(string category)
        {
            lock (_lockObj)
            {
                if (_latestPacket == null)
                {
                    Debug.WriteLine($"[MVB Getter] _latestPacket이 NULL 상태입니다! (category={category})");
                    OnLog?.Invoke($"[디버그] GetCurrentRawData({category}) 호출됨 -> 반환 데이터 NULL", Color.Orange);
                    return null;
                }

                byte[] source = null;
                string key = category != null ? category.ToUpper() : string.Empty;

                switch (key)
                {
                    case "DI1":
                        source = _latestPacket.Di1Raw;
                        break;
                    case "DI2":
                        source = _latestPacket.Di2Raw;
                        break;
                    case "DI3":
                        source = _latestPacket.Di3Raw;
                        break;
                    case "DO":
                        source = _latestPacket.DoRaw;
                        break;
                    default:
                        source = null;
                        break;
                }

                if (source == null)
                {
                    Debug.WriteLine($"[MVB Getter] 알 수 없거나 빈 데이터 category={category}");
                    return null;
                }

                string hexStr = BitConverter.ToString(source).Replace("-", " ");
                Debug.WriteLine($"[MVB Getter] 검증기 전달 데이터[{category}]({source.Length}bytes): {hexStr}");

                return (byte[])source.Clone();
            }
        }

        private byte[] ConvertHexToBytes(string hex)
        {
            if (hex.Length % 2 != 0) return null;

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int hi = GetHexVal(hex[i * 2]);
                int lo = GetHexVal(hex[i * 2 + 1]);

                if (hi < 0 || lo < 0) return null;

                bytes[i] = (byte)((hi << 4) | lo);
            }
            return bytes;
        }

        private int GetHexVal(char hex)
        {
            int val = (int)hex;
            if (val >= '0' && val <= '9') return val - '0';
            if (val >= 'A' && val <= 'F') return val - 'A' + 10;
            if (val >= 'a' && val <= 'f') return val - 'a' + 10;
            return -1;
        }

        private void ClearChannelStates(ChannelContext context)
        {
            if (context == null) return;
            if (context.ActiveDi1 != null) Array.Clear(context.ActiveDi1, 0, context.ActiveDi1.Length);
            if (context.ActiveDi2 != null) Array.Clear(context.ActiveDi2, 0, context.ActiveDi2.Length);
            if (context.ActiveDi3 != null) Array.Clear(context.ActiveDi3, 0, context.ActiveDi3.Length);
            if (context.ActiveDo != null) Array.Clear(context.ActiveDo, 0, context.ActiveDo.Length);
        }
    }
}