using System;
using System.Reflection;
using System.Windows.Forms;
using TCMSTester.Services;

namespace TCMSTester.Tests
{
    public static class TcmsTestRunner
    {
        public static void RunAllTests()
        {
            try
            {
                // 1. TcmsTestService 인스턴스 생성
                var service = new TcmsTestService(null);

                // 2. 34바이트 테스트 Hex 패킷
                string dummyHex = "41A001202608260815" + "112233445566" + "AABBCCDDEEFF" + "1234567890AB" + "998877665544" + "00";

                // 3. 수신 로직 실행
                service.OnDataReceived(dummyHex);

                // 4. Private 데이터 추출 및 검증
                byte[] di1 = GetPrivateRawData(service, "DI1");
                byte[] doData = GetPrivateRawData(service, "DO");

                string di1Text = di1 != null ? BitConverter.ToString(di1) : "null";
                string doText = doData != null ? BitConverter.ToString(doData) : "null";

                MessageBox.Show($"[1단계 파싱 결과]\n\nDI1: {di1Text}\nDO : {doText}", "테스트 성공");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"테스트 도중 예외가 발생했습니다:\n{ex.Message}", "테스트 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Helper Methods

        private static byte[] GetPrivateRawData(TcmsTestService service, string category)
        {
            var method = typeof(TcmsTestService).GetMethod("GetCurrentRawData",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (byte[])method?.Invoke(service, new object[] { category });
        }

        #endregion
    }
}