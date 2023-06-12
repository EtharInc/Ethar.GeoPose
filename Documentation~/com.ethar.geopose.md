<!-- Offline documentation -->
# Ethar GeoPose implementation documentation

<p align="center">
    <img alt="GeoPose Logo" src="https://etharinc.github.io/Ethar.GeoPose.Docs/images/branding/GeoPoseLogoWhiteRGB.svg" width="500" height="500">
</p>

The OGC GeoPose Standard defines the encodings for the real-world position and orientation of a real or a digital object in a machine-readable form, using GeoPose enables the easy integration of digital elements on and in relation to the surface of the planet.

Ultimately at its core, GeoPose is a standard to define positions in space collectively with their relative "real-world" rotation to define digital objects placed/located in the world.

[Ethar Inc.](https://www.ethar.com/) has fulfilled this vision with a C# library to accelerate the adoption of the [GeoPose standard](https://docs.ogc.org/dis/21-056r10/21-056r10.html), including a Unity extension to make adoption in the most popular game engine even easier.

> [To read more about Ethar's GeoPose implementation, check the documentation site for more information](https://etharinc.github.io/Ethar.GeoPose.Docs)

## Who is Ethar?

[Ethar, Inc.](https://www.ethar.com/) is a spatial computing company focused on delivering tools for the entire life-cycle of XR content. We believe the future of spatial computing is open and cooperative, we strive to incorporate open and interoperable technology standards into the very foundation of our products.

Along the way we have built some useful tooling, that we want to share with the community, and encourage the adoption of open standards. This is our implementation of GeoPose. We think it is one of the most important building blocks of an open, interoperable & decentralized spatial web.

## Outline Scope

The GeoPose standard is made up of several key concepts, namely:

* [A Pose, or fixed position for an object.](#pose)
* [The Orientation or direction of a posed object.](#orientation)
* [GeoPose Structural Data Units (SDU) to describe locational metadata.](#structural-data-units)
* [A Frame Specification for GeoPose Data.](#frame-specifications)
* [A GeoPose Authority](#geopose-authorities).

> Additional terms and concepts are covered in [Section 4](https://docs.ogc.org/dis/21-056r10/21-056r10.html#toc15) of the GeoPose specification.

### Pose

At the core of the GeoPose definition is the concept of a Pose, that being a position or place where an object is located.  A Pose may be determined by a coordinate system relative to a physical location, which may be a GPS, cartesian, image tracked or SLAM position.

The position is absolute at the time of creation and updated as the need arises.

### Orientation

To define the actual direction in which a GeoPosed object is placed, a direction is needed to denote the physical orientation of a placed object.  The orientation of objects relative to their position is one of the key factors that sets the GeoPose standard apart from other positioning solutions.

### Structural Data Units

The base definition of a GeoPose construct is its ```Structural Data Unit``` definition, which outlines the serialized data that is shared between entities to communicate GeoPosed data.  By default, there are 8 Structural Data Units defined within the [GeoPose standard](https://docs.ogc.org/dis/21-056r10/21-056r10.html#toc45), which are:

* Basic YawPitchRoll - Basic positioning using WGS84 coordinates for position and Euler angles for orientation.
* Basic Quaternion - Basic positioning using WGS84 coordinates for position and a Quaternion for orientation.
* Advanced - An advanced concept utilizing a [Frame Specification](#frame-specifications) that defines a reference frame for an object.
* Graph - An SDU that contains a directed acyclic graph representation of the transformational relationships between reference frames defined by [Frame Specifications](#frame-specifications).
* Chain - An SDU that represents a linear sequence of poses linked by full 6DoF transformations, with the first frame in the sequence being extrinsic.
* Regular Series - An ordered set of operations to perform on a GeoPosed object, complete with timed events.
* Irregular Series - An unordered set of operations for use on a GeoPosed Object.
* Stream - Another advanced use case whereby complex operations can be structured, such as animation.

Structural Data Units consist of base GeoPose Data Types and can contain one or more [Frame Specifications](#frame-specifications) for extending a GeoPosed Object.  Which type of SDU you use will largely depend on your use case, and there is always the option of creating your own (at the cost of interoperability).

> See the [GeoPose standard](https://docs.ogc.org/dis/21-056r10/21-056r10.html#toc45) section on Structural Data Units for more information.

### Frame Specifications

A ```Frame Specification``` can take on multiple roles or responsibilities for objects placed using a GeoPose, these can range from Transform Animations, waypoints, relativistic placement and structure.  Which frame specifications are used will be largely determined by the use case required and the [SDU](#structural-data-units) that has been implemented.

At its most simplistic level, Frame Specifications are references used to co-locate a GeoPose object in relation to another physical entity (such as the Earth) or another GeoPose.  In advanced cases they can be used to infer animation, or the bounds of a GeoPosed object.

Unlike SDU's however, Frame Specifications require an authority who is responsible for orchestrating the content of the specification and ultimately, controls how the data is assembled and disassembled for transport. (Different organisations may implement different authorities for managing how they interpret and expose GeoPosed data.)

> See the [GeoPose standard](https://docs.ogc.org/dis/21-056r10/21-056r10.html#term-frame-specification) section on Frame Specifications for more information.

### GeoPose Authorities

An ```Authority``` in the GeoPose standard is the entity responsible for the understanding, conversion and transformation of any Frame Specification. It is defined in the Ethar GeoPose library through an Interface designed to enforce a specific pattern for Authority Implementations, as shown below:

[IAuthority Interface](https://etharinc.github.io/Ethar.GeoPose.Docs/images/architecture/IAuthority-Interface.png)<br/>
*figure 1: IAuthority Interface.*

The interface defines a single property and several methods required by an Authority for operation, namely:

* Authority Name - The unique name/identifier for the authority in the form of ```"/geopose/1.0"```
* ConvertJsonToFrameSpec - Method to take in a GeoPose Frame Specification JSON string and output a Frame Specification definition.
* ConvertFrameSpecToJson - Method to take a Frame Specification object and turn it into serialized GeoPose Frame Specification JSON string.
* ConvertJsonToTransitionModel - Method to take a [Transition Model](https://docs.ogc.org/dis/21-056r10/21-056r10.html#toc17) JSON string and output a Transition Model definition.
* ConvertTransitionModelToJson - Method to serialize a [Transition model](https://docs.ogc.org/dis/21-056r10/21-056r10.html#toc17) into a specific GeoPose Transition Model JSON string

> Additionally, it is recommended to also implement a ```FrameSpecificationValidator``` as part of any Authority implementation, to validate any Frame Specifications and handle any irregularities with incoming data.

You can see a fully implemented authority implementation [here](https://github.com/EtharInc/Ethar.GeoPose/blob/develop/Ethar.GeoPose.Authority/EtharGeoPoseAuthority.cs) or refer to the ```Ethar GeoPose Authority Sample``` included with this package.

It is Critical, when implementing your own authority, to define the frame specification that the authority is responsible for, including the data types (either c# native or GeoPose elements) and then write ```Convertors``` to extract and understand the Frame specifications being handled by the authority, with specific attention to use the ```ValidationUtilities``` provided by the Ethar GeoPose library, for example:

```csharp
public static ExampleExtrinsicFrameSpec ConvertJObjectToExampleExtrinsicFrameSpec(JObject jObject)
{
    // Validated the incoming json object string and checks that it has the required values and also
    // checks if this is the authority mentioned in the incoming data that handles the frame specification.
    if (ValidationUtilities.ValidateJsonObjectParameters(jObject, Constants.AuthorityName, out var queryString))
    {
        // Retrieves the required data from the json to construct the Frame Specification.
        var lat = float.Parse(queryString.Get("latitude"));
        var lon = float.Parse(queryString.Get("longitude"));

        // Returns a new instance of the Frame Specification with the data populated.
        return new ExampleExtrinsicFrameSpec { Latitude = lat, Longitude = lon };
    }

    return null;
}
```

*figure 2: Authority Frame Specification conversion.*

This ensures that all serialization and deserialization is handled automatically by the authority whenever a Frame Specification (or SDU containing a Frame Specification) is used.

> For a more detailed explanation, check the [Ethar GeoPose documentation](https://etharinc.github.io/Ethar.GeoPose.Docs).

### Authority Provider

To ensure the successful use and access to any implemented GeoPose Authorities, an ```AuthorityProvider``` class is provided as part of this package to Register, Request and remove active Authorities in your solution, this provides a "Single Path" for querying authorities when evaluating incoming data.

Under the hood, the Authority Provider is simply a Safe Dictionary implemented within a Static class which has proven useful for API-level access within a project.

The surface of the Authority Provider is as follows:

![IAuthority Interface](https://etharinc.github.io/Ethar.GeoPose.Docs/images/architecture/AuthorityProvider-utility.png)<br/>
*figure 3: Authority Provider.*

The utility defines a single exposed property and several methods to safely access the dictionary, namely:

* Authorities - Read only list of registered authorities.
* RegisterAuthority - Used to register an Authority instance as active.
* UnregisterAuthority - Used to remove an Authority from active use and dispose of it.
* GetAuthority - Safe method for retrieving an Authority by its Name, returns null if not found.

Use of the AuthorityProvider to manage access to Authorities is recommended when handling incoming GeoPose data to ensure quick and safe access.

> For more detailed examples of authority registration and use, check the [Ethar GeoPose documentation](https://etharinc.github.io/Ethar.GeoPose.Docs).

## Further Information

The [Ethar GeoPose documentation](https://etharinc.github.io/Ethar.GeoPose.Docs) contains more information on the development and use of this library.  Any issues or queries should be directed to the [Ethar GeoPose Git repository here](https://github.com/EtharInc/Ethar.GeoPose/issues).

We hope this library proves useful to all developers and we welcome contribution and support as it evolves alongside the [GeoPose Specification](https://docs.ogc.org/dis/21-056r10/21-056r10.html).

*All rights reserved [Ethar Inc.](https://ethar.com) 2023.*
