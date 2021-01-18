﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Models;
using Core.DomainServices.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace WebApi.Tests.Mocks.Extends
{
    public class MockGenericExtension
    {
        public static void ExtendMock<T>(Mock<IRepository<T>> repository, List<T> entityList) where T: BaseEntity
        {
            repository.Setup(x => x.Update(It.IsAny<T>(), It.IsAny<IdentityUser>()))
                .Returns((T a, IdentityUser user) => Task.FromResult(
                    entityList.Where(c => c.Id.ToString() == a.Id.ToString()).Select(d =>
                    {
                        d.UpdatedAt = DateTime.Now;
                        return d;
                    }).Single()
                ));
        }
    }
}
