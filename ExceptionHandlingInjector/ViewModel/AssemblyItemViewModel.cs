using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ExceptionHandlingInjector.Infrastructure;
using ExceptionHandlingInjector.Model;
using GalaSoft.MvvmLight;

namespace ExceptionHandlingInjector.ViewModel
{
    public  class AssemblyItemViewModel:ViewModelBase
    {
        private readonly IAssemblyItem _assemblyItem;
        
        public AssemblyItemViewModel(IAssemblyItem assemblyItem)
        {
            _assemblyItem = assemblyItem;
            
            Name = assemblyItem.Name;
            IsInjected = assemblyItem.IsInjected;
            Body = assemblyItem.Body;
            Items=assemblyItem.GetItems().Select(item=>new AssemblyItemViewModel(item)).ToList();
        }

        

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }


        private bool _isInjected;

        public bool IsInjected
        {
            get
            {
                if (Items.Any(item => !item.IsInjected))
                    _isInjected = false;

                return _isInjected;
            }
            set
            {
                if (_isInjected != value)
                {
                    _isInjected = value;
                    RaisePropertyChanged("IsInjected");

                    Items.ToList().ForEach(item=>item.IsInjected=value);
                    InjectUninject(value);
                }
            }
        }

        private string _body;

        public string Body
        {
            get { return _body; }
            set
            {
                if (_body != value)
                {
                    _body = value;
                    RaisePropertyChanged("Body");
                }
            }
        }

        private void InjectUninject(bool isInject)
        {
            if (IsSelected)
            {
                if (isInject)
                {
                    _assemblyItem.Inject();
                }
                else
                {
                    _assemblyItem.UnInject();
                }

            }
            
        }


        private IList<AssemblyItemViewModel> _items = new List<AssemblyItemViewModel>();

        public IList<AssemblyItemViewModel> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    RaisePropertyChanged("Items");
                }
            }
        }


        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        

    }
}
