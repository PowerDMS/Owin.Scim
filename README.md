# Owin.Scim
OWIN SCIM 1.1 & 2.0 implementation for .NET >= 4.5.1  
[![Build status](https://ci.appveyor.com/api/projects/status/qgblu9mx4f53tvee/branch/master?svg=true)](https://ci.appveyor.com/project/powerdms/owin-scim/branch/master) [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Owin.Scim.svg?maxAge=1800)](https://www.nuget.org/packages/Owin.Scim/)

RFC 7643  
RFC 7644

Looking for solid contributers to expedite this effort!  
email me:  daniel.gioulakis [at] powerdms [dot] com

If you have been evaluating the alpha builds from nuget, we would love to hear feedback from you. Please reach out and share how your experience has been working with Owin.Scim. Any issues and feature requests are welcome. Help us shape Owin.Scim into the best fully-featured and compliant open-source SCIM server!

Endpoints Implemented.

##### SCIM v2  
- /scim/v2/ServiceProviderConfig
- /scim/v2/Schemas
- /scim/v2/ResourceTypes
- /scim/v2/Users
- /scim/v2/Groups

##### SCIM v1   
- /scim/v1/ServiceProviderConfigs
- /scim/v1/Schemas
- /scim/v1/Users
- /scim/v1/Groups

PROJECT STATUS
==============
This project is in active development with the goal of completing basic protocol implementation by mid-2016.

Roadmap
-------
The list below doesn't necessarily denote priority or order.

- [ ] Finish users endpoints
  - [x] Create  
  - [x] Retrieve  
  - [x] Replace  
  - [x] Update (Patch) (in progress - cleanup code)
    - [x] Add  
    - [x] Replace  
    - [x] Remove  
  - [x] Delete  
  - [x] Query
- [x] Schema extensions
- [x] Add SCIM server configuration endpoints
  - [x] /ServiceProviderConfig
  - [x] /Schemas
  - [x] /ResourceTypes
- [x] Add support for mutability rule-processing.
- [ ] Add support for bulk processing
- [ ] Add groups endpoints
  - [x] Create
  - [x] Retrieve
  - [x] Replace
  - [ ] Update (Patch)  (in progress - cleanup code)
    - [x] Add
    - [x] Replace
    - [x] Remove
  - [x] Delete  
  - [x] Query
- [ ] Add support for querying
  - [x] Filtering
  - [ ] Sorting (in design)
  - [ ] Ordering (in design)
  - [ ] Pagination (in design)
  - [ ] Projection (in progress - currently only works with top-level attributes / non-urn qualified references)
- [x] SCIM Extensiblity
  - [x] Canonicalization  
  - [x] Validation  
  - [x] Attribute Behavior (mutability, caseExact, returned, uniqueness, etc)
  - [x] Custom resource types and endpoints
- [x] Add endpoint authorization support
- [ ] Add endpoint for supporting query-on-root
- [x] Add support for SCIM v1.1
- [ ] Add logging functionality
- [ ] Outbound Provisioning (SCIM Event Notification)
  - [ ] Event Triggers / Handlers
  - [ ] Client Subscription Management

Getting Started
===============
Please see the WIKI for all documentation. It is being continually updated as the project develops into a stable SCIM implementation.
