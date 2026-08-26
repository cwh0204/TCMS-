using System;

namespace TCMSTester.Config
{
    public class DigitalItemConfig
    {
        public int ChannelNo { get; set; }        // 채널/비트 번호 (0, 1, 2...)
        public string Name { get; set; }          // 부품/신호명 (TP24V, MCBa 등)
        public int OnValue { get; set; }          // 체크 시 전송/설정할 값
        public int OffValue { get; set; }         // 해제 시 전송/설정할 값 (기본: 0)
        public bool IsChecked { get; set; }       // 초기 체크 상태

        public DigitalItemConfig()
        {
            Name = string.Empty;
            OnValue = 1;
            OffValue = 0;
            IsChecked = false;
        }
    }
}