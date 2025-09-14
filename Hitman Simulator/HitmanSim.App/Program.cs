using Hitman_Simulator;
using Hitman_Simulator.HitmanSimulator.Core;

namespace HitmanSim.App
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Hitman Simulator — Campaign Demo ===");

            // 1) Ініціалізація ядерних сервісів
            IWorld world = new World();
            IThreadPoolManager threadPool = new ThreadPoolManager();
            ILogger logger = Logger.Instance;
            IActionManager actionManager = new ActionManager(threadPool, logger);

            // 2) Фасад гри
            IGameFacade game = new GameFacade(world, actionManager);
            game.Initialize(); // підвантажить MissionPack за замовчуванням

            // 3) Створюємо агента і додаємо у гру
            var agent = new Agent47();
            game.AddAgent(agent);

            // 4) (Опціонально) Ви можете додати додаткові цілі вручну або використовувати ті, що у MissionPack
            // Наприклад, якщо хочете додати ще ціль:
            // var custom = new Target(99, "Custom Target", 3);
            // game.AddTargetToWorld(custom);

            // 5) Запускаємо кампанію (кількість кроків на місію, пауза між кроками у мс)
            game.RunCampaign(stepsPerMission: 5, stepDelayMs: 800);

            Console.WriteLine("\nCampaign complete. Press any key to exit...");
            Console.ReadKey();
        }
    }

}
