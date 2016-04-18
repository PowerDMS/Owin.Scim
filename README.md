# Owin.Scim
OWIN SCIM 2.0 implementation  
[![Build status](https://ci.appveyor.com/api/projects/status/qgblu9mx4f53tvee/branch/master?svg=true)](https://ci.appveyor.com/project/powerdms/owin-scim/branch/master) [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Owin.Scim.svg?maxAge=1800)](https://www.nuget.org/packages/Owin.Scim/)

RFC 7643  
RFC 7644

Looking for solid contributers to expedite this effort!  
email me:  daniel.gioulakis [at] powerdms [dot] com

PROJECT STATUS
==============
This project is in active development with the goal of completing basic protocol implementation by mid-2016.

Roadmap
-------
The list below doesn't necessarily denote priority or order.

1. [ ] Finish users endpoints
  1. [x] Create  
  2. [x] Retrieve  
  3. [x] Replace  
  4. [x] Update (Patch)  
    1. [x] Support for path filters  
    2. [x] Add  
    3. [x] Replace  
    4. [x] Remove  
  5. [x] Delete  
  6. [ ] Query
2. [x] Schema extensions
3. [ ] Add SCIM server configuration endpoints
  1. [x] /ServiceProviderConfig
  2. [ ] /Schemas (IN PROGRESS)
  3. [x] /ResourceTypes
4. [x] Add support for mutability rule-processing.
5. [ ] Add support for bulk processing
6. [x] Add groups endpoints
  1. [x] Create
  2. [x] Retrieve
  3. [x] Replace
  4. [x] Update (Patch)
  5. [x] Delete  
  6. [ ] Query
7. [ ] Add support for querying
  1. [ ] Filtering (parsing into an expression tree is already done)
  2. [ ] Sorting
  3. [ ] Ordering
  4. [ ] Pagination
  5. [ ] Projection (in progress - currently only works with top-level attributes / non-urn qualified references)
8. [x] Add more extensiblity options
  1. [x] Canonicalization  
  2. [x] Validation  
  3. [x] Attribute Behavior (mutability, caseExact, returned, uniqueness, etc)
9. [ ] Add endpoint authorization support

Getting Started
===============
Please see the WIKI for all documentation. It is being continually updated as the project develops into a stable SCIM implementation.
