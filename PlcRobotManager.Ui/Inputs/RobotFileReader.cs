using Newtonsoft.Json;
using PlcRobotManager.Core.Infos;
using PlcRobotManager.Util.ExcelUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlcRobotManager.Ui.Inputs
{
    public class RobotFileReader
    {
        private readonly IExcelService excelService = new ExcelService();

        public List<RobotInfo> Read(string robotJsonFilePath)
        {
            if (!File.Exists(robotJsonFilePath))
                throw new ArgumentException(nameof(robotJsonFilePath));
            string jsonText = File.ReadAllText(robotJsonFilePath);
            var robotList = JsonConvert.DeserializeObject<List<RobotJsonItem>>(jsonText);
            return Read(robotList);
        }

        public List<RobotInfo> Read(IEnumerable<RobotJsonItem> robotItems)
        {
            List<RobotInfo> robotInfos = new List<RobotInfo>();
            foreach (var robotItem in robotItems)
            {
                var robotInfo = new RobotInfo()
                {
                    Name = robotItem.Name,
                    GathererType = robotItem.GathererType,
                    DataLoggingCycle = robotItem.DataLoggingCycle,
                    DataLoggingEnabled = robotItem.DataLoggingEnabled,
                    AdditionalIdleTime = robotItem.AdditionalIdleTime,
                    PlcInfos = new List<PlcInfo>()
                };
                robotInfos.Add(robotInfo);

                foreach (var plc in robotItem.PlcList)
                {
                    var plcInfo = new PlcInfo()
                    {
                        Name = plc.Name,
                        ActTargetSimulator = plc.ActTargetSimulator,
                        StationNumber = plc.StationNumber,
                    };
                    plcInfo.DeviceLabelInfos = excelService.Read<DeviceLabelInfo>(plc.LabelFilePath);

                    robotInfo.PlcInfos.Add(plcInfo);
                }
            }
            return robotInfos;
        }
    }
}
