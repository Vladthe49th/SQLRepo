using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hitman_Simulator.HitmanSimulator.Core;

namespace Hitman_Simulator
{
    public class Target : ITarget
    {
        private static readonly Random _rnd = new();

        public int Id { get; private set; }
        public string Name { get; private set; }
        public int SuspicionLevel { get; set; }
        public bool IsAlive { get; private set; } = true;

        public string Codename => throw new NotImplementedException();

        public Target(int id, string name, int suspicionLevel = 0)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SuspicionLevel = suspicionLevel;
        }

  
        public Target(string name) : this(0, name, 0) { }

        public void Move()
        {
            if (!IsAlive) return;
            // Проста логіка руху — трохи збільшуємо підозру
            SuspicionLevel = Math.Min(100, SuspicionLevel + _rnd.Next(0, 3));
        }

        public void Eliminate()
        {
            IsAlive = false;
        }

        public override string ToString() =>
            $"{Name} (Id:{Id}) Alive:{IsAlive} Suspicion:{SuspicionLevel}";
    }





}
