using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace TCMSTester.Config
{
    public static class AppConfigManager
    {
        /// <summary>
        /// 지정한 경로의 JSON 설정 파일을 읽어 AppConfig 객체로 역직렬화합니다.
        /// 실패 또는 경로 오류 시 Visual Studio 출력(Output) 창에 디버그 로그를 출력합니다.
        /// </summary>
        public static AppConfig LoadConfig(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"[ConfigError] 파일이 존재하지 않습니다. 절대 경로: {Path.GetFullPath(filePath)}");
                    return new AppConfig();
                }

                string json = File.ReadAllText(filePath);
                var config = JsonConvert.DeserializeObject<AppConfig>(json);

                if (config == null)
                {
                    Debug.WriteLine("[ConfigError] JSON 역직렬화 결과가 null입니다.");
                    return new AppConfig();
                }

                Debug.WriteLine($"[ConfigSuccess] 설정 로드 성공 - Inputs: {config.DigitalInputs.Count}개, Outputs: {config.DigitalOutputs.Count}개");
                return config;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ConfigError] JSON 파싱 중 예외 발생: {ex.Message}");
                return new AppConfig();
            }
        }
    }
}