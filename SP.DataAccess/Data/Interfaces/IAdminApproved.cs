namespace SP.DataAccess.Data.Interfaces
{
    public interface IAdminApproved : ICreated
    {
        bool AdminApproved { get; set; }
    }
}
