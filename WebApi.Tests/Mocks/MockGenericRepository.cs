﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Moq;

namespace WebApi.Tests.Mocks
{
    using Microsoft.AspNetCore.Identity;

    public static class MockGenericRepository
    {
        public static Mock<IRepository<T>> GetUserInformationMock<T>(List<T> usersInformationList) where T : BaseEntity
        {
            var repository = new Mock<IRepository<T>>();

            repository.Setup(x => x.Get())
                .Returns(usersInformationList);

            repository.Setup(x => x.Get(It.IsAny<IEnumerable<string>>()))
                .Returns(usersInformationList);

            repository.Setup(x => x.Get(It.IsAny<int>(),It.IsAny<IEnumerable<string>>()))
                .Returns((int s, IEnumerable<string> _) => usersInformationList.FirstOrDefault(x => x.Id == s));

            repository.Setup(t => t.Get(It.IsAny<int>()))
                .ReturnsAsync((int s) => usersInformationList.FirstOrDefault(x => x.Id == s));

            repository.Setup(x => x.Add(It.IsAny<T>(), It.IsAny<IdentityUser>()))
                .Callback((T t, IdentityUser _) =>
                {
                    usersInformationList.Add(t);
                })
                .Returns(Task.FromResult(It.IsAny<T>()));

            repository.Setup(x => x.Delete(It.IsAny<int>(), It.IsAny<IdentityUser>()))
                .Callback((int t, IdentityUser _) =>
                {
                    usersInformationList.RemoveAt(t);
                })
                .Returns(Task.FromResult(It.IsAny<T>()));

            repository.Setup(t => t.Get(It.IsAny<Expression<Func<T, bool>>>()))
                .Returns((Expression<Func<T, bool>> query) => usersInformationList.AsQueryable().Where(query).ToList());




            return repository;
        }
    }
}