using System;

namespace LeaderElection
{
    class Program
    {
        static void Main(string[] args)
        {
            var electionService = new LeaderElectionService();
            electionService.Start();
            Console.ReadLine();
        }
    }
}
