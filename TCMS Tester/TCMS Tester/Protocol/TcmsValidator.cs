using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITester;
using static CITester.FormMain;

namespace TCMSTester.Protocol
{
    public class TcmsValidator
    {
        public class ValidationResult
        {
            public bool IsSuccess { get; set; } = true;
            public List<string> FailedPins { get; set; } = new List<string>();
        }

        /// <summary>
        /// 수신 데이터와 기대 설정값 비교 검증 (EChannelState 매핑) - 예외 안전성 보강 버전
        /// </summary>
        public static ValidationResult ValidateGroup(
            string categoryName,
            byte[] actualRaw,
            bool[] expectedBits,
            FormMain.EChannelState[] uiChannelStates,
            int activeCount)
        {
            var result = new ValidationResult();

            string category = string.IsNullOrWhiteSpace(categoryName) ? "CH" : categoryName;

            // 1. UI 채널 배열 및 활성 개수 검증 (Null 및 경계값 방어)
            if (uiChannelStates == null || activeCount <= 0)
            {
                result.IsSuccess = false;
                result.FailedPins.Add($"[{category}] UI 채널 상태 배열이 null이거나 활성 개수가 올바르지 않습니다.");
                return result;
            }

            // 2. actualRaw Null 검증
            if (actualRaw == null)
            {
                result.IsSuccess = false;
                int limit = Math.Min(activeCount, uiChannelStates.Length);
                for (int idx = 0; idx < limit; idx++)
                {
                    uiChannelStates[idx] = FormMain.EChannelState.Err;
                }
                result.FailedPins.Add($"[{category}] 수신된 Raw 데이터(actualRaw)가 null입니다.");
                return result;
            }

            // 3. UI 배열 경계 초과 방지를 위한 안전 루프 개수 산출
            int loopCount = Math.Min(activeCount, uiChannelStates.Length);

            for (int i = 0; i < loopCount; i++)
            {
                int portNo = (i / 8) + 1; // 1-based 포트(바이트) 번호
                int bitNo = (i % 8) + 1;  // 1-based 비트 번호

                // actualRaw 배열 길이 부족 시 IndexOutOfRangeException 방지
                if (actualRaw.Length < portNo)
                {
                    uiChannelStates[i] = FormMain.EChannelState.Err;
                    result.IsSuccess = false;
                    result.FailedPins.Add($"{category}_P{portNo}_B{bitNo} (Raw 데이터 바이트 부족: {actualRaw.Length}Bytes)");
                    continue;
                }

                byte portByte = actualRaw[portNo - 1];

                // ★ LSB(비트1=0x01) ~ MSB(비트8=0x80) 추출로 수정 (TcmsPacket과 연동 통일)
                bool isActualOn = ((portByte >> (bitNo - 1)) & 0x01) == 1;
                bool isExpectedOn = expectedBits != null && expectedBits.Length > i && expectedBits[i];

                if (isActualOn == isExpectedOn)
                {
                    // 검증 성공: On / Off 매핑
                    uiChannelStates[i] = isActualOn ? FormMain.EChannelState.On : FormMain.EChannelState.Off;
                }
                else
                {
                    // 검증 실패: Err 매핑
                    uiChannelStates[i] = FormMain.EChannelState.Err;
                    result.IsSuccess = false;
                    result.FailedPins.Add($"{category}_P{portNo}_B{bitNo} (기대:{isExpectedOn}, 실제:{isActualOn})");
                }
            }

            return result;
        }
    }
}