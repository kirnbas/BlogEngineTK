using System;
using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogEngineTK.WebUI.Controllers;
using System.Collections.Generic;
using Moq;
using System.Linq;
using BlogEngineTK.Domain.Entities;
using BlogEngineTK.Domain.Repositories;

namespace BlogEngineTK.Tests
{
    /// <summary>
    /// Mock of IPostRepository with generated posts for unit-tests
    /// </summary>
    class MockPostRepository
    {
        /// <summary>
        /// Стандартное кол-во инициализируемых элементов в репозитории (исп-те для оптимизации).
        /// Лучше не менять значение, т.к. на него уже несколько завязаны некоторые тесты
        /// </summary>
        public const int STDN = 10;

        private static Mock<IPostRepository> mock = new Mock<IPostRepository>();
        private static Post[] posts;

        public static Mock<IPostRepository> Mock { get { return mock; } }
        
        /// <summary>
        /// Возвращает тестовый IPostRepository с инициализированными полями PostId, Name, Price : (1, ..., n)
        /// </summary>
        /// <param name="n">С каким кол-вом постов инициализировать репозиторий</param>
        public static IPostRepository GetRepository(int n)
        {
            if (posts == null || posts.Length != n)
            {
                posts = new Post[n];
                for (int i = 1; i <= n; i++)
                {
                    posts[i - 1] = new Post
                    {
                        PostId = i,
                        CreatedDate = DateTime.Now,
                        Header = i.ToString(),
                        ShortText = "short" + i,
                    };                    
                }
            }

            for (int i = 0; i < posts.Length; i++)
            {
                posts[i].CreatedDate = posts[i].CreatedDate.AddDays(i);
            }

            mock = new Mock<IPostRepository>();

            mock.Setup(m => m.Posts)
                .Returns(posts.AsQueryable());

            return mock.Object;
        }
    }
}
