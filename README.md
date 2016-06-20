# Owin.Scim
OWIN SCIM 1.1 & 2.0 implementation for .NET >= 4.5.1  
[![Build status](https://ci.appveyor.com/api/projects/status/qgblu9mx4f53tvee/branch/master?svg=true)](https://ci.appveyor.com/project/powerdms/owin-scim/branch/master) [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Owin.Scim.svg?maxAge=1800)](https://www.nuget.org/packages/Owin.Scim/)

RFC 7643  
RFC 7644

Looking for solid contributers to expedite this effort!  
email me:  daniel.gioulakis [at] powerdms [dot] com

If you have been evaluating the alpha builds from nuget, we would love to hear feedback from you. Please reach out and share how your experience has been working with Owin.Scim. Any issues and feature requests are welcome. Help us shape Owin.Scim into the best fully-featured and compliant open-source SCIM server!

Latest News!
============
We're excited to announce that Owin.Scim is now actively deployed to Azure AppServices as part of our way to showcase the capabilities of Owin.Scim. You can access all live SCIM endpoints at: http://owin-scim.azurewebsites.net/scim/ (e.g. `http://owin-scim.azurewebsites.net/scim/<version>/<endpoint>`)  

####Live Endpoints for Interoperability Testing  
#####SCIM v2  
- http://owin-scim.azurewebsites.net/scim/v2/serviceproviderconfig
- http://owin-scim.azurewebsites.net/scim/v2/schemas
- http://owin-scim.azurewebsites.net/scim/v2/resourcetypes
- http://owin-scim.azurewebsites.net/scim/v2/users
- http://owin-scim.azurewebsites.net/scim/v2/groups

#####SCIM v1   
- http://owin-scim.azurewebsites.net/scim/v1/serviceproviderconfigs
- http://owin-scim.azurewebsites.net/scim/v1/schemas
- http://owin-scim.azurewebsites.net/scim/v1/users
- http://owin-scim.azurewebsites.net/scim/v1/groups

PROJECT STATUS
==============
This project is in active development with the goal of completing basic protocol implementation by mid-2016.

Roadmap
-------
The list below doesn't necessarily denote priority or order.

01. [ ] Finish users endpoints
  1. [x] Create  
  2. [x] Retrieve  
  3. [x] Replace  
  4. [ ] Update (Patch) (in progress - cleanup code)
    1. [x] Add  
    2. [x] Replace  
    3. [x] Remove  
  5. [x] Delete  
  6. [x] Query
02. [x] Schema extensions
03. [x] Add SCIM server configuration endpoints
  1. [x] /ServiceProviderConfig
  2. [x] /Schemas
  3. [x] /ResourceTypes
04. [x] Add support for mutability rule-processing.
05. [ ] Add support for bulk processing
06. [ ] Add groups endpoints
  1. [x] Create
  2. [x] Retrieve
  3. [x] Replace
  4. [ ] Update (Patch)  (in progress - cleanup code)
    1. [x] Add
    2. [x] Replace
    3. [x] Remove
  5. [x] Delete  
  6. [x] Query
07. [ ] Add support for querying
  1. [x] Filtering
  2. [ ] Sorting (in design)
  3. [ ] Ordering (in design)
  4. [ ] Pagination (in design)
  5. [ ] Projection (in progress - currently only works with top-level attributes / non-urn qualified references)
08. [x] SCIM Extensiblity
  1. [x] Canonicalization  
  2. [x] Validation  
  3. [x] Attribute Behavior (mutability, caseExact, returned, uniqueness, etc)
  4. [x] Custom resource types and endpoints
09. [x] Add endpoint authorization support
10. [ ] Add endpoint for supporting query-on-root
11. [ ] Add support for SCIM v1.1
12. [ ] Add logging functionality
13. [ ] Outbound Provisioning (SCIM Event Notification)
  1. [ ] Event Triggers / Handlers
  2. [ ] Client Subscription Management

Getting Started
===============
Please see the WIKI for all documentation. It is being continually updated as the project develops into a stable SCIM implementation.
