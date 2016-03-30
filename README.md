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
9. [ ] Add authN / authZ

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
All core SCIM resource types and rules are added by default.  There are many valid use cases to modify the default Owin.Scim resource type settings:  
* service provider may require certain fields  
* service provider has specific canonicalization rules  
* service provider has specific validation rules  
* etc ...  

The majority of SCIM's resource type attributes have qualities which default to:  
* caseExact: false  
* mutability: readWrite (readOnly, writeOnly, immutable)  
* required: false  
* returned: default (always, never)  
* uniqueness: none (server, global)  

Owin.Scim auto-assigns all core attribute qualities as per https://tools.ietf.org/html/rfc7643#section-7.  Depending on your requirements as a SCIM service provider, you may need to modify these qualities as you see fit.  It is up to you to ensure any modifications remain in compliance of the SCIM standard.

####Adding resource types:
```csharp
app.UserScimServer(
  new ScimServerConfiguration()
    .RemoveResourceType<User>
    .AddResourceType<User, UserValidator>(
      schemaIdentifiers => schemaIdentifiers.Contains(ScimConstants.Schemas.User),
      userResourceBuilder => { ... });
```

######Schema Binding Rules
In the above example, if the predicate lambda function is satisfied (returns true) by the schemas collection passed into it as an argument, Owin.Scim will instantiate an instance of the associated type argument `User`.

######Resource Type Definitions / Type Definitions
In order to process rules concerning mutability and attribute serialization, Owin.Scim requires all resource types and complex types to define their metadata schema representations.  Without this, Owin.Scim will interpret each attribute with the default SCIM attribute behaviors: Mutability.ReadWrite, Returned.Default, etc.

To define your metadata, each object must specify a `ScimTypeDefinitionAttribute`. (e.g. [ScimTypeDefinition(typeof(UserDefinition))])

A type definition must implement IScimTypeDefinition and furthermore, inherit from either `ScimResourceTypeDefinitionBuilder<T>` or `ScimTypeDefinitionBuilder<T>`.  Inherit from the former when you are defining resource types and the latter when defining any other object. See the `UserDefinition` class for additional clarification.

####Modifying core resource types
```csharp
app.UseScimServer(
  new ScimServerConfiguration()
    .ModifyResourceType<User>(ModifyUserResourceType));

// ...

private void ModifyUserResourceType(ScimResourceTypeDefinitionBuilder<User> builder)
{
  builder
    .SetValidator<UserValidator>() // allows you to change the validator for the resource
    .AddSchemaExtension<EnterpriseUserExtension, EnterpriseUserExtensionValidator>(ScimConstants.Schemas.UserEnterprise, false); // add optional or required schema extensions
}
```

#####Adding Canonicalization Rules
Owin.Scim allows the developer to specify canonicalization rules (delegates) as rules for resource type attributes. Some rules are built-in by default.

```csharp
builder
  .For(u => u.Emails)
      .AddCanonicalizationRule(email => 
          email.Canonicalize(
              e => e.Value, 
              e => e.Display, 
              value =>
          {
              // canonicalize email.Value (user@MyDomain.cOm) into email.Display (user@mydomain.com)
              var atIndex = value.IndexOf('@') + 1;
              if (atIndex == 0) return null; // IndexOf returned -1, invalid email
                              
              return value.Substring(0, atIndex) + value.Substring(atIndex).ToLower();
          }))
      .AddCanonicalizationRule((Email attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state))
```
This example illustrates setting canonicalization rules on multi-valued attributes for email.  Here, we are taking the email.Value attribute and canonicalizing it to the email.Display attribute.  Since this rule requires access to multiple email attributes, we add this rule to the Emails attribute of User.  Next, we will look at canonicalization rules for scalar attributes.

*By default, all core resource types with multi-valued attributes have a canonicalization rule to* `EnforceSinglePrimaryAttribute`*.  This complies with the specification's rules.*

#####Scalar Attributes
It may be you're only canonicalizing a single scalar-valued attribute or do not need to reference it's parent object's other attributes. (e.g. value and display).  In that case, just apply the canonicalization rule:

```csharp
  For(photo => photo.Value)
      .AddCanonicalizationRule(value => value.ToLower()))
```

*You do not need to check for null (reference-type) or default (value-type) values being passed in.*  Owin.Scim saves time by doing this for you.

######Canonical Values
Owin.Scim does not include or define any acceptable attribute values (SCIM "canonical values").  
The SCIM specification frequently references "canonical values" for multi-valued attribute's `type` attribute.  Values like: `work`, `home`, `other`, etc.  Owin.Scim does not view this as canonicalization but rather validation.  In this light, canonicalization should be viewed strictly in terms of normalization of data.
