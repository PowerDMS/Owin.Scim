This sample application showcases much of Owin.Scim's features including:
  1. Getting service provider configuration, e.g. /serviceproviderconfig
  2. Getting resource types, e.g. /resourcetypes
  3. Getting schemas, e.g. /schemas
  4. /users
    1. Creating a user
    2. Getting a user
    3. Defining a custom validator for an existing resource type (CustomUserValidator)
  5. /tenants (custom resource type)
    1. Create a tenant
    2. Getting a tenant
    3. Defining a resource type api controller
    4. Defining a resource type schema
    5. Defining a custom resource type validator
  6. Using an external dependency injection container for resolving external services within Owin.Scim services.