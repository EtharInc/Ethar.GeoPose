// Runtime execution of the GeoPose examples
//
//  1. Basic GeoPose data deserialization from Solarpose data source.
//  2. Serialzing GeoPose SDU through an Authority for posting.
//  3. Deserializing GeoPose JSON and constucting an SDU.
//
// See https://etharinc.github.io/Ethar.GeoPose.Docs/ for more information

using Ethar.GeoPose.Examples;

Console.WriteLine("-----Start-----");

Console.WriteLine("Getting GeoPose data and deserializing");
var result = await Example_BasicSerialization.DeSerialiseGeoPose();
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
