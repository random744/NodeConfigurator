using System;
using System.Collections.ObjectModel;
using Opc.Ua;
using NodeConfigurator.Services;

namespace NodeConfigurator.ViewModels
{
    public class TreeNodeViewModel : ViewModelBase
    {
        private readonly IOpcUaClientService? _opcService;
        
        public string DisplayName { get; set; } = string.Empty;
        public string NodeId { get; set; } = string.Empty;
        public string BrowseName { get; set; } = string.Empty;
        public NodeClass NodeClass { get; set; }
        public string DataType { get; set; } = string.Empty;
        public string AccessLevel { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CurrentValue { get; set; } = string.Empty;
        
        public bool CanBeSelected => NodeClass == NodeClass.Variable;
        public bool ShowDataType => NodeClass == NodeClass.Variable;
        public bool CanWrite { get; set; }
        
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
        
        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (SetProperty(ref _isExpanded, value) && _isExpanded && Children.Count == 0 && HasChildren)
                {
                    LoadChildren();
                }
            }
        }
        
        private bool _isSelectedInTree;
        public bool IsSelectedInTree
        {
            get => _isSelectedInTree;
            set => SetProperty(ref _isSelectedInTree, value);
        }
        
        public bool HasChildren { get; set; }
        public ObservableCollection<TreeNodeViewModel> Children { get; set; } = new();
        
        private async void LoadChildren()
        {
            if (_opcService == null || string.IsNullOrEmpty(NodeId)) return;
            
            try
            {
                var references = await _opcService.BrowseAsync(Opc.Ua.NodeId.Parse(NodeId));
                foreach (var reference in references)
                {
                    var childNode = new TreeNodeViewModel(_opcService)
                    {
                        DisplayName = reference.DisplayName.Text ?? reference.BrowseName.Name,
                        NodeId = reference.NodeId.ToString(),
                        BrowseName = reference.BrowseName.Name,
                        NodeClass = reference.NodeClass,
                        HasChildren = reference.NodeClass != NodeClass.Variable
                    };
                    Children.Add(childNode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading children: {ex.Message}");
            }
        }
        
        public TreeNodeViewModel(IOpcUaClientService? opcService = null)
        {
            _opcService = opcService;
        }
    }
}
