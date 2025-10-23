using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Core.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    public UserRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public User? GetUserByUserName(string userName)
    {

        return _applicationDbContext.Users.SingleOrDefault(p => p.UserName == userName);
    }

  
      public User? GetById(int id)
    {
        return _applicationDbContext.Users.Find(id);
    }

    public List<User> List()
    {
        return _applicationDbContext.Users.ToList();
    }

    public User Add(User entity)
    {
        _applicationDbContext.Users.Add(entity);
        return entity;
    }

    public void Update(User entity)
    {
        _applicationDbContext.Users.Update(entity);
    }

    public void Delete(int id)
    {
        var user = _applicationDbContext.Users.Find(id);
        if (user != null)
        {
            _applicationDbContext.Users.Remove(user);
        }
    }

  public int SaveChanges()
    {
        return _applicationDbContext.SaveChanges();
    }

}

