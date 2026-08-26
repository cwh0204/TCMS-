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
        // Unit별 ChannelNo 범위 설정
        public const int TC_START = 0;
        public const int TC_COUNT = 144; // 0 ~ 143 (144개)

        public const int CC_START = 144;
        public const int CC_COUNT = 96;  // 144 ~ 239 (96개)

        public const int DU_START = 240;
        public const int DU_COUNT = 5;   // 240 ~ 244 (5개)

        public List<DigitalItemConfig> DigitalInputs { get; set; } = new List<DigitalItemConfig>();
        public List<DigitalItemConfig> DigitalOutputs { get; set; } = new List<DigitalItemConfig>();

        /// <summary>
        /// ChannelNo를 기준으로 해당 핀이 속한 Unit 구분을 반환합니다.
        /// </summary>
        public static EUnitType GetUnitType(int channelNo)
        {
            if (channelNo >= TC_START && channelNo < TC_START + TC_COUNT) return EUnitType.TC;
            if (channelNo >= CC_START && channelNo < CC_START + CC_COUNT) return EUnitType.CC;
            if (channelNo >= DU_START && channelNo < DU_START + DU_COUNT) return EUnitType.DU;
            return EUnitType.Unknown;
        }

        /// <summary>
        /// ChannelNo를 해당 Unit 내부 상대 인덱스(0부터 시작)로 변환합니다. (C# 7.3 호환)
        /// (예: ChannelNo 144 -> CC 유닛의 0번 핀)
        /// </summary>
        public static int GetRelativePinIndex(int channelNo)
        {
            EUnitType unit = GetUnitType(channelNo);
            switch (unit)
            {
                case EUnitType.TC:
                    return channelNo - TC_START;
                case EUnitType.CC:
                    return channelNo - CC_START;
                case EUnitType.DU:
                    return channelNo - DU_START;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// 신호 이름으로 전체 채널 번호를 가져옵니다. (없으면 -1 반환)
        /// </summary>
        public int GetChannelNo(string name)
        {
            var item = DigitalInputs.FirstOrDefault(x => x.Name == name)
                    ?? DigitalOutputs.FirstOrDefault(x => x.Name == name);

            return item?.ChannelNo ?? -1;
        }

        /// <summary>
        /// 특정 ChannelNo에 해당하는 설정 객체를 조회합니다.
        /// </summary>
        public DigitalItemConfig GetConfigByChannelNo(int channelNo)
        {
            return DigitalInputs.FirstOrDefault(x => x.ChannelNo == channelNo)
                ?? DigitalOutputs.FirstOrDefault(x => x.ChannelNo == channelNo);
        }

        /// <summary>
        /// 특정 Unit(TC, CC, DU)의 DigitalInputs 목록만 필터링하여 가져옵니다.
        /// </summary>
        public List<DigitalItemConfig> GetInputsByUnit(EUnitType unit)
        {
            return DigitalInputs
                .Where(x => GetUnitType(x.ChannelNo) == unit)
                .OrderBy(x => x.ChannelNo)
                .ToList();
        }

        /// <summary>
        /// 특정 Unit(TC, CC, DU)의 DigitalOutputs 목록만 필터링하여 가져옵니다.
        /// </summary>
        public List<DigitalItemConfig> GetOutputsByUnit(EUnitType unit)
        {
            return DigitalOutputs
                .Where(x => GetUnitType(x.ChannelNo) == unit)
                .OrderBy(x => x.ChannelNo)
                .ToList();
        }

        // 인덱서로 간단히 가져오기 (예: config["신호명"])
        public int this[string name] => GetChannelNo(name);
    }
}