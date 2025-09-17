using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public class World : IWorld
    {
        private readonly List<IAgent> _agents = new();
        private readonly List<ITarget> _targets = new();

        public IReadOnlyCollection<IAgent> Agents => _agents.AsReadOnly();
        public IReadOnlyCollection<ITarget> Targets => _targets.AsReadOnly();

        // Подія, яка викликається при русі або зміні стану цілі
        public event Action<ITarget>? OnTargetMoved;

        public void AddAgent(IAgent agent) => _agents.Add(agent);

        public void AddTarget(ITarget target) => _targets.Add(target);

        // Тут симуляція світу - можна оновлювати стани агентів і цілей
        public void Simulate()
        {
            foreach (var target in _targets)
            {
                // Наприклад, просте "рух" цілі:
                target.Update();

                // Викликаємо подію (можна підписати логування або реакцію агентів)
                OnTargetMoved?.Invoke(target);
            }

            foreach (var agent in _agents)
            {
                // Аналогічно оновлюємо агентів
                agent.Update();
            }
        }
    }



}
