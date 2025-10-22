using System.Linq.Expressions;
using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    User? GetUserByUserName(string userName);

    User? GetById(int id);

    List<User> List();

    User Add(User entity);

    void Update(User entity);

    void Delete(int id);

    int SaveChanges();
    
}