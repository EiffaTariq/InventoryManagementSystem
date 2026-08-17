namespace IMS.Enums
{
    public enum POStatus
    {
        Draft = 0,             // being built, not sent anywhere
        Submitted = 1,         // sent, awaiting approval
        Approved = 2,          // authorized, waiting on supplier to ship
        PartiallyReceived = 3, // some goods physically arrived
        FullyReceived = 4,     // all goods physically arrived
        Closed = 5,            // successfully completed & reconciled
        Cancelled = 6          // terminated before completion
    }
}
