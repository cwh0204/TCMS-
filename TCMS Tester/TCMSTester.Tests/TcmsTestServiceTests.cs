using System;
using System.Reflection;
using Xunit;
using TCMSTester.Services; // TcmsTestService가 위치한 네임스페이스

namespace TCMSTester.Tests
{
    public class TcmsTestServiceTests
    {
        [Fact]
        public void OnDataReceived_정상_Hex패킷_수신시_DI1_DO_파싱_검증()
        {
            // Arrange (준비)
            var service = new TcmsTestService(null);

            // 34바이트 테스트 Hex 패킷 (헤더:41A0, DI1:112233445566, DO:998877665544)
            string dummyHex = "41A001202608260815" + "112233445566" + "AABBCCDDEEFF" + "1234567890AB" + "998877665544" + "00";

            // Act (실행)
            service.OnDataReceived(dummyHex);

            // Assert (검증)
            byte[] di1Data = GetPrivateRawData(service, "DI1");
            byte[] doData = GetPrivateRawData(service, "DO");

            Assert.NotNull(di1Data);
            Assert.Equal(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 }, di1Data);

            Assert.NotNull(doData);
            Assert.Equal(new byte[] { 0x99, 0x88, 0x77, 0x66, 0x55, 0x44 }, doData);
        }

        [Fact]
        public void OnDataReceived_잘못된_헤더_수신시_기존데이터_유지_검증()
        {
            // Arrange
            var service = new TcmsTestService(null);
            string validHex = "41A001202608260815112233445566AABBCCDDEEFF1234567890AB99887766554400";
            service.OnDataReceived(validHex); // 정상 데이터 선입력

            // Act (헤더가 1122인 비정상 패킷 주입)
            string invalidHex = "112201202608260815112233445566AABBCCDDEEFF1234567890AB99887766554400";
            service.OnDataReceived(invalidHex);

            // Assert (파싱 실패로 기존 DI1 데이터가 유지되어야 함)
            byte[] di1Data = GetPrivateRawData(service, "DI1");
            Assert.Equal(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 }, di1Data);
        }

        #region Private Method Invoker Helper

        private byte[] GetPrivateRawData(TcmsTestService service, string category)
        {
            var method = typeof(TcmsTestService).GetMethod("GetCurrentRawData", BindingFlags.NonPublic | BindingFlags.Instance);
            return (byte[])method?.Invoke(service, new object[] { category });
        }

        #endregion
    }
}