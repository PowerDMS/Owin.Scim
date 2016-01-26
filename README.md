# Owin.Scim
OWIN SCIM 2.0 implementation

RFC 7643

RFC 7644

Looking for solid contributers to expedite this effort!  
email me:  daniel.gioulakis [at] powerdms [dot] com

PROJECT STATUS
==============
This project is in active development until further information of Microsoft's  Microsoft.SystemForCrossDomainIdentityManagement has been announced.  As of now, it looks like that project is focusing specifically on outbound SCIM provisioning from Azure AD only.  As of now it's not SCIM compliant.

For more information, see:  
https://github.com/Azure/AzureAD-BYOA-Provisioning-Samples  
https://www.nuget.org/packages/Microsoft.SystemForCrossDomainIdentityManagement/  

Roadmap
-------

1. Finish users endpoints
  1. [x] Create  
  2. [x] Retrieve  
  3. [x] Replace  
  4. [ ] Update (Patch) (IN PROGRESS)
    1. [x] Support for path filters
    2. [x] Add
    3. [x] Replace
    4. [x] Remove
    5. [ ] Add more test coverage to prove compliance
    6. [ ] Error handling / rule processing
  5. [x] Delete  
  6. [ ] Add support for custom canonical types / validation
2. Add groups endpoints
3. Add SCIM server configuration endpoints
  1. [ ] /ServiceProviderConfig
  2. [ ] /Schemas
  3. [ ] /ResourceTypes
4. Add support for bulk processing
5. Add support for querying
  1. [ ] Filtering (parsing into an expression tree is already done)
  2. [ ] Sorting
  3. [ ] Ordering
  4. [ ] Pagination
  5. [ ] Projection
6. Add more extensiblity options
7. Add authN / authZ
