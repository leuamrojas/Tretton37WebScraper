using System;
using System.Collections.Generic;
using System.Text;

namespace Tretton37WebScraper.Core
{
    public delegate void UpdateProgressCallbackDelegate(int percent);

    public class ProgressCallback
    {

        public void UpdateProgress(UpdateProgressCallbackDelegate callbackDelegate, int percent)
        {
            if (callbackDelegate != null)
            {
                callbackDelegate(percent);
            }
        }
    }
}
