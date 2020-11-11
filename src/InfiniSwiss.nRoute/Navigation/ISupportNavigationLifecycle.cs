using nRoute.Components;
using System;
using System.Threading.Tasks;

namespace nRoute.Navigation
{
    /// <summary>
    /// Provides navigation lifetime participation functionality
    /// </summary>
    public interface ISupportNavigationLifecycle
    {
        /// <summary>
        /// The title of the navigation content
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Initializes the supporter
        /// </summary>
        /// <param name="requestParameters">Merged request parameters that included the navigation parameters and 
        /// the parsed parameters from the Url</param>
        void Initialize(ParametersCollection requestParameters);

        Task InitializeAsync(ParametersCollection requestParameters);

        /// <summary>
        /// Indicates closing/navigation-away, which can be cancelled by the supporter
        /// </summary>
        /// <param name="confirmCallback">Callback confirming to proceed with the closing, else false to indicate cancel the closing</param>
        void Closing(Action<bool> confirmCallback);
    }
}
