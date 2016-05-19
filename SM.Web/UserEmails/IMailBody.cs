namespace SM.Web.UserEmails
{
    public interface IMailBody
    {
        string TransformText();
        string Title { get; }
    }
}
