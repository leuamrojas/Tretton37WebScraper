namespace Tretton37WebScraper.Core
{
    public delegate void UpdateProgressCallbackDelegate(int percent);

    public class ProgressCallback
    {

        public void UpdateProgress(UpdateProgressCallbackDelegate callbackDelegate, int percent)
        {
            callbackDelegate?.Invoke(percent);
        }
    }
}
