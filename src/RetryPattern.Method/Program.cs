global using Polly;

namespace RetryPattern.Method
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            int retryAttempts = 5;

            var retry = Policy
            .Handle<Exception>()
            .Or<ArgumentException>()
            .WaitAndRetryAsync(
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
                await retry.ExecuteAsync(async () =>
                {
                    ClientService clientService = new();
                    await clientService.Login();

                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine("You are Authenticated ! ! !");
                    Console.WriteLine(Environment.NewLine);

                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }
    }

    public class ClientService
    {
        public async Task<bool> Login()
        {
            return await Task.Factory.StartNew(() =>
            {
                Random random = new();
                int rNumber = random.Next(int.MinValue, int.MaxValue);

                return rNumber % 2 == 0
                    ? true
                    : throw new Exception($"Login error. Random number was {rNumber}! ! !");
            });
        }
    }
}