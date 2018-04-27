using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace JaminBooks
{
    /// <summary>
    /// Starts the web server
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point of the entire program.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Builds the web host.
        /// </summary>
        /// <param name="args">arguments</param>
        /// <returns>The web host</returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}