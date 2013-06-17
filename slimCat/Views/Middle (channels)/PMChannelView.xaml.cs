﻿/*
Copyright (c) 2013, Justin Kadrovach
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModels;

namespace Views
{
    /// <summary>
    /// Interaction logic for PMChannelView.xaml
    /// </summary>
    public partial class PMChannelView : DisposableView
    {
        #region Fields
        private PMChannelViewModel _vm;
        private SnapToBottomManager _manager;
        #endregion

        #region Constructors
        public PMChannelView(PMChannelViewModel vm)
        {
            try
            {
                InitializeComponent();
                if (vm == null) throw new ArgumentNullException("vm");

                _vm = vm;
                this.DataContext = _vm;

                _manager = new SnapToBottomManager(messages);

                _vm.NewMessageArrived += OnNewMessageArrived;
                _vm.StatusChanged += OnStatusChanged;
            }

            catch (Exception ex)
            {
                ex.Source = "PMChannel View, init";
                Exceptions.HandleException(ex);
            }
        }
        #endregion

        #region Methods
        private void OnMessagesLoaded(object sender, EventArgs e)
        {
            _manager.AutoDownScroll(false, true);
        }

        private void OnNewMessageArrived(object sender, EventArgs e)
        {
            bool keepAtCurrent = _vm.Model.Messages.Count >= Models.ApplicationSettings.BackLogMax;
            _manager.AutoDownScroll(keepAtCurrent);
        }

        private void OnStatusChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(
                (Action)delegate
                {
                    if (!CharacterStatusDisplayer.IsExpanded)
                        CharacterStatusDisplayer.IsExpanded = true;
                });
        }

        internal override void Dispose(bool IsManaged)
        {
            if (IsManaged)
            {
                _vm.StatusChanged -= OnStatusChanged;
                _vm.NewMessageArrived -= OnNewMessageArrived;
                _manager = null;
                this.DataContext = null;
                _vm = null;
            }
        }
        #endregion
    }
}
