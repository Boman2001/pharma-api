using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Moq;

namespace WebApi.Tests.Mocks
{
   public static class MockGenericRepository
    {
        public static Mock<IRepository<T>> GetUserInformationMock<T>(List<T> usersInformationList) where T : BaseEntity
        {
            var repository = new Mock<IRepository<T>>();

            repository.Setup(x => x.Get())
                .Returns(usersInformationList);

            repository.Setup(t => t.Get(It.IsAny<int>()))
                .ReturnsAsync((int s) => usersInformationList.FirstOrDefault(x => x.Id == s));

            repository.Setup(x => x.Add(It.IsAny<T>()))
                .Callback((T t) => usersInformationList.Add(t))
                .Returns(Task.FromResult(It.IsAny<T>()));

            repository.Setup(x => x.Delete(It.IsAny<int>()))
                .Callback((int t) => usersInformationList.RemoveAt(t))
                .Returns(Task.FromResult(0));

            return repository;
        }
    }
}
