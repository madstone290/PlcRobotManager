namespace PlcRobotManager.Core
{
    /// <summary>
    /// 명령 처리 결과
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static Result<T> Success(T data) => new Result<T>() {  IsSuccessful = true, Data = data };
        public static Result<T> Fail(string message) => new Result<T>() { IsSuccessful = false, Message = message};
    }

    /// <summary>
    /// 제네릭 타입을 사용하지 않는 결과
    /// </summary>
    public class Result : Result<bool>
    {
        
    }

    
}
