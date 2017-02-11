// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the MockDispatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Platform.Core;

namespace Kastil.TestUtils
{


    /// <summary>
    /// Defines the MockDispatcher type.
    /// </summary>
    public class MockDispatcher
        : MvxMainThreadDispatcher, IMvxViewDispatcher
    {
        /// <summary>
        /// The requests
        /// </summary>
        public readonly List<MvxViewModelRequest> Requests = new List<MvxViewModelRequest>();

        /// <summary>
        /// Requests the main thread action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>returns true.</returns>
        public bool RequestMainThreadAction(Action action)
        {
            action();
            return true;
        }

        /// <summary>
        /// Shows the view model.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>return true.</returns>
        public bool ShowViewModel(MvxViewModelRequest request)
        {
            this.Requests.Add(request);
            return true;
        }


        public readonly List<MvxPresentationHint> Hints = new List<MvxPresentationHint>(); 
        /// <summary>
        /// Changes the presentation.
        /// </summary>
        /// <param name="hint">The hint.</param>
        /// <returns>an exception.</returns>
        public bool ChangePresentation(MvxPresentationHint hint)
        {
            Hints.Add(hint);
            return true;
        }
    }
}
