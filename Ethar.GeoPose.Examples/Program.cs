// Runtime execution of the GeoPose examples
//
//  1. Basic GeoPose data deserialization from Solarpose data source.
//  2. Serialzing GeoPose SDU through an Authority for posting.
//  3. Deserializing GeoPose JSON and constucting an SDU.
//
// See https://etharinc.github.io/Ethar.GeoPose.Docs/ for more information

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Ethar.GeoPose.Examples;

// The TLS certificate of geopose.io, which hosts the SolarPose service, expired on 1 January 2026. Until it is renewed, this
// program accepts that one failure for that one host. The host name and the chain of trust are still checked. Remove this once
// the certificate is renewed.
using var solarPoseClient = new HttpClient(new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (request, certificate, chain, errors) =>
        errors == SslPolicyErrors.None ||
        (request.RequestUri?.Host == "geopose.io" &&
         errors == SslPolicyErrors.RemoteCertificateChainErrors &&
         chain is { ChainStatus.Length: > 0 } &&
         chain.ChainStatus.All(status => status.Status == X509ChainStatusFlags.NotTimeValid)),
});

Console.WriteLine("-----Start-----");

Console.WriteLine("Getting GeoPose data and deserializing");
var result = await Example_BasicSerialization.DeSerialiseGeoPose(solarPoseClient);
Console.WriteLine($"Geopose data - position {result.Position} - angles {result.Angles}");

Console.WriteLine("----------");

Console.WriteLine("Serializing SDU");
var jsonResult = Example_AuthorityImplementation.SerializeExample();
Console.WriteLine($"Serialised Data {jsonResult}");

Console.WriteLine("----------");

Console.WriteLine("Deserializing SDU");
var sduResult = Example_AuthorityImplementation.DeserializeExample();
Console.WriteLine($"Deserialized Data {sduResult}");

Console.WriteLine("-----End-----");
