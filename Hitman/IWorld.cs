using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public interface IWorld
    {
        // Колекції для читання
        IReadOnlyCollection<IAgent> Agents { get; }
        IReadOnlyCollection<ITarget> Targets { get; }

        // Подія, яка сигналізує про зміну/рух цілі
        event Action<ITarget>? OnTargetMoved;

        // Додавання сутностей
        void AddAgent(IAgent agent);
        void AddTarget(ITarget target);

        // Головний метод для оновлення світу
        void Simulate();
    }


}
