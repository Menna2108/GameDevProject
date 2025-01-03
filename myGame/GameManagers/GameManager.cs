using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameManagers
{
    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        public bool IsGameStarted { get; set; } = false;
        public bool IsPaused { get; set; } = false;
        public bool IsExiting { get; set; } = false;
        public bool IsGameOver { get; set; } = false;
        public int CurrentLevel { get; set; } = 1;
        public bool IsGameWon { get; set; }

        private GameManager()
        {
            IsGameStarted = false;
            IsPaused = false;
            IsExiting = false;
            IsGameOver = false;
            IsGameWon = false;
        } // Singleton: private constructor
    }
}
