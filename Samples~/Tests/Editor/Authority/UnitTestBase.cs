using Ethar.GeoPose.Authority;
using NUnit.Framework;
using System.Net.Http;

namespace Ethar.GeoPose.Authority.UnitTests
{
    public class UnitTestBase
    {
        protected HttpClient Client;

        [OneTimeSetUp]
        public void Setup()
        {
            this.Client = new HttpClient();
            AuthorityProvider.RegisterAuthority(new EtharGeoPoseAuthority());
        }
    }
}