namespace Compass.Services
{
    public class ServiceResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public object Message { get; set; }
        public object Payload { get; set; }
        public bool IsSuccess { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
