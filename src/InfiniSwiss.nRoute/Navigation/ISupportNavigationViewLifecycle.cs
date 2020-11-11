using System;

namespace nRoute.Navigation
{
    /// <summary>
    /// Provides support for transitioning visuals whilst navigating
    /// </summary>
    public interface ISupportNavigationViewLifecycle
    {
        /// <summary>
        /// Indicates navigation-to, which can be perform visual transitions
        /// </summary>
        /// <param name="request"></param>
        void Initialize(NavigationResponse response);

        /// <summary>
        /// Indicates closing/navigation-away, which can be (indefinitely) delayed to perform visual transitions
        /// </summary>
        /// <param name="request"></param>
        /// <param name="closeCallback"></param>
        void Closing(NavigationRequest request, Action closeCallback);
    }
}