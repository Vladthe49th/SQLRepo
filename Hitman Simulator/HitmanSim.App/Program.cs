using Hitman_Simulator;
using Hitman_Simulator.HitmanSimulator.Core;

namespace HitmanSim.App
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Hitman Simulator — Campaign Demo ===");


            IWorld world = new World();
            IThreadPoolManager threadPool = new ThreadPoolManager();
            ILogger logger = Logger.Instance;
            IActionManager actionManager = new ActionManager(threadPool, logger);


            IGameFacade game = new GameFacade(world, actionManager);
            game.Initialize(); 

          
            var agent = new Agent47();
            game.AddAgent(agent);

          

      
            game.RunCampaign(stepsPerMission: 5, stepDelayMs: 800);

            Console.WriteLine("\nCampaign complete. Press any key to exit...");
            Console.ReadKey();
        }
    }

}
