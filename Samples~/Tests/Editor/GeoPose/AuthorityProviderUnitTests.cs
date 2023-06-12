#if !UNITY_EDITOR
using Ethar.GeoPose.Authority;
using Moq;
using NUnit.Framework;

namespace Ethar.GeoPose.UnitTests
{
    [TestFixture]
    internal class AuthorityProviderUnitTests
    {
        [Test]
        public void CanProperlyRegisterAuthorities()
        {
            var auth1 = new Mock<IAuthority>();
            auth1.Setup(x => x.AuthorityName).Returns("test 1");

            var auth2 = new Mock<IAuthority>();
            auth2.Setup(x => x.AuthorityName).Returns("test 2");

            var auth3 = new Mock<IAuthority>();
            auth3.Setup(x => x.AuthorityName).Returns("test 3");

            AuthorityProvider.RegisterAuthority(auth1.Object);
            AuthorityProvider.RegisterAuthority(auth2.Object);
            AuthorityProvider.RegisterAuthority(auth3.Object);

            Assert.That(AuthorityProvider.Authorities.Count, Is.EqualTo(3));
            Assert.That(AuthorityProvider.Authorities.Any(a => string.Equals(a.AuthorityName, "test 1")));
            Assert.That(AuthorityProvider.Authorities.Any(a => string.Equals(a.AuthorityName, "test 2")));
            Assert.That(AuthorityProvider.Authorities.Any(a => string.Equals(a.AuthorityName, "test 3")));
        }

        [Test]
        public void CanProperlyUnregisterAuthorities()
        {
            var auth1 = new Mock<IAuthority>();
            auth1.Setup(x => x.AuthorityName).Returns("test 1");

            var auth2 = new Mock<IAuthority>();
            auth2.Setup(x => x.AuthorityName).Returns("test 2");

            var auth3 = new Mock<IAuthority>();
            auth3.Setup(x => x.AuthorityName).Returns("test 3");

            AuthorityProvider.RegisterAuthority(auth1.Object);
            AuthorityProvider.RegisterAuthority(auth2.Object);
            AuthorityProvider.RegisterAuthority(auth3.Object);

            Assert.That(AuthorityProvider.Authorities.Count, Is.EqualTo(3));

            AuthorityProvider.UnregisterAuthority("test 1");

            Assert.That(AuthorityProvider.Authorities.Count, Is.EqualTo(2));
            Assert.That(AuthorityProvider.Authorities.Any(a => string.Equals(a.AuthorityName, "test 2")));
            Assert.That(AuthorityProvider.Authorities.Any(a => string.Equals(a.AuthorityName, "test 3")));

            AuthorityProvider.UnregisterAuthority("test 2");

            Assert.That(AuthorityProvider.Authorities.Count, Is.EqualTo(1));
            Assert.That(AuthorityProvider.Authorities.Any(a => string.Equals(a.AuthorityName, "test 3")));

            AuthorityProvider.UnregisterAuthority("test 3");

            Assert.That(AuthorityProvider.Authorities.Count, Is.EqualTo(0));
        }
    }
}
#endif