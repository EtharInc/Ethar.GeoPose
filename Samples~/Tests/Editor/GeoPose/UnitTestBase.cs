using System.Net.Http;
using Ethar.GeoPose.Authority;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    public class UnitTestBase
    {
        protected HttpClient Client;

        [SetUp]
        public void Setup()
        {
            this.Client = new HttpClient();
            AuthorityProvider.RegisterAuthority(new EtharGeoPoseAuthority());
        }


        [TearDown]
        public void Teardown()
        {
            AuthorityProvider.UnregisterAuthority("/Ethar.GeoPose/1.0");
        }
    }
}