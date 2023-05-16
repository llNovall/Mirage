namespace Domain.Services
{
    public interface IAzureStorageService
    {
        Task<string> UploadImageAsync(string imgType, Stream imgStream);
    }
}