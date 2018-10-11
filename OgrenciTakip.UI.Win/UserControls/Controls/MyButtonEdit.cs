﻿using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using OgrenciTakip.UI.Win.Interfaces;
using System.Drawing;
using System;
using System.ComponentModel;

namespace OgrenciTakip.UI.Win.UserControls.Controls
{
    [ToolboxItem(true)]
    public class MyButtonEdit : ButtonEdit, IStatusBarKisayol
    {
        public MyButtonEdit()
        {
            Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            Properties.AppearanceFocused.BackColor = Color.LightCyan;
        }
        public override bool EnterMoveNextControl { get; set; } = true;
        public string StatusBarAciklama { get; set; }
        public string StatusBarKisayol { get; set; } = "F4 :";
        public string StatusBarKisayolAciklama { get; set; }

        #region Event

        private long? _id;
        //Derlendikten sonra property listesinde göstermez.
        [Browsable(false)]
        public long? Id
        {
            get { return _id; }
            set
            {
                var oldValue = _id;
                var newValue = value;
                if (oldValue == newValue)
                {
                    return;
                }
                _id = value;

                IdChanged?.Invoke(this, new IdChangedEventArgs(oldValue, newValue));
            }
        }

        //delegate yazıldığı zaman hiç bir zaman null a düşmez.

        public event EventHandler<IdChangedEventArgs> IdChanged; 
        #endregion

    }
    public class IdChangedEventArgs : EventArgs
    {
        public IdChangedEventArgs(long? oldValue, long? newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        public long? OldValue { get; }
        public long? NewValue { get; }
    }
}
