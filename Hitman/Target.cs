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
        private readonly ILogger _logger;

        public int Id { get; }
        public string Codename { get; }
        public string Name { get; }
        public int SuspicionLevel { get; set; }
        public bool IsAlive { get; private set; }
        public string Location { get; private set; }

        public Target(int id, string name, string? codename = null, string initialLocation = "Unknown", int suspicionLevel = 0, ILogger? logger = null)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Codename = codename ?? $"TGT-{id}";
            Location = initialLocation;
            SuspicionLevel = suspicionLevel;
            IsAlive = true;
            _logger = logger ?? Logger.Instance;
        }

        
        public void Move()
        {
            if (!IsAlive)
            {
                _logger.Log($"[Target] {Name} ({Codename}) cannot move — already eliminated.");
                return;
            }

            // Проста логіка переміщення — випадкова локація
            string newLoc = $"Location-{_rnd.Next(1, 20)}";
            Location = newLoc;

            // Невелика вірогідна зміна підозри при пересуванні
            SuspicionLevel = Math.Min(100, SuspicionLevel + _rnd.Next(0, 3));

            _logger.Log($"[Target] {Name} ({Codename}) moved to {Location}. Suspicion: {SuspicionLevel}");
        }

        /// <summary>
        /// Update() викликається з World.Simulate().
        /// Тут можна додати складнішу поведінку (патрулювання, реакції),
        /// зараз: з імовірністю робить Move(), інакше трохи заспокоюється.
        /// </summary>
        public void Update()
        {
            if (!IsAlive) return;

            double r = _rnd.NextDouble();
            if (r < 0.65)
            {
                Move();
            }
            else
            {
                // "Заспокоєння" — зменшуємо підозру, але не нижче нуля
                int dec = _rnd.Next(0, 2);
                SuspicionLevel = Math.Max(0, SuspicionLevel - dec);
                _logger.Log($"[Target] {Name} ({Codename}) idles. Suspicion: {SuspicionLevel}");
            }
        }

        /// <summary>
        /// Ліквідація цілі.
        /// </summary>
        public void Eliminate()
        {
            if (!IsAlive)
            {
                _logger.Log($"[Target] {Name} ({Codename}) elimination attempted but already dead.");
                return;
            }

            IsAlive = false;
            _logger.Log($"[Target] {Name} ({Codename}) has been eliminated!");
        }

        public override string ToString()
        {
            return $"{Name} (Id:{Id}, Codename:{Codename}, Alive:{IsAlive}, Suspicion:{SuspicionLevel}, Location:{Location})";
        }
    }





}
