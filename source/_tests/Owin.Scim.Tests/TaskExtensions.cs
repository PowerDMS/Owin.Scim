namespace Owin.Scim.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Machine.Specifications;

    public static class TaskExtensions
    {
        public static AwaitResult<T> AwaitResponse<T>(this Task<T> responseTask)
        {
            try
            {
                responseTask.Wait(TimeSpan.FromHours(1)); // Debugging
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1)
                {
                    throw ex.InnerExceptions.First();
                }

                throw;
            }

            return new AwaitResult<T>(responseTask);
        }
    }
}