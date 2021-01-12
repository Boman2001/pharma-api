using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Moq;

namespace WebApi.Tests.Mocks.Extends
{
    public static class MockDoctorExtension
    {
        public static void ExtendMock(Mock<IRepository<UserInformation>> repository, List<UserInformation> usersInformationList)
        {
            repository.Setup(x => x.Update(It.IsAny<UserInformation>()))
                .Returns((UserInformation a) => Task.FromResult(
                    usersInformationList.Where(c => c.Id == a.Id).Select(d =>
                    {
                        d.User.UserName = a.User.UserName;
                        return d;
                    }).Single()
                ));
        }
    }
}
