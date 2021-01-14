using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Moq;

namespace WebApi.Tests.Mocks.Extends
{
    using Microsoft.AspNetCore.Identity;

    public static class MockUserExtension
    {
        public static void ExtendMock(Mock<IRepository<UserInformation>> repository,
            IEnumerable<UserInformation> usersInformationList)
        {
            repository.Setup(x => x.Update(It.IsAny<UserInformation>(), It.IsAny<IdentityUser>()))
                .Returns((UserInformation a) => Task.FromResult(
                    usersInformationList.Where(c => c.UserId.ToString() == a.UserId.ToString()).Select(d =>
                    {
                        d.Name = a.Name;
                        return d;
                    }).Single()
                ));
        }
    }
}