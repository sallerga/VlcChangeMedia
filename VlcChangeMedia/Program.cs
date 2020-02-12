using LibVLCSharp.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace VlcChangeMedia
{
    sealed class Program : IDisposable
    {
        private const int nrOfPlayers = 16;
        private static readonly MediaPlayer[] players = new MediaPlayer[nrOfPlayers];
        private static LibVLC libVLC;

        static async Task Main(string[] args)
        {
            Core.Initialize();

            libVLC = new LibVLC(new[] {
                "--verbose=2",
                "--no-audio",
            });


            libVLC.SetLogFile("vlclog.txt");

            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new MediaPlayer(libVLC);
            }

            long count = 0;
            while (true)
            {
                var playTasks = players.Select((p, i) => Task.Run(() =>
                {
                    Play(p, @"..\..\..\00020309.MPG", (float)i / nrOfPlayers);
                }));
                await Task.WhenAll(playTasks);
                Console.WriteLine(count++);
            }
        }

        public static void Play(MediaPlayer player, string file, float position)
        {
            using (var media = new Media(libVLC, file, FromType.FromPath))
            {
                player.Media = media;
            }
            if (player.Play())
            {
                player.Position = position;
            }
        }

        public void Dispose()
        {
            libVLC?.Dispose();
            foreach (var player in players)
            {
                player?.Dispose();
            }
        }
    }
}
