﻿/*
Copyright (c) 2014, Los Alamos National Security, LLC
All rights reserved.
Copyright 2014. Los Alamos National Security, LLC. This software was produced under U.S. Government contract 
DE-AC52-06NA25396 for Los Alamos National Laboratory (LANL), which is operated by Los Alamos National Security, 
LLC for the U.S. Department of Energy. The U.S. Government has rights to use, reproduce, and distribute this software.  
NEITHER THE GOVERNMENT NOR LOS ALAMOS NATIONAL SECURITY, LLC MAKES ANY WARRANTY, EXPRESS OR IMPLIED, 
OR ASSUMES ANY LIABILITY FOR THE USE OF THIS SOFTWARE.  If software is modified to produce derivative works, 
such modified software should be clearly marked, so as not to confuse it with the version available from LANL.

Additionally, redistribution and use in source and binary forms, with or without modification, are permitted provided 
that the following conditions are met:
•	Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
disclaimer. 
•	Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following 
disclaimer in the documentation and/or other materials provided with the distribution. 
•	Neither the name of Los Alamos National Security, LLC, Los Alamos National Laboratory, LANL, the U.S. Government, 
nor the names of its contributors may be used to endorse or promote products derived from this software without specific 
prior written permission. 
THIS SOFTWARE IS PROVIDED BY LOS ALAMOS NATIONAL SECURITY, LLC AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL LOS ALAMOS NATIONAL SECURITY, LLC OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING 
IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.ComponentModel;
using System.Threading;

namespace NCC
{
    /// <summary>
    /// Allows long-running operations to report progress updates back to the caller.
    /// </summary>
    public class ProgressTracker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressTracker"/> class.
        /// </summary>
        public ProgressTracker(bool modal = false)
        {
            m_modal = modal;
            m_synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="progress">The progress, as a percentage from 0 to 100.</param>
        /// <param name="text">A text indication of the current state, e.g a file name, or a current rate.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="progress"/> is not between 0 and 100.</exception>
        public void ReportProgress(int progress, string text)
        {
            if (progress < 0 || progress > 100) {
                throw new ArgumentOutOfRangeException();
            }

            m_synchronizationContext.Post(
                _ => OnProgressChanged(new ProgressChangedEventArgs(progress, text)),
                null);
        }

        /// <summary>
        /// Occurs when a progress update is reported.
        /// </summary>
        /// <remarks>
        /// Handlers registered with this event will be invoked on the
        /// <see cref="SynchronizationContext"/> captured when the instance was constructed.
        /// </remarks>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Raises the <see cref="ProgressChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ProgressChangedEventArgs"/> that contains data for the event.</param>
        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = ProgressChanged;

            if (handler != null) {
                handler(this, e);
            }
        }

        SynchronizationContext m_synchronizationContext;

        public bool m_modal;
    }
}
