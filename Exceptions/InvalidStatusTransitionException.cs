namespace IMS.Exceptions
{
    public class InvalidStatusTransitionException : Exception
    {
        public InvalidStatusTransitionException(string currentStatus, string attemptedStatus)
            : base($"Cannot transition Purchase Order from '{currentStatus}' to '{attemptedStatus}'.")
        {
        }
    }
}