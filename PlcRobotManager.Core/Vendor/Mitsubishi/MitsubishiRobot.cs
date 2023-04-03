using PlcRobotManager.Core.Vendor.Mitsubishi.Gatherers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    public class MitsubishiRobot : IRobot
    {
        private readonly Stopwatch readStopWatch = new Stopwatch();

        private readonly ILogger _logger = LoggerFactory.GetLogger<MitsubishiRobot>();

        /// <summary>
        /// 스레드 슬립 단위시간(밀리초)
        /// </summary>
        private const int SLEEP_BASIS = 100;

        /// <summary>
        /// 재접속 시간(밀리초)
        /// </summary>
        private const int RECONNECT_MIL_SEC = 3000;

        /// <summary>
        /// 작업간 아이들 시간(밀리초)
        /// </summary>
        private const int WORK_IDLE_MIL_SEC = 10;

        /// <summary>
        /// PLC연결 여부
        /// </summary>
        private bool _connected;

        /// <summary>
        /// 작업쓰레드
        /// </summary>
        private Thread _worker;

        /// <summary>
        /// PLC. PLC와 통신한다.
        /// </summary>
        private readonly IMitsubishiPlc _plc;

        /// <summary>
        /// 작업중지 여부
        /// </summary>
        private volatile bool _stop;

        /// <summary>
        /// 이름
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 로그식별을 위한 로봇아이디
        /// </summary>
        private readonly string _logId;

        /// <summary>
        /// 누적 데이터 조회횟수
        /// </summary>
        private long _totalReadCount;

        /// <summary>
        /// 데이터 조회에 사용할 디바이스 라벨
        /// </summary>
        private readonly List<DeviceLabel> _deviceLabels = new List<DeviceLabel>();

        /// <summary>
        /// 가공 데이터 접근을 위한 락커
        /// </summary>
        private readonly object _processedDataLocker = new object();

        /// <summary>
        /// 원본 데이터 접근을 위한 락커
        /// </summary>
        private readonly object _rawDataLocker = new object();

        /// <summary>
        /// 로봇 데이터(가공)
        /// </summary>
        private readonly Dictionary<string, object> _processedData = new Dictionary<string, object>();

        /// <summary>
        /// 로봇 데이터(원본)
        /// </summary>
        private readonly Dictionary<string, short> _rawData = new Dictionary<string, short>();

        /// <summary>
        /// 읽기작업을 진행할 데이터 리더
        /// </summary>
        private readonly IPlcDataGatherer _plcDataGatherer;

        public MitsubishiRobot(string name, IMitsubishiPlc plc, DataGathererType dataGathererType, IEnumerable<DeviceLabel> deviceLabels)
        {
            if (deviceLabels == null) throw new ArgumentNullException(nameof(deviceLabels));

            _name = name;
            _plc = plc;
            _logId = $"[RobotId:{_name}]";

            _deviceLabels.AddRange(deviceLabels);

            switch (dataGathererType)
            {
                case DataGathererType.Auto:
                    _plcDataGatherer = new AutoDataGatherer(plc, deviceLabels); break;
                case DataGathererType.Random:
                    _plcDataGatherer = new RandomDataGatherer(plc, deviceLabels); break;
                case DataGathererType.Manual:
                    _plcDataGatherer = new ManualGatherer(plc, deviceLabels); break;
                default:
                    throw new ArgumentException(nameof(dataGathererType));

            }

            _plcDataGatherer.CycleStarted += (s, e) => CycleStarted?.Invoke(s, e);
            _plcDataGatherer.CycleEnded += (s, e) => CycleEnded?.Invoke(s, e);
            _plcDataGatherer.ValueChanged += (s, e) => ValueChanged?.Invoke(s, e);
        
        }

        public event EventHandler<object> Save;
        public event EventHandler<CycleEventArgs> CycleStarted;
        public event EventHandler<CycleEventArgs> CycleEnded;
        public event EventHandler<ValueChangeEventArgs> ValueChanged;

        public string Name => _name;

        public long TotalReadCount => _totalReadCount;

        public int AdditionalIdleTime { get; set; }
        public bool DataLoggingEnabled { get; set; }
        public int DataLoggingCycle { get; set; } = 1;
        /// <summary>
        /// 실제 적용할 로깅사이클 값
        /// </summary>
        public int ActDataLoggingCycle => DataLoggingCycle < 1 ? 1 : DataLoggingCycle;

        public async Task RunAsync()
        {
            _worker = new Thread(() =>
            {
                DoWork();
            })
            {
                IsBackground = true
            };
            _worker.SetApartmentState(ApartmentState.STA);
            _worker.Start();

            await Task.CompletedTask;
        }

        private void DoWork()
        {
            _plc.Initialize();

            /// 데이터 조회 완료횟수
            int readCycle = 0;
            while (true)
            {
                if (_stop) break;

                if (!Open())
                {
                    SleepWithChecking(RECONNECT_MIL_SEC);
                    continue;
                }

                if (_stop) break;

                var readSuccess = ReadData();
                if (!readSuccess) // 종료 후 재시도
                {
                    Close();
                    SleepWithChecking(RECONNECT_MIL_SEC);
                    continue;
                }

                _logger?.Debug($"{_logId} read data successfully");

                // 저장 조건 확인
                if (SaveRequired()) // 데모 저장
                    Save?.Invoke(this, GetProcessedData());

                if (DataLoggingEnabled && (++readCycle % ActDataLoggingCycle == 0))
                {
                    var text = JsonConvert.SerializeObject(GetRawData());
                    _logger.Info($"{_logId}  {text}");
                }

                SleepWithChecking(WORK_IDLE_MIL_SEC + AdditionalIdleTime);
            }

            if (_connected)
                Close();
        }

        /// <summary>
        /// PLC에 연결
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool Open()
        {
            if (_connected)
                return true;

            var result = _plc.Open();
            _connected = result.IsSuccessful;

            if (_connected)
                _logger?.Info($"{_logId}  succeeded opening");
            else
                _logger?.Info($"{_logId}  faield opening. msg: {result.Message}");

            return _connected;
        }

        private bool ReadData()
        {
            readStopWatch.Restart();
            var result = _plcDataGatherer.Gather();
            _logger.Debug($"{_logId} ReadData ElapsedMilliseconds: {readStopWatch.ElapsedMilliseconds}");
            readStopWatch.Reset();

            if (!result.IsSuccessful)
            {
                _logger?.Error($"{_logId} [read failed] {result.Message}");
                return false;
            }

            SetRawData(_plcDataGatherer.RawData);
            SetProcessedData(_plcDataGatherer.ProcessedData);

            _totalReadCount++;

            return true;
        }

        /// <summary>
        /// 저장 이벤트 발생여부를 확인한다.
        /// </summary>
        /// <returns></returns>
        private bool SaveRequired()
        {
            // 디버그용
            return _totalReadCount != 0 && _totalReadCount % 10 == 0;
        }

        private void Close()
        {
            _plc.Close();
            _connected = false;
            _logger?.Info($"{_logId}  disconnected successfully");
        }

        /// <summary>
        /// 작업중지 여부를 확인하며 스레드 슬립을 진행한다.
        /// </summary>
        /// <param name="milliSeconds"></param>
        private void SleepWithChecking(int milliSeconds)
        {
            int slept = 0;
            while (slept < milliSeconds)
            {
                if (_stop)
                    return;

                int sleepTime = Math.Min(SLEEP_BASIS, milliSeconds - slept);
                Thread.Sleep(sleepTime);

                slept += sleepTime;
            }
        }

        public async Task StopAsync()
        {
            _stop = true;

            await Task.Run(() => _worker.Join());
        }

        public Dictionary<string, object> GetProcessedData()
        {
            var copy = new Dictionary<string, object>();
            lock (_processedDataLocker)
            {
                foreach (var item in _processedData)
                    copy.Add(item.Key, item.Value);
            }
            return copy;
        }

        private void SetProcessedData(IEnumerable<KeyValuePair<string, object>> data)
        {
            lock (_processedDataLocker)
            {
                foreach (var item in data)
                    _processedData[item.Key] = item.Value;
            }
        }

        public Dictionary<string, short> GetRawData()
        {
            var copy = new Dictionary<string, short>();
            lock (_rawDataLocker)
            {
                foreach (var item in _rawData)
                    copy.Add(item.Key, item.Value);
            }
            return copy;
        }

        private void SetRawData(IEnumerable<KeyValuePair<string, short>> data)
        {
            lock (_rawDataLocker)
            {
                foreach (var item in data)
                    _rawData[item.Key] = item.Value;
            }
        }

        public List<string> GetPlcNames()
        {
            return new List<string>() { _plc.Name };
        }

        public List<DeviceLabel> GetDeviceLabels(string plc)
        {
            return _deviceLabels.ToList();
        }
    }
}
