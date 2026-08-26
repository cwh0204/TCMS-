using System;
using System.Collections.Generic;
using System.Linq;

namespace TCMSTester.Config
{
    /// <summary>
    /// TCMS 장치 유닛 구분
    /// </summary>
    public enum EUnitType
    {
        Unknown,
        TC,
        CC,
        DU,
        ER
    }

    public class AppConfig
    {
        #region Digital Input (DI) Channel Ranges (TC: 32개, CC: 96개, DU: 5개)
        public const int DI_TC_START = 0;
        public const int DI_TC_COUNT = 32;  // 0 ~ 31 (32개)

        // [수정] CC 유닛 DI 시작 번호를 144로 변경 (144 ~ 239)
        public const int DI_CC_START = 144;
        public const int DI_CC_COUNT = 96;  // 144 ~ 239 (96개: DI1, DI2, DI3 각 32개)

        public const int DI_DU_START = 240;
        public const int DI_DU_COUNT = 5;   // 240 ~ 244 (5개)
        #endregion

        #region Digital Output (DO) Channel Ranges (TC: 144개, CC: 32개, DU: 5개)
        public const int DO_TC_START = 0;
        public const int DO_TC_COUNT = 144; // 0 ~ 143 (144개)

        // [수정] CC 유닛 DO 시작 번호를 32로 변경 (32 ~ 63)
        public const int DO_CC_START = 32;
        public const int DO_CC_COUNT = 32;  // 32 ~ 63 (32개)

        public const int DO_DU_START = 64;
        public const int DO_DU_COUNT = 5;   // 64 ~ 68 (5개)
        #endregion

        // JSON에서 로드되는 인풋/아웃풋 리스트
        public List<DigitalItemConfig> DigitalInputs { get; set; } = new List<DigitalItemConfig>();
        public List<DigitalItemConfig> DigitalOutputs { get; set; } = new List<DigitalItemConfig>();

        /// <summary>
        /// 문자열("TC", "CC", "DU")을 EUnitType Enum으로 변환합니다.
        /// </summary>
        public static EUnitType ParseUnitType(string unitStr)
        {
            if (Enum.TryParse(unitStr, true, out EUnitType result))
            {
                return result;
            }
            return EUnitType.Unknown;
        }

        /// <summary>
        /// DI / DO 및 Unit에 따른 시작 ChannelNo를 반환합니다.
        /// </summary>
        public static int GetStartChannelNo(EUnitType unit, bool isOutput = false)
        {
            if (!isOutput) // DI
            {
                switch (unit)
                {
                    case EUnitType.TC: return DI_TC_START; // 0
                    case EUnitType.CC: return DI_CC_START; // 144
                    case EUnitType.DU: return DI_DU_START; // 240
                    default: return 0;
                }
            }
            else // DO
            {
                switch (unit)
                {
                    case EUnitType.TC: return DO_TC_START; // 0
                    case EUnitType.CC: return DO_CC_START; // 32
                    case EUnitType.DU: return DO_DU_START; // 64
                    default: return 0;
                }
            }
        }

        /// <summary>
        /// ChannelNo와 신호 종류(DI/DO)를 기준으로 해당 핀이 속한 Unit 구분을 반환합니다.
        /// </summary>
        public static EUnitType GetUnitType(int channelNo, bool isOutput = false)
        {
            if (!isOutput) // DI
            {
                if (channelNo >= DI_TC_START && channelNo < DI_TC_START + DI_TC_COUNT) return EUnitType.TC;
                if (channelNo >= DI_CC_START && channelNo < DI_CC_START + DI_CC_COUNT) return EUnitType.CC;
                if (channelNo >= DI_DU_START && channelNo < DI_DU_START + DI_DU_COUNT) return EUnitType.DU;
            }
            else // DO
            {
                if (channelNo >= DO_TC_START && channelNo < DO_TC_START + DO_TC_COUNT) return EUnitType.TC;
                if (channelNo >= DO_CC_START && channelNo < DO_CC_START + DO_CC_COUNT) return EUnitType.CC;
                if (channelNo >= DO_DU_START && channelNo < DO_DU_START + DO_DU_COUNT) return EUnitType.DU;
            }
            return EUnitType.Unknown;
        }

        /// <summary>
        /// ChannelNo를 해당 Unit 내부 상대 인덱스(0부터 시작)로 변환합니다.
        /// </summary>
        public static int GetRelativePinIndex(int channelNo, bool isOutput = false)
        {
            EUnitType unit = GetUnitType(channelNo, isOutput);
            int startNo = GetStartChannelNo(unit, isOutput);

            return (unit != EUnitType.Unknown) ? (channelNo - startNo) : -1;
        }

        /// <summary>
        /// 유닛 및 카테고리(DI1, DI2, DI3, DO)별 시작 ChannelNo를 정확히 반환합니다.
        /// </summary>
        public static int GetCategoryStartChannelNo(string unitStr, string category, AppConfig configInstance = null)
        {
            EUnitType unit = ParseUnitType(unitStr);
            bool isOutput = (category?.ToUpper() == "DO");

            // 1. configInstance가 전달된 경우 Name이 카테고리(DI1, DI2 등)로 시작하는 항목 검색
            if (configInstance != null)
            {
                var targetList = isOutput ? configInstance.DigitalOutputs : configInstance.DigitalInputs;

                var unitItems = targetList.Where(x =>
                    GetUnitType(x.ChannelNo, isOutput) == unit &&
                    (x.Name != null && x.Name.StartsWith(category, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                if (unitItems.Count > 0)
                {
                    return unitItems.Min(x => x.ChannelNo);
                }
            }

            // 2. configInstance가 없거나 검색 결과가 없는 경우 오프셋 계산 반환
            int baseStartNo = GetStartChannelNo(unit, isOutput);

            if (!isOutput && unit == EUnitType.CC)
            {
                // 로그상 DI1이 48개(144~191)를 검사하므로 48개 간격 기준
                switch (category?.ToUpper())
                {
                    case "DI1": return baseStartNo;        // 144
                    case "DI2": return baseStartNo + 48;   // 192
                    case "DI3": return baseStartNo + 96;   // 240
                }
            }

            return baseStartNo;
        }

        /// <summary>
        /// 신호 이름으로 ChannelNo를 조회합니다.
        /// </summary>
        public int GetChannelNo(string name)
        {
            var item = DigitalInputs.FirstOrDefault(x => x.Name == name)
                    ?? DigitalOutputs.FirstOrDefault(x => x.Name == name);

            return item?.ChannelNo ?? -1;
        }

        /// <summary>
        /// 특정 ChannelNo에 해당하는 설정 객체를 DI 또는 DO 리스트에서 조회합니다.
        /// </summary>
        public DigitalItemConfig GetConfigByChannelNo(int channelNo, bool isOutput = false)
        {
            return isOutput
                ? DigitalOutputs.FirstOrDefault(x => x.ChannelNo == channelNo)
                : DigitalInputs.FirstOrDefault(x => x.ChannelNo == channelNo);
        }

        /// <summary>
        /// 특정 Unit(TC, CC, DU)의 DigitalInputs 목록을 가져옵니다.
        /// </summary>
        public List<DigitalItemConfig> GetInputsByUnit(EUnitType unit)
        {
            return DigitalInputs
                .Where(x => GetUnitType(x.ChannelNo, isOutput: false) == unit)
                .OrderBy(x => x.ChannelNo)
                .ToList();
        }

        /// <summary>
        /// 특정 Unit(TC, CC, DU)의 DigitalOutputs 목록을 가져옵니다.
        /// </summary>
        public List<DigitalItemConfig> GetOutputsByUnit(EUnitType unit)
        {
            return DigitalOutputs
                .Where(x => GetUnitType(x.ChannelNo, isOutput: true) == unit)
                .OrderBy(x => x.ChannelNo)
                .ToList();
        }

        public int this[string name] => GetChannelNo(name);
    }
}