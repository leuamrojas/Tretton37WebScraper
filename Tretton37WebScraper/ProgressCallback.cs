using System;
using System.Collections.Generic;
using System.Text;

namespace Tretton37WebScraper
{
    public delegate void UpdateProgressCallbackDelegate(int percent);

    public class ProgressCallback
    {

        public void UpdateProgress(UpdateProgressCallbackDelegate callback, int percent)
        {
            if (callback != null)
            {
                callback(percent);
            }
        }
    }
}
