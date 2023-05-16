namespace Domain.Entities.Interfaces
{
    public interface IAuthorable
    {
        string AuthorId { get; set; }
        Author Author { get; set; }
    }
}