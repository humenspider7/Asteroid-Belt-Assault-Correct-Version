using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Asteroid_Belt_Assault
{
    public static class SoundManager
    {
        private static List<SoundEffect> explosions = new
            List<SoundEffect>();
        private static int explosionCount = 4;

        private static SoundEffect playerShot;
        private static SoundEffect enemyShot;
        private static SoundEffect playerDeath;
        private static Song Smokemon;
        private static Song goku;
        private static SoundEffect SS3;

        private static Random rand = new Random();

        public static void Initialize(ContentManager content)
        {
            try
            {
                playerShot = content.Load<SoundEffect>(@"Sounds\Shot1");
                enemyShot = content.Load<SoundEffect>(@"Sounds\Shot2");
                playerDeath = content.Load<SoundEffect>(@"Sounds\playerExplosion");
                Smokemon = content.Load<Song>(@"Sounds\Smokemon");
                goku = content.Load<Song>(@"Sounds\Goku");

                for (int x = 1; x <= explosionCount; x++)
                {
                    explosions.Add(
                        content.Load<SoundEffect>(@"Sounds\Explosion" +
                            x.ToString()));
                }
            }
            catch
            {
                Debug.Write("SoundManager Initialization Failed");
            }
        }

        public static void PlayExplosion()
        {
            try
            {
                explosions[rand.Next(0, explosionCount)].Play();
            }
            catch
            {
                Debug.Write("PlayExplosion Failed");
            }
        }

        public static void PlayPlayerShot()
        {
            try
            {
                playerShot.Play();
            }
            catch
            {
                Debug.Write("PlayPlayerShot Failed");
            }
        }

        public static void PlayEnemyShot()
        {
            try
            {
                enemyShot.Play();
            }
            catch
            {
                Debug.Write("PlayEnemyShot Failed");
            }
        }

        public static void PlayPlayerDeath()  //It's all Obi Wan's Fault!
        {
            try
            {
                playerDeath.Play();
            }
            catch
            {
                Debug.Write("PlayPlayerDeath Failed");
            }
        }
        public static void PlaySmokemon()
        {
            try
            {
                MediaPlayer.Play(Smokemon);
            }
            catch
            {
                Debug.Write("Smokemon Failed");
            }
        }

        public static void PlayGoku()
        {
            try
            {
                MediaPlayer.Play(goku);
            }
            catch
            {
                Debug.Write("Goku Failed");
            }
        }

        public static void StopSong()
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Stop();
        }

    }
}
