namespace WebApplication.Definitions
{
    public class PublisherConfiguration
    {
        public StatusNotificationMethod WorkItemStatusNotificationMethod { get; set; }

        public string CallbackUrlBase { get; set; }

        public enum StatusNotificationMethod
        {
            /// <summary>
            /// Use polling.
            /// </summary>
            UsePolling,

            /// <summary>
            /// UseCallback.
            /// </summary>
            UseCallback
        }
    }
}