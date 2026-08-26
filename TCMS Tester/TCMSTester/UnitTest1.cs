using System;
using System.Reflection;
using Xunit;
using TCMSTester.Services;

namespace TCMSTester
{
    public class TcmsTestServiceTests
    {
        [Fact]
        public void OnDataReceived_정상_Hex패킷_수신시_정상파싱_검증()
        {
            // Arrange (준비)
            var service = new TcmsTestService(null);

            // 34바이트 Hex 패킷 (헤더:41A0, DI1:112233445566, DO:998877665544)
            string dummyHex = "41A001202608260815" + "112233445566" + "AABBCCDDEEFF" + "1234567890AB" + "998877665544" + "00";

            // Act (실행)
            service.OnDataReceived(dummyHex);

            // Assert (검증)
            byte[] di1Data = InvokePrivateGetRawData(service, "DI1");
            byte[] doData = InvokePrivateGetRawData(service, "DO");

            Assert.NotNull(di1Data);
            Assert.Equal(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 }, di1Data);

            Assert.NotNull(doData);
            Assert.Equal(new byte[] { 0x99, 0x88, 0x77, 0x66, 0x55, 0x44 }, doData);
        }

        [Fact]
        public void OnDataReceived_비정상_헤더_및_짧은패킷_수신시_무시_검증()
        {
            // Arrange (준비)
            var service = new TcmsTestService(null);
            string validHex = "41A001202608260815" + "112233445566" + "AABBCCDDEEFF" + "1234567890AB" + "998877665544" + "00";
            service.OnDataReceived(validHex); // 초기 정상 데이터 셋팅

            // Act 1: 잘못된 헤더 입력 (1122)
            string invalidHeaderHex = "112201202608260815112233445566AABBCCDDEEFF1234567890AB99887766554400";
            service.OnDataReceived(invalidHeaderHex);

            // Assert 1: 이전 데이터 유지되어야 함
            byte[] di1Data = InvokePrivateGetRawData(service, "DI1");
            Assert.Equal(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 }, di1Data);

            // Act 2: 길이가 짧은 패킷 입력
            string shortHex = "41A001202608260815112233445566";
            service.OnDataReceived(shortHex);

            // Assert 2: 여전히 이전 데이터 유지되어야 함
            di1Data = InvokePrivateGetRawData(service, "DI1");
            Assert.Equal(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 }, di1Data);
        }

        #region Private Reflection Helper

        private byte[] InvokePrivateGetRawData(TcmsTestService service, string category)
        {
            var method = typeof(TcmsTestService).GetMethod("GetCurrentRawData",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (byte[])method?.Invoke(service, new object[] { category });
        }

        #endregion
    }
}