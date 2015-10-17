namespace Owin.Scim.Mappings
{
    using System.ComponentModel.Composition;

    using AutoMapper;
    
    using Model.Users;

    using NContext.Extensions.AutoMapper.Configuration;

    using Security;

    public class UserMapping : IConfigureAutoMapper
    {
        private readonly IManagePasswords _PasswordManager;

        [ImportingConstructor]
        public UserMapping([Import]IManagePasswords passwordManager)
        {
            _PasswordManager = passwordManager;
        }

        public void Configure(IConfiguration mapper)
        {
            // All below attributes are readWrite
            mapper.CreateMap<Address, Address>();
            mapper.CreateMap<Name, Name>();
            mapper.CreateMap<Email, Email>();
            mapper.CreateMap<Entitlement, Entitlement>();
            mapper.CreateMap<InstantMessagingAddress, InstantMessagingAddress>();
            mapper.CreateMap<Role, Role>();
            mapper.CreateMap<Photo, Photo>();
            mapper.CreateMap<Name, Name>();
            mapper.CreateMap<Manager, Manager>();
            mapper.CreateMap<X509Certificate, X509Certificate>();
            mapper.CreateMap<PhoneNumber, PhoneNumber>();

            // UserGroup is readOnly!
            mapper.CreateMap<UserGroup, UserGroup>()
                .ForAllMembers(c => c.UseDestinationValue());

            mapper.CreateMap<User, User>()
                .ForMember(dst => dst.Id, c => c.UseDestinationValue())
                .ForMember(dst => dst.Groups, c => c.UseDestinationValue()) // User.Groups is readOnly!
                .ForMember(dst => dst.Password, c => c.Ignore()) // User.Password is handled in the service for validation purposes.
                .ForMember(dst => dst.Meta, c => c.UseDestinationValue()) // User.Meta is readOnly!
                .ForMember(dst => dst.Schemas, c => c.UseDestinationValue()); // User.Schemas is readOnly!
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}