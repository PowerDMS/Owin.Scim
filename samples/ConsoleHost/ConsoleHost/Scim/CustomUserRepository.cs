namespace ConsoleHost.Scim
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using AutoMapper;

    using Kernel;

    using Owin.Scim.ErrorHandling;
    using Owin.Scim.Model.Users;
    using Owin.Scim.Querying;
    using Owin.Scim.Repository;

    /// <summary>
    /// This example user repository illustrates how Owin.Scim can make use of external
    /// dependency injection containers so that your SCIM repositories can depend on their
    /// registrations without requiring double-registration with different containers.
    /// </summary>
    /// <seealso cref="Owin.Scim.Repository.IUserRepository" />
    public class CustomUserRepository : IUserRepository
    {
        private readonly IMapper _Mapper;

        private readonly KernelUserManager _UserManager;

        public CustomUserRepository(
            IMapper mapper,
            KernelUserManager userManager)
        {
            _Mapper = mapper;
            _UserManager = userManager;
        }

        public async Task<ScimUser> CreateUser(ScimUser user)
        {
            var kernelUser = _Mapper.Map<KernelUser>(user);
            var userRecord = await _UserManager.CreateUser(kernelUser);

            if (userRecord == null)
                throw new ScimException(HttpStatusCode.BadRequest, "Could not create user.");

            return _Mapper.Map<ScimUser>(userRecord);
        }

        public async Task<ScimUser> GetUser(string userId)
        {
            var userRecord = await _UserManager.GetUser(userId);
            if (userRecord == null)
                return null;

            return _Mapper.Map<ScimUser>(userRecord);
        }

        public async Task UpdateUser(ScimUser user)
        {
            var kernelUser = _Mapper.Map<KernelUser>(user);
            await _UserManager.UpdateUser(kernelUser);
        }

        public async Task<ScimUser> DeleteUser(string userId)
        {
            var userRecord = await _UserManager.DeleteUser(userId);
            return _Mapper.Map<ScimUser>(userRecord);
        }

        public Task<IEnumerable<ScimUser>> QueryUsers(ScimQueryOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserNameAvailable(string userName)
        {
            return Task.FromResult(true);
        }
    }
}