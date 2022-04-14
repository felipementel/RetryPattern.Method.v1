global using Polly;

namespace RetryPattern.Method
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int retryAttempts = 5;
            var retry = Policy
            .Handle<Exception>()
            .WaitAndRetry(
                retryCount: retryAttempts,
                sleepDurationProvider: retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)),
                onRetry: (exception, sleepDuration, attemptNumber, context) =>
            {
                Console.WriteLine($"Retrying in {sleepDuration}. {attemptNumber}" +
                    $" / {retryAttempts}. Error message: {exception.Message}");
            });

            try
            {
                retry.Execute(() =>
                {
                    ClientService clientService = new();
                    clientService.Login();

                    Console.WriteLine("You are Authenticated ! ! !");

                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                });
            }
            catch (Exception)
            {

                throw;
            }            
        }
    }

    public class ClientService
    {
        public bool Login()
        {
            Random random = new();
            int rNumber = random.Next(int.MinValue,int.MaxValue);

            return rNumber % 2 == 0 
                ? true 
                : throw new Exception($"Login error. Random number was {rNumber}! ! !");
        }
    }
}