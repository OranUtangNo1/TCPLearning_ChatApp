using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppClient
{
    /// <summary>
    /// ViewModel基底クラス
    /// </summary>
    public class ViewModelBase: INotifyPropertyChanged
    {
        #region  Event Handler
        /// <summary>PropertyChanged</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>SetProperty</summary>
        public void SetProperty<T>(ref T field, T val, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, val))
            {
                field = val;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
