using myGame.GameInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameManagers
{
    internal class CoinCollector : ICoinCollector
    {
        public int TotalCoins { get; private set; } = 0;

        public void OnCoinCollected()
        {
            TotalCoins++;
        }
    }
}
