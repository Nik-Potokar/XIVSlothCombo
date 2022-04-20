using Dalamud.Utility;
using ImGuiScene;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XIVSlothComboPlugin;
using Log = Dalamud.Logging.PluginLog;

namespace XIVSlothComboPlugin
{
    public static class ImageHandler
    {

        private static BlockingCollection<Func<Task>> downloadQueue = new();
        private static BlockingCollection<Action> loadQueue = new();
        private static CancellationTokenSource downloadToken = new();
        private static Thread downloadThread;
        private static bool isBusy = false;

        static ImageHandler()
        {
            downloadThread = new Thread(DownloadTask);
            downloadThread.Start();
        }

        private async static void DownloadTask()
        {
            while (!downloadToken.Token.IsCancellationRequested)
            {
                try
                {
                    if (!downloadQueue.TryTake(out var task, -1, downloadToken.Token))
                        return;

                    await task.Invoke();
                }
                catch (OperationCanceledException)
                {
                    // Shutdown signal.
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An unhandled exception occurred in the plugin image downloader");
                }
            }

        }

        public async static Task<bool> TryGetImage(string url)
        {
            byte[]? iconTexture = Service.Configuration.GetImageInCache(url);

            if (iconTexture != null)
                return true;

            if (!downloadQueue.IsCompleted && !isBusy)
            {
                isBusy = true;
                downloadQueue.Add(async () => await DownloadImageAsync(url));
                isBusy = false;
            }

            return false;
        }

        private async static Task DownloadImageAsync(string url)
        {
            static bool TryLoadIcon(byte[] bytes, string? loc, out TextureWrap? icon)
            {
                // FIXME(goat): This is a hack around this call failing randomly in certain situations. Might be related to not being called on the main thread.
                try
                {
                    icon = Service.Interface.UiBuilder.LoadImage(bytes);
                }
                catch (AccessViolationException ex)
                {
                    Log.Error(ex, "Access violation during load plugin icon from {Loc}", loc);

                    icon = null;
                    return false;
                }

                if (icon == null)
                {
                    return false;
                }

                return true;
            }

            if (!url.IsNullOrEmpty())
            {
                HttpResponseMessage data;
                try
                {
                    Log.Log("Getting Data");
                    data = await Util.HttpClient.GetAsync(url);
                }
                catch (InvalidOperationException)
                {
                    Log.Debug("Can't Download");
                    return;
                }
                catch (Exception ex)
                {
                    Log.Debug("Can't Download 2");
                    return;
                }

                if (data.StatusCode == HttpStatusCode.NotFound)
                {
                    Log.Debug("URL NOT FOUND");
                    return;
                }


                data.EnsureSuccessStatusCode();

                var bytes = await data.Content.ReadAsByteArrayAsync();

                Service.Configuration.SetImageInCache(url, bytes);
                Service.Configuration.Save();
                return;
            }
        }
    }
}
