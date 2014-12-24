
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using ExceptionHandlingInjector.Infrastructure;
using ExceptionHandlingInjector.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Mono.Cecil;

namespace ExceptionHandlingInjector.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase,IDisposable
    {
        private IDisposable _subscriber;
        
        
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            
            
        }

       
        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    RaisePropertyChanged("Path");
                }
            }
        }

        private AssemblyViewModel _assemblyInfo;

        public AssemblyViewModel AssemblyInfo
        {
            get { return _assemblyInfo;}
            set
            {
                if (_assemblyInfo != value)
                {
                    _assemblyInfo = value;
                    RaisePropertyChanged("AssemblyInfo");
                }
            }
        }

        private AssemblyItemViewModel _selectedItem;

        public AssemblyItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged("SelectedItem");
                }
            }
        }

        private RelayCommand _browseCommand;

        public ICommand BrowseCommand
        {
            get
            {
                return _browseCommand ??(_browseCommand= new RelayCommand(OpenExplorer));
            }
        }

        private RelayCommand _applyCommand;

        public ICommand ApplyCommand
        {
            get { return _applyCommand ?? (_applyCommand = new RelayCommand(Apply)); }
        }

        private void Apply()
        {
              AssemblyInfo.Save(Path);
        }


        private void OpenExplorer()
        {
            var openFileDialog = new OpenFileDialog() { Filter = "(*.dll;*.exe)|*.dll;*.exe" };
            openFileDialog.ShowDialog();
            Path = openFileDialog.FileName;

            if (!string.IsNullOrWhiteSpace(Path))
            {
                var task=Task.Factory.StartNew(() =>
                {
                    var assemblyLocation = new FileInfo(Path);

                    var assemblyModel = new AssemblyModel(AssemblyDefinition.ReadAssembly(assemblyLocation.FullName));
                    AssemblyInfo = new AssemblyViewModel(assemblyModel);
                    
                    _subscriber = PopulateItems(AssemblyInfo.Items).Select(v => v.OnPropertyChanges("IsSelected")).Merge().Subscribe(Observer.Create<AssemblyItemViewModel>(li =>
                    {
                        if (li.IsSelected)
                        {
                            SelectedItem = li;
                        }

                    }));

                });

                //task.Wait();
            }
        }

        private IEnumerable<AssemblyItemViewModel> PopulateItems(IList<AssemblyItemViewModel> items)
        {
            var chs=items.SelectMany(i => i.Items).ToList();
            var childs = chs.Count > 0 ? PopulateItems(chs).ToList() : new List<AssemblyItemViewModel>();
            return items.Union(childs);

        }


        public void Dispose()
        {
            if (_subscriber != null)
            {
                _subscriber.Dispose();
            }
        }
    }
}