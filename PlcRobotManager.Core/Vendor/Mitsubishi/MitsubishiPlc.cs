using ActProgTypeLib;
using ActSupportMsgLib;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    /// <summary>
    /// 미쯔비시 PLC. ActProgType을 이용하여 통신을 한다.
    /// <para>ActProgType의 인스턴스는 STA환경에서만 올바르게 작동한다. 인스턴스 생성 및 실행 모두 동일한 STA스레드에서 이루어져야 한다</para>
    /// </summary>
    public class MitsubishiPlc : IMitsubishiPlc
    {
        private readonly ILogger _logger = LoggerFactory.GetLogger<MitsubishiPlc>();

        private readonly string _logId;

        /// <summary>
        /// 미쯔비시 프로그램 모듈
        /// </summary>
        private ActProgType _progModule;

        /// <summary>
        /// 메시지 지원 모듈
        /// </summary>
        private ActSupportMsg _msgModule;

        /// <summary>
        /// PLC식별을 위한 이름
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// ActProgType생성에 사용할 옵션
        /// </summary>
        private readonly ProgOptions _options;

        public MitsubishiPlc(string name, ProgOptions options)
        {
            _name = name;
            _options = options;
            _logId = $"[MitsubishiPlc:{_name}]";
        }
    
        /// <summary>
        /// 초기화 작업을 수행한다.
        /// STA스레드 안에서 호출되어야 한다.
        /// </summary>
        public void Initialize()
        {
            _progModule = new ActProgType();
            _msgModule = new ActSupportMsg();

            _progModule.ActTargetSimulator = _options.ActTargetSimulator;
            _progModule.ActUnitType = _options.ActUnitType;
            _progModule.ActProtocolType = _options.ActProtocolType;
        }

        public Result Open()
        {
            var code = _progModule.Open();
            _msgModule.GetErrorMessage(code, out string message);

            return new Result()
            {
                IsSuccessful = code == 0,
                Message = message,
            };
        }

        public Result Close()
        {
            _progModule.Close();

            return new Result()
            {
                IsSuccessful = true
            };
        }

        public Result<short[]> ReadBlock2(string startAddress, int length)
        {
            var data = new short[length];
            var code = _progModule.ReadDeviceBlock2(startAddress, length, out data[0]);
            _msgModule.GetErrorMessage(code, out string message);
            
            _logger?.Debug($"{_logId} result of 'ReadBlock2': {code==0}");

            return new Result<short[]>()
            {
                IsSuccessful = code == 0,
                Message = message,
                Data = data
            };
        }

        public Result<int[]> ReadBlock4(string startAddress, int length)
        {
            var data = new int[length];
            var code = _progModule.ReadDeviceBlock(startAddress, length, out data[0]);
            _msgModule.GetErrorMessage(code, out string message);

            _logger?.Debug($"{_logId} result of 'ReadBlock4': {code == 0}");

            return new Result<int[]>()
            {
                IsSuccessful = code == 0,
                Message = message,
                Data = data
            };
        }

        public Result<short[]> ReadRandom(string randomAddress, int length)
        {
            var data = new short[length];
            var code = _progModule.ReadDeviceRandom2(randomAddress, length, out data[0]);
            _msgModule.GetErrorMessage(code, out string message);

            _logger?.Debug($"{_logId} result of 'ReadRandom': {code == 0}");

            return new Result<short[]>()
            {
                IsSuccessful = code == 0,
                Message = message,
                Data = data
            };
        }

  
    }
}
