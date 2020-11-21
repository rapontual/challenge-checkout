using System;
using Challenge.Core.DTO;
using Challenge.Core.Model;

namespace Challenge.Data.Repository.Interfaces
{
    public interface ISecurityRepository
    {
        Merchant Authenticate(string login, string password);

        void CreateUser(UserDTO user);
    }
}
