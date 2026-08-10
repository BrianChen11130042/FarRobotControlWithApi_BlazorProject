namespace FarRobotControlWithApi_BlazorProject.DTOModel
{
    public class AccessTokenDto
    {
        public string accessToken{ get; set; } = string.Empty;

        public string tokenType { get; set; } = string.Empty;

        public DateTime retrieveTime { get; set;} = DateTime.Now.AddDays(-30);
    }
}
