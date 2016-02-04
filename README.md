# Owin.Scim
OWIN SCIM 2.0 implementation

RFC 7643

RFC 7644

Looking for solid contributers to expedite this effort!  
email me:  daniel.gioulakis [at] powerdms [dot] com

PROJECT STATUS
==============
This project is in active development with the goal of completing basic protocol implementation by mid-2016.

Roadmap
-------

1. Finish users endpoints
  1. [x] Create  
  2. [x] Retrieve  
  3. [x] Replace  
  4. [ ] Update (Patch)
    1. [x] Support for path filters
    2. [x] Add
    3. [x] Replace
    4. [x] Remove
    5. [x] Add more test coverage to prove compliance
    6. [ ] Error handling / rule processing
  5. [x] Delete  
  6. [ ] Add support for custom canonical types / validation  
2. Schema extensions
  1. [x] Add custom parameter binding for schema extension deserialization
3. Add SCIM server configuration endpoints (IN PROGRESS)
  1. [x] /ServiceProviderConfig
  2. [ ] /Schemas
  3. [ ] /ResourceTypes
4. Add groups endpoints
  1. [ ] Create
  2. [ ] Retrieve
  3. [ ] Replace
  4. [ ] Update (Patch)
  5. [ ] Delete
5. Add support for bulk processing
6. Add support for querying
  1. [ ] Filtering (parsing into an expression tree is already done)
  2. [ ] Sorting
  3. [ ] Ordering
  4. [ ] Pagination
  5. [ ] Projection
7. Add more extensiblity options
8. Add authN / authZ
