namespace IMS.Enums
{
    public enum POStatus
    {
        Draft = 0,             // being built, not sent anywhere
        Submitted = 1,         // sent, awaiting approval
        Approved = 2,          
        PartiallyReceived = 3, 
        FullyReceived = 4,    
        Closed = 5,         
        Cancelled = 6         
    }
}
