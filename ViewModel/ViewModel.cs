using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Runtime.CompilerServices;

using WpfNodes.Entity;

namespace WpfNodes
{
    //Commands view.
    /// <summary>
    /// Контракт команд с представлением.
    /// </summary>
    class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }
        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
    //Notify property.
    /// <summary>
    /// Уведомления клиентов об изменениях.
    /// </summary>
    class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    ////class DsNodes : NotifyPropertyChanged
    ////{
    ////    private int _id;
    ////    public int Id { get { return _id; } set { _id = value; OnPropertyChanged(nameof(Id)); } }
    ////    private string _name;
    ////    public string Name { get { return _name; } set { _name = value; OnPropertyChanged(nameof(Name)); } }
    ////    private Nullable<int> _parentId;
    ////    public Nullable<int> ParentId { get { return _parentId; } set { _parentId = value; OnPropertyChanged(nameof(ParentId)); } }
    ////    private List<DsNodes> _tableNodes1;
    ////    public List<DsNodes> TableNodes1
    ////    {
    ////        get { return _tableNodes1; }
    ////        set
    ////        {
    ////            _tableNodes1 = value;
    ////            OnPropertyChanged(nameof(TableNodes1));
    ////        }
    ////    }
    ////    public DsNodes(int count = 0)
    ////    {
    ////        TableNodes1 = new List<DsNodes>();
    ////        for (int i = 0; i < count; i++)
    ////        {
    ////            TableNodes1.Add(new DsNodes() { Name = $"Num: {i}" });
    ////        }
    ////    }
    ////}

    /// <summary>
    /// Модель представления. Содержит логику программы.
    /// </summary>
    class ViewModel : NotifyPropertyChanged
    {
        private readonly NodesEntities context = new NodesEntities();
        private DbSet<TablesNodes> table;
        public ViewModel()
        {
            table = context.TablesNodes;
            FillNodes();
        }

        //Select root nodes.
        public void FillNodes()
        {
            var q = (from n in table where n.ParentId == null select n).ToList();
            Nodes = q;
        }

        //Method nodes.
        private List<TablesNodes> _nodes;
        public List<TablesNodes> Nodes
        {
            get { return _nodes; }
            set { _nodes = value; OnPropertyChanged(nameof(Nodes)); }
        }

        /// <summary>
        /// Метод обращения/вызова временных форм.
        /// </summary>
        /// <typeparam name="T">Тип вызываемой формы</typeparam>
        /// <param name="dataContext">Объект который поместим в DataContext.</param>
        public void ShowWindow<T>(object dataContext) where T : Window, new()
        {
            var window = new T();
            window.DataContext = dataContext;
            window.Show();
        }

        //User commands.
        public ICommand ClickProperty
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    if (obj != null && obj is TablesNodes)
                    {
                        ShowWindow<NodeProperty>(obj);
                    }
                });
            }
        }
        public ICommand ClickAppend
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var item = new TablesNodes() { NameNode = "<name>" };
                    if (obj != null && obj is TablesNodes)
                    {
                        (obj as TablesNodes).TablesNodes1.Add(item);
                    }
                    else
                    {
                        table.Add(item);
                    }
                });
            }
        }
        public ICommand ClickRemove
        {
            get
            {
                return new DelegateCommand((obj) =>
                    {
                        if (obj != null && obj is TablesNodes)
                        {
                            table.Remove(obj as TablesNodes);
                        }
                    });
            }
        }
        public ICommand ClickSave
        {
            get
            {
                return new DelegateCommand((obj) =>
                    {
                        //commit changes.
                        context.SaveChanges();
                        FillNodes();
                    },
                    (obj) =>
                        {
                            //if has uncommitted changes.
                            return context.ChangeTracker.HasChanges();
                        });
            }
        }
        public ICommand ClickCancel
        {
            get
            {
                return new DelegateCommand((obj) =>
                    {
                        foreach (DbEntityEntry entry in context.ChangeTracker.Entries())
                        {
                            switch (entry.State)
                            {
                                // Under the covers, changing the state of an entity from  
                                // Modified to Unchanged first sets the values of all  
                                // properties to the original values that were read from  
                                // the database when it was queried, and then marks the  
                                // entity as Unchanged. This will also reject changes to  
                                // FK relationships since the original value of the FK  
                                // will be restored. 
                                case EntityState.Modified:
                                    entry.State = EntityState.Unchanged;
                                    break;
                                case EntityState.Added:
                                    entry.State = EntityState.Detached;
                                    break;
                                // If the EntityState is the Deleted, reload the date from the database.   
                                case EntityState.Deleted:
                                    entry.Reload();
                                    break;
                                default: break;
                            }
                        }
                    }, 
                    (obj) => 
                    {
                        return context.ChangeTracker.HasChanges();
                    });
            }
        }
    }
}
