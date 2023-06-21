<center>
<picture align="center">
    <source media="(prefers-color-scheme: dark)" srcset="https://raw.githubusercontent.com/EtharInc/Ethar.GeoPose/main/Images/branding/GeoPoseLogoWhiteRGB.svg">
    <source media="(prefers-color-scheme: light)" srcset="https://raw.githubusercontent.com/EtharInc/Ethar.GeoPose/main/Images/branding/GeoPoseLogoBlackRGB.svg">
    <img alt="GeoPose Logo" src="https://raw.githubusercontent.com/EtharInc/Ethar.GeoPose/main/Images/branding/GeoPoseLogoDarkBlueColor.svg" width="500" height="500">
</picture>
</center>

# GeoPose Authority Implementation by [Ethar, Inc.](https://www.ethar.com/)

[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/EtharInc/Ethar.GeoPose/blob/main/LICENSE)

The [GeoPose standard](https://docs.ogc.org/dis/21-056r10/21-056r10.html) enables the easy integration of digital elements on and in relation to the surface of the planet.

Ethar.GeoPose.Authority is a C# implementation of a GeoPose authority whose responsibility is to manage, convert and maintain Frame Specifications used within a GeoPose definition.

## Table of Contents

- [Introduction](#introduction)
- [Who is Ethar](#who-is-ethar)
- [Features](#ethar-geopose-authority-implementation-features)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [Terms of Use](#terms-of-use)
- [License](#license)

## Introduction

Ethar.GeoPose is a convenient library written in C# for performing GeoPose calculations and integrating them into your applications, the Ethar.GeoPose.Authority provides a full Authority implementation based on the GeoPose standard and implements the 5 main Frame Specifications outlined by the specification.

GeoPose is a standard stewarded by the [Open Geospatial Consortium](https://www.ogc.org/) & supported by members of [Open AR Cloud](https://www.openarcloud.org/) which defines the encodings for the real world position and orientation of a real or a digital object in a machine-readable form.

The GeoPose standard describes a geographically-anchored pose (GeoPose) with 6 degrees of freedom referenced to one or more standardized Coordinate Reference Systems (CRSs). This provides an interoperable way to seamlessly express, record, and share the GeoPose of objects in an entirely consistent manner across different applications, users, devices, services, and platforms which adopt the standard or are able to translate/exchange the GeoPose into another CRS.

## Who is Ethar?

[Ethar, Inc.](https://www.ethar.com/) is a spatial computing company focused on delivering tools for the entire life-cycle of XR content. We believe the future of spatial computing is open and cooperative, we strive to incorporate open and interoperable technology standards into the very foundation of our products.

Along the way we have built some useful tooling, that we want to share with the community, and encourage the adoption of open standards. This is our implementation of GeoPose. We think it is one of the most important building blocks of an open, interoperable & decentralized spatial web.

## Ethar GeoPose Authority Implementation Features

- Implements 5 core Frame Specifications.
- Provides JSON conversion routines to receive and transmit GeoPose data utilising the Frame Specifications within the GeoPose SDU's.
- Validation functions to qualify incoming Frame Specifications.
- An example Interpolated Transition Model and a fallback None Transition Model.
- Full Authority implementation ready for use, including helper functions.
- Full Unit Testing of GeoPose Authority concepts and elements. (available via the [GitHub site](https://github.com/EtharInc/Ethar.GeoPose/tree/main/Ethar.GeoPose.Authority.UnitTests), not included with npm package)

> Full documentation on the implementation specification and helper docs, including common guides for C# and Unity can be found at:
>
> [https://etharinc.github.io/Ethar.GeoPose.Docs](https://etharinc.github.io/Ethar.GeoPose.Docs)

## Installation

The Ethar GeoPose library and the corresponding Ethar Authority implementation have been provided in as many places as possible, including:

- NuGet - for cross platform C# development.
- OpenUPM - for versioned Unity deployment.
- Git - Fully open sourced on GitHub.

> For comments, questions and queries, please log a request on the [Ethar GeoPose GitHub site here](https://github.com/EtharInc/Ethar.GeoPose/issues).

### Install via NuGet

```xml
<ItemGroup>
  <PackageReference Include="Ethar.GeoPose.Authority" Version="1.0.0" />
</ItemGroup>
```

### Install via OpenUPM

```text
    openupm add com.ethar.GeoPose
```

### Git Source

Being open source, all the code, examples and features are available via the GitHub page for the Ethar GeoPose Library here:

``` text
https://github.com/EtharInc/Ethar.GeoPose
```

## Usage

Beyond what is outlined in the GeoPose standard, use of the Ethar GeoPose authority is documented on the [Ethar GeoPose documentation site here](https://etharinc.github.io/Ethar.GeoPose.Docs).

## Concepts

In summary, the following concepts are crucial to understanding the [GeoPose specification](https://docs.ogc.org/dis/21-056r10/21-056r10.html) defined by the OGC, namely:

- [A Pose, or fixed position for an object.](#pose)
- [The Orientation or direction of a posed object.](#orientation)
- [GeoPose Structural Data Units (SDU) to describe locational metadata.](#structural-data-units)
- [A Frame Specification for GeoPose Data.](#frame-specifications)
- [A GeoPose Authority](#geopose-authorities).

### Pose

At the core of the GeoPose definition is the concept of a Pose, that being a position or place where an object is located.  A Pose may be determined by a coordinate system relative to a physical location, which may be a GPS, cartesian, image tracked or SLAM position.

The position is absolute at the time of creation and updated as the need arises.

### Orientation

To define the actual direction in which a GeoPosed object is placed, a direction is needed to denote the physical orientation of a placed object.  The orientation of objects relative to their position is one of the key factors that sets the GeoPose standard apart from other positioning solutions.

### Structural Data Units

The base definition of a GeoPose construct is its ```Structural Data Unit``` definition, which outlines the serialized data that is shared between entities to communicate GeoPosed data.  By default, there are 8 Structural Data Units defined within the [GeoPose standard](https://docs.ogc.org/dis/21-056r10/21-056r10.html#toc45), which are:

- Basic YawPitchRoll - Basic positioning using WGS84 coordinates for position and Euler angles for orientation.
- Basic Quaternion - Basic positioning using WGS84 coordinates for position and a Quaternion for orientation.
- Advanced - An advanced concept utilizing a [Frame Specification](#frame-specifications) that defines a reference frame for an object.
- Graph - An SDU that contains a directed acyclic graph representation of the transformational relationships between reference frames defined by [Frame Specifications](#frame-specifications).
- Chain - An SDU that represents a linear sequence of poses linked by full 6DoF transformations, with the first frame in the sequence being extrinsic.
- Regular Series - An ordered set of operations to perform on a GeoPosed object, complete with timed events.
- Irregular Series - An unordered set of operations for use on a GeoPosed Object.
- Stream - Another advanced use case whereby complex operations can be structured, such as animation.

Structural Data Units consist of base GeoPose Data Types and can contain one or more [Frame Specifications](#frame-specifications) for extending a GeoPosed Object.  Which type of SDU you use will largely depend on your use case, and there is always the option of creating your own (at the cost of interoperability).

> See the [GeoPose standard](https://docs.ogc.org/dis/21-056r10/21-056r10.html#toc45) section on Structural Data Units for more information.

### Frame Specifications

A ```Frame Specification``` can take on multiple roles or responsibilities for objects placed using a GeoPose, these can range from Transform Animations, waypoints, relativistic placement and structure.  Which frame specifications are used will be largely determined by the use case required and the [SDU](#structural-data-units) that has been implemented.

At its most simplistic level, Frame Specifications are references used to co-locate a GeoPose object in relation to another physical entity (such as the Earth) or another GeoPose.  In advanced cases they can be used to infer animation, or the bounds of a GeoPosed object.

Unlike SDU's however, Frame Specifications require an authority who is responsible for orchestrating the content of the specification and ultimately, controls how the data is assembled and disassembled for transport. (Different organizations may implement different authorities for managing how they interpret and expose GeoPosed data.)

> See the [GeoPose standard](https://docs.ogc.org/dis/21-056r10/21-056r10.html#term-frame-specification) section on Frame Specifications for more information.

### GeoPose Authorities

An ```Authority``` in the GeoPose standard is the entity responsible for the understanding, conversion and transformation of any Frame Specification. It is defined in the Ethar GeoPose library through an Interface designed to enforce a specific pattern for Authority Implementations, as shown below:

![IAuthority Interface](https://raw.githubusercontent.com/EtharInc/Ethar.GeoPose/main/Images/architecture/IAuthority-Interface.png)<br/>
*figure 1: IAuthority Interface.*

The interface defines a single property and several methods required by an Authority for operation, namely:

- Authority Name - The unique name/identifier for the authority in the form of ```"/GeoPose/1.0"```
- ConvertJsonToFrameSpec - Method to take in a GeoPose Frame Specification JSON string and output a Frame Specification definition.
- ConvertFrameSpecToJson - Method to take a Frame Specification object and turn it into serialized GeoPose Frame Specification JSON string.
- ConvertJsonToTransitionModel - Method to take a [Transition Model](https://docs.ogc.org/dis/21-056r10/21-056r10.html#toc17) JSON string and output a Transition Model definition.
- ConvertTransitionModelToJson - Method to serialize a [Transition model](https://docs.ogc.org/dis/21-056r10/21-056r10.html#toc17) into a specific GeoPose Transition Model JSON string

> Additionally, it is recommended to also implement a ```FrameSpecificationValidator``` as part of any Authority implementation, to validate any Frame Specifications and handle any irregularities with incoming data.

You can see a fully implemented authority implementation [here](https://github.com/EtharInc/Ethar.GeoPose/blob/main/Ethar.GeoPose.Authority/EtharGeoPoseAuthority.cs) or refer to the ```Ethar GeoPose Authority Sample``` included with this package.

It is Critical, when implementing your own authority, to define the frame specification that the authority is responsible for, including the data types (either c# native or GeoPose elements) and then write ```Convertors``` to extract and understand the Frame specifications being handled by the authority, with specific attention to use the ```ValidationUtilities``` provided by the Ethar GeoPose library, for example:

```csharp
public static ExampleExtrinsicFrameSpec ConvertJObjectToExampleExtrinsicFrameSpec(JObject jObject)
{
    // Validated the incoming json object string and checks that it has the required values and also
    // checks if this is the authority mentioned in the incoming data that handles the frame specification.
    if (ValidationUtilities.ValidateJsonObjectParameters(jObject, Constants.AuthorityName, out var queryString))
    {
        // Retrieves the required data from the json to construct the Frame Specification.
        var lat = float.Parse(queryString.GetParameter("latitude"));
        var lon = float.Parse(queryString.GetParameter("longitude"));

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

![Authority Provider](https://raw.githubusercontent.com/EtharInc/Ethar.GeoPose/main/Images/architecture/AuthorityProvider-utility.png)<br/>
*figure 3: Authority Provider.*

The utility defines a single exposed property and several methods to safely access the dictionary, namely:

- Authorities - Read only list of registered authorities.
- RegisterAuthority - Used to register an Authority instance as active.
- UnregisterAuthority - Used to remove an Authority from active use and dispose of it.
- GetAuthority - Safe method for retrieving an Authority by its Name, returns null if not found.

Use of the AuthorityProvider to manage access to Authorities is recommended when handling incoming GeoPose data to ensure quick and safe access.

> For more detailed examples of authority registration and use, check the [Ethar GeoPose documentation](https://etharinc.github.io/Ethar.GeoPose.Docs).

## Contributing

Ethar.GeoPose is made possible by the excellent work of the Ethar Team:

<table><tbody>

<tr><th align="left">The Masked Coder</th><td>???</td><td>???</td></tr>

<tr><th align="left">Simon Jackson</th><td><a href="https://github.com/SimonDarksideJ">GitHub/SimonDarksideJ</a></td><td><a href="https://www.linkedin.com/in/xrconsultant/">LinkedIn/xrconsultant</a></td></tr>

<tr><th align="left">Connor Davis</th><td><a href="https://github.com/davisjc22/john-connor-davis">GitHub/john-connor-davis</a></td><td><a href="https://www.linkedin.com/in/john-connor-davis">LinkedIn/John-Connor-Davis</a></td></tr>

<tr><th align="left">Colin Steinmann</th><td><a href="https://github.com/metaColin">GitHub/metaColin</a></td><td><a href="https://www.linkedin.com/in/colinsteinmann/">LinkedIn/colinsteinmann</a></td></tr>

</tbody></table>

## Terms of Use

For full terms of use please refer our [documentation site](https://etharinc.github.io/Ethar.GeoPose.Docs/termsofuse.html).

## License

Ethar.GeoPose is distributed under [Apache 2.0 License](https://etharinc.github.io/Ethar.GeoPose.Docs/license.html)
