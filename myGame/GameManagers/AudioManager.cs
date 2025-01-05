using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myGame.GameManagers
{
    public class AudioManager
    {
        private Song gameSound;
        private Song gameOver;
        private Song bossFightSong;
        private SoundEffect shootSound;
        private SoundEffect winSound;
        private SoundEffect coinSound;
        private SoundEffect shootedSound;
        private SoundEffect levelSound;

        private static AudioManager _instance;
        public static AudioManager Instance => _instance ??= new AudioManager();

        private AudioManager() { } // Singleton pattern

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            gameSound = content.Load<Song>("Audio/gameSound");
            gameOver = content.Load<Song>("Audio/over");
            bossFightSong = content.Load<Song>("Audio/bossfight");
            shootSound = content.Load<SoundEffect>("Audio/shoot");
            winSound = content.Load<SoundEffect>("Audio/win");
            coinSound = content.Load<SoundEffect>("Audio/coin");
            shootedSound = content.Load<SoundEffect>("Audio/shooted");
            levelSound = content.Load<SoundEffect>("Audio/leveledUp");
        }

        public void PlayGameSound(bool repeat = true)
        {
            MediaPlayer.Play(gameSound);
            MediaPlayer.IsRepeating = repeat;
        }

        public void PlayGameOver() => MediaPlayer.Play(gameOver);
        public void PlayBossFight()
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(bossFightSong);
            MediaPlayer.IsRepeating = true;
        }
        public void PlayShootSound() => shootSound.Play();
        public void PlayWinSound() => winSound.Play();
        public void PlayCoinSound() => coinSound.Play();
        public void PlayShootedSound() => shootedSound.Play();
        public void PlayLevelSound() => levelSound.Play();

        public void StopMusic() => MediaPlayer.Stop();
    }
}
