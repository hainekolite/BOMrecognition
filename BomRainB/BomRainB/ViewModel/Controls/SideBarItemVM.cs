using BomRainB.ViewModel.Commands;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.ViewModel.Controls
{
    public sealed class SideBarItemVM
    {
        //no one can inherit from this -- SEALED class
        private readonly string _text;
        public string Text => _text;
        private readonly PackIconKind _icon;
        public PackIconKind Icon => _icon;
        private readonly RelayCommand _command;
        public RelayCommand Command => _command;

        public SideBarItemVM(string text, PackIconKind icon, RelayCommand command)
        {
            _text = text;
            _icon = icon;
            _command = command;
        }
    }
}
