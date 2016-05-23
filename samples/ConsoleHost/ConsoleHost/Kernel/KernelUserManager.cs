namespace ConsoleHost.Kernel
{
    using System.Threading.Tasks;

    public class KernelUserManager
    {
        private readonly KernelUserRepository _UserRepository;

        public KernelUserManager(KernelUserRepository userRepository)
        {
            _UserRepository = userRepository;
        }
        
        public Task<KernelUser> CreateUser(KernelUser user)
        {
            // Typically more business logic here

            return _UserRepository.CreateUser(user);
        }

        public Task<KernelUser> GetUser(string userId)
        {
            // Typically more business logic. e.g. does the authenticated user have access to the userId resource?

            return _UserRepository.GetUser(userId);
        }

        public Task UpdateUser(KernelUser user)
        {
            return _UserRepository.UpdateUser(user);
        }

        public Task<KernelUser> DeleteUser(string userId)
        {
            return _UserRepository.DeleteUser(userId);
        }
    }
}