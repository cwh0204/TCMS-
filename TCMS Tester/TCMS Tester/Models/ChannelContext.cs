using CITester;

namespace TCMSTester.Models
{
    /// <summary>
    /// TCMS 유닛별(TC/CC/DU) 활성화 채널 상태 배열 및 개수 정보 DTO
    /// </summary>
    public class ChannelContext
    {
        public FormMain.EChannelState[] ActiveDi1 { get; set; }
        public FormMain.EChannelState[] ActiveDi2 { get; set; }
        public FormMain.EChannelState[] ActiveDi3 { get; set; }
        public FormMain.EChannelState[] ActiveDo { get; set; }

        public int ActiveDi1Count { get; set; }
        public int ActiveDi2Count { get; set; }
        public int ActiveDi3Count { get; set; }
        public int ActiveDoCount { get; set; }
    }
}