namespace ExactTarget.DataExtensions.Core.Shared
{
    public interface ISharedCoreRequestClient
    {
        bool DoesObjectExist(string propertyName, string value, string objectType);
        string RetrieveObjectId(string propertyName, string value, string objectType);
    }
}