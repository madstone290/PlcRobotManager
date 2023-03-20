namespace PlcRobotManager.Core
{
    /// <summary>
    /// 데이터 영속성 관리
    /// </summary>
    public interface IDataManager
    {
        void Save(object data);
    }
}
