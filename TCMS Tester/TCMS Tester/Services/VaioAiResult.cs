using System;

namespace TCMSTester.Services
{
    public class VaioAiResult
    {
        public ushort RawCh1 { get; set; } // 0~10mA  (규격: 0~1000 count -> 0~10.00mA)
        public ushort RawCh2 { get; set; } // 0~20mA  (규격: 0~1000 count -> 0~20.00mA)
        public ushort RawCh3 { get; set; } // 0~10V   (규격: 0~1000 count -> 0~10.00V)
        public ushort RawCh4 { get; set; } // 0~10V   (규격: 0~1000 count -> 0~10.00V)
        public ushort RawCh5 { get; set; } // Mascon FB (규격: 1500 count -> 15.00V)

        // 단위 스케일링 변환 (1000 count 기준)
        public double Ch1_mA => RawCh1 / 100.0;       // 1000 -> 10.00mA
        public double Ch2_mA => RawCh2 / 50.0;        // 1000 -> 20.00mA
        public double Ch3_V => RawCh3 / 100.0;        // 1000 -> 10.00V
        public double Ch4_V => RawCh4 / 100.0;        // 1000 -> 10.00V
        public double Ch5_V => RawCh5 / 100.0;        // 1500 -> 15.00V

        /// <summary>
        /// 규격 3.1: Ch5 Mascon Feedback 15V 정상 여부 (기본 오차 ±0.5V 허용)
        /// </summary>
        public bool IsMasconValid(double tolerance = 0.5) => Math.Abs(Ch5_V - 15.0) <= tolerance;
    }
}