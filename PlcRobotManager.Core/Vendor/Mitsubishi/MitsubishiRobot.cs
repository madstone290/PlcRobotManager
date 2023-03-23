﻿using PlcRobotManager.Core.Vendor.Mitsubishi.Gatherers;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PlcRobotManager.Core.Vendor.Mitsubishi
{
    public class MitsubishiRobot : IRobot
    {
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
        /// 로봇 데이터(가공)
        /// </summary>
        private readonly ConcurrentDictionary<string, object> _processedData = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 로봇 데이터(원본)
        /// </summary>
        private readonly ConcurrentDictionary<string, short> _rawData = new ConcurrentDictionary<string, short>();

        /// <summary>
        /// 읽기작업을 진행할 데이터 리더
        /// </summary>
        private readonly IPlcDataGatherer _plcDataReader;

        public MitsubishiRobot(string name, IMitsubishiPlc plc, DataGathererType dataGathererType, IEnumerable<DeviceLabel> deviceLabels)
        {
            if(deviceLabels == null) throw new ArgumentNullException(nameof(deviceLabels));

            _name = name;
            _plc = plc;
            _logId = $"[RobotId:{_name}]";

            _deviceLabels.AddRange(deviceLabels);

            switch (dataGathererType)
            {
                case DataGathererType.Auto:
                    _plcDataReader = new AutoDataGatherer(plc, deviceLabels);break;
                case DataGathererType.Random:
                    _plcDataReader = new RandomDataGatherer(plc, deviceLabels); break;
                case DataGathererType.Manual:
                    _plcDataReader = new ManualGatherer(plc, deviceLabels); break;
                default:
                    throw new ArgumentException(nameof(dataGathererType));

            }

        }

        public event EventHandler<object> Save;

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

            Stopwatch sw = new Stopwatch();
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

                sw.Restart();
                var readSuccess = ReadData();
                _logger.Debug($"{_logId} ReadData ElapsedMilliseconds: {sw.ElapsedMilliseconds}");

                if (!readSuccess)
                {
                    Close();
                    SleepWithChecking(RECONNECT_MIL_SEC);
                    continue;
                }
                else
                {
                    _logger?.Debug($"{_logId} read data successfully");

                    if (DataLoggingEnabled && (++readCycle % ActDataLoggingCycle == 0))
                    {
                        var data = new Dictionary<string, object>(_processedData);
                        var text = JsonConvert.SerializeObject(data);
                        _logger.Info($"{_logId}  {text}");
                    }
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
            var result = _plcDataReader.Gather();
            if (!result.IsSuccessful)
            {
                _logger?.Error($"{_logId} [read failed] {result.Message}");
                return false;
            }

            foreach(var pair in _plcDataReader.RawData)
            {
                _rawData[pair.Key] = pair.Value;
            }

            foreach (var pair in _plcDataReader.ProcessedData)
            {
                _processedData[pair.Key] = pair.Value;
            }

            _totalReadCount++;

            // 데모 저장
            if (_totalReadCount != 0 && _totalReadCount % 10 == 0)
                Save?.Invoke(this, new Dictionary<string, object>(_processedData));

            return true;
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
            return new Dictionary<string, object>(_processedData);
        }

        public Dictionary<string, short> GetRawData()
        {
            return new Dictionary<string, short>(_rawData);
        }
    }
}
