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
2. Schema extensions
  1. [x] Add custom parameter binding for schema extension deserialization
3. Add SCIM server configuration endpoints
  1. [x] /ServiceProviderConfig
  2. [ ] /Schemas (IN PROGRESS)
  3. [ ] /ResourceTypes (IN PROGRESS)
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
  1. [ ] Canonicalization (IN PROGRESS)
  2. [ ] Validation
  3. [ ] Attribute Behavior (mutability, caseExact, returned, uniqueness, etc) (IN PROGRESS)
8. Add authN / authZ

Getting Started
===============
Like most OWIN extensions, just configure your appBuilder.  
```csharp
app.UseScimServer(
  new ScimServerConfiguration
  {
      RequireSsl = false
  }
  .AddAuthenticationScheme(
      new AuthenticationScheme(
          "oauthbearertoken",
          "OAuth Bearer Token",
          "Authentication scheme using the OAuth Bearer Token standard.", 
          specUri: new Uri("https://tools.ietf.org/html/rfc6750"),
          isPrimary: true))
  .ConfigureETag(supported: true, isWeak: true));
```

##SCIM Extensibility  
###Defining & Modifying Resource Types  
All core SCIM resource types and rules are added by default.

SCIM's resource type attributes default to:  
* caseExact: false  
* mutability: readWrite (readOnly, writeOnly, immutable)  
* required: false  
* returned: default (always, never)  
* uniqueness: none (server, global)  

Owin.Scim auto-assigns all core attribute qualities as per https://tools.ietf.org/html/rfc7643#section-7.  Depending on your requirements as a SCIM service provider, you may need to modify these qualities as you see fit.  It is up to you to ensure any modificaitons remain in compliance of the SCIM standard.

To modify core resource types:
```csharp
app.UseScimServer(
  new ScimServerConfiguration { RequireSsl = false }
    .ModifyResourceType<User>(ModifyUserResourceType));

// ...

private void ModifyUserResourceType(ScimResourceTypeDefinitionBuilder<User> builder)
{
  builder
    .For(u => u.UserName)
        .SetRequired(true)
        .SetUniqueness(Unique.Server)
    .For(u => u.Name)
        .DefineSubAttributes(nameCofig => nameCofig
            .For(name => name.FamilyName)
                .SetRequired(true))
    .For(u => u.Id)
        .SetMutability(Mutable.ReadOnly)
        .SetReturned(Return.Always)
        .SetUniqueness(Unique.Server)
        .SetCaseExact(true)
    .For(u => u.Password)
        .SetMutability(Mutable.WriteOnly)
        .SetReturned(Return.Never)
    .For(u => u.Groups)
        .SetMutability(Mutable.ReadOnly)
}
```
In the example provided, I have made a User.Name.FamilyName required.  This is against the default SCIM rule and will enforce clients to submit a FamilyName when creating/modifying users.
