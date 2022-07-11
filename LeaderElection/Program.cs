using System;
using System.Threading.Tasks;

namespace LeaderElection
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var electionService = new LeaderElectionService();
            await electionService.Start();
            Console.ReadLine();
        }
    }
}
