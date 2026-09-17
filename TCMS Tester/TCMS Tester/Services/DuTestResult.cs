namespace TCMSTester.Services
{
    /// <summary>
    /// DU(표시기) 통신 시험 결과 모델 (MVB / RS485)
    /// </summary>
    public class DuTestResult
    {
        public bool IsMvbPass { get; set; }
        public bool IsRs485Pass { get; set; }

        public string MvbStatusMessage { get; set; } = string.Empty;
        public string Rs485StatusMessage { get; set; } = string.Empty;

        /// <summary>
        /// DU 통신 시험 전체 합격 여부
        /// </summary>
        public bool IsAllPassed => IsMvbPass && IsRs485Pass;
    }
}