using System.Linq;
using Challenge.Core.DTO;
using Challenge.Core.Model;
using Challenge.Data.Repository.Interfaces;

namespace Challenge.Data.Repository
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly ChallengeDbContext dbContext;

        public SecurityRepository(ChallengeDbContext context)
        {
            this.dbContext = context;
        }

        public Merchant Authenticate(string login, string password)
        {
            var merchant = dbContext.Merchant
                .FirstOrDefault(m =>
                    m.Login == login &&
                    m.Password == password);

            return merchant;
        }

        public void CreateUser(UserDTO user)
        {
            var merchant = new Merchant
            {
                Login = user.Login,
                Password = user.Password,
                Name = user.Name,
                IsAdmin = false
            };

            dbContext.Merchant
                .Add(merchant);

            dbContext.SaveChanges();
        }
    }
}
