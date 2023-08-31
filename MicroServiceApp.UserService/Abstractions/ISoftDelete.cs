namespace MicroServiceApp.UserService.Abstractions;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}