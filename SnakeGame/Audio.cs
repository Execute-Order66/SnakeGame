using System;
using System.Threading;
using System.Windows.Media;

namespace SnakeGame
{
    public static class Audio
    {
        public readonly static MediaPlayer EatFood = LoadAudio("among.mp3");
        public readonly static MediaPlayer DeathSound = LoadAudio("Death.mp3");
        public readonly static MediaPlayer Theme = LoadAudio("Theme.mp3");

        private static MediaPlayer LoadAudio(string fileName, double volume=1,bool repeat = false, bool autoReset = true)
        {
            MediaPlayer player = new();
            player.Open(new Uri($"Assets/{fileName}", UriKind.Relative));
            player.Volume = volume;
            if (repeat)
            {
                player.MediaEnded += PlayerRepeat_MediaEnded;
            }

           if (autoReset)
            {
                player.MediaEnded += Player_MediaEnded;
            }


            return player;
        }

        private static void PlayerRepeat_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Stop();
            m.Position = new TimeSpan(0);
            m.Play();
        }

        private static void Player_MediaEnded(object sender, EventArgs e)
        {
            MediaPlayer m = sender as MediaPlayer;
            m.Stop();
            m.Position = new TimeSpan(0);
        }

    }
}
