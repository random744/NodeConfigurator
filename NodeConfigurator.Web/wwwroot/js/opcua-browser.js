// OPC-UA Browser JavaScript

$(document).ready(function() {
    let selectedNodeIds = new Set();
    
    // Load initially selected nodes
    $('.selected-node-item').each(function() {
        selectedNodeIds.add($(this).data('node-id'));
    });
    
    // Initialize jsTree
    $('#treeview').jstree({
        'core': {
            'data': function(node, callback) {
                var nodeId = node.id === '#' ? '' : node.id;
                
                $.get('/OpcUa/GetNodes', { nodeId: nodeId }, function(response) {
                    if (response.success) {
                        var nodes = response.nodes.map(function(n) {
                            return {
                                id: n.nodeId,
                                text: n.displayName,
                                children: n.hasChildren,
                                icon: getNodeIcon(n.nodeClass),
                                state: { 
                                    checkbox_disabled: !n.canBeSelected,
                                    selected: selectedNodeIds.has(n.nodeId)
                                },
                                data: n
                            };
                        });
                        callback(nodes);
                    } else {
                        showToast('Fehler beim Laden der Knoten: ' + response.message, 'error');
                        callback([]);
                    }
                }).fail(function() {
                    showToast('Verbindungsfehler beim Laden der Knoten', 'error');
                    callback([]);
                });
            },
            'check_callback': true
        },
        'checkbox': {
            'three_state': false,
            'cascade': '',
            'tie_selection': false
        },
        'plugins': ['checkbox', 'search']
    });
    
    // Handle node selection
    $('#treeview').on('check_node.jstree', function(e, data) {
        var node = data.node;
        if (node.data && node.data.canBeSelected) {
            selectNode(node.data);
        }
    });
    
    // Handle node deselection
    $('#treeview').on('uncheck_node.jstree', function(e, data) {
        var node = data.node;
        if (node.data && node.data.canBeSelected) {
            unselectNode(node.data.nodeId);
        }
    });
    
    // Handle node click for details
    $('#treeview').on('select_node.jstree', function(e, data) {
        showNodeDetails(data.node.data);
    });
    
    // Search functionality
    var searchTimeout = false;
    $('#search').keyup(function() {
        if (searchTimeout) {
            clearTimeout(searchTimeout);
        }
        var searchValue = $(this).val();
        searchTimeout = setTimeout(function() {
            $('#treeview').jstree(true).search(searchValue);
        }, 250);
    });
    
    // Remove node button
    $(document).on('click', '.remove-node', function() {
        var nodeId = $(this).data('node-id');
        unselectNode(nodeId);
        
        // Uncheck in tree
        var tree = $('#treeview').jstree(true);
        tree.uncheck_node(nodeId);
    });
    
    // Select node function
    function selectNode(nodeData) {
        if (selectedNodeIds.has(nodeData.nodeId)) {
            return; // Already selected
        }
        
        var selectedNode = {
            NodeId: nodeData.nodeId,
            DisplayName: nodeData.displayName,
            BrowseName: nodeData.browseName,
            DataType: nodeData.dataType || ''
        };
        
        $.ajax({
            url: '/OpcUa/SelectNode',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(selectedNode),
            success: function(response) {
                if (response.success) {
                    selectedNodeIds.add(nodeData.nodeId);
                    addSelectedNodeToUI(selectedNode);
                    updateSelectedCount();
                }
            },
            error: function() {
                showToast('Fehler beim Auswählen des Knotens', 'error');
            }
        });
    }
    
    // Unselect node function
    function unselectNode(nodeId) {
        $.ajax({
            url: '/OpcUa/UnselectNode',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(nodeId),
            success: function(response) {
                if (response.success) {
                    selectedNodeIds.delete(nodeId);
                    removeSelectedNodeFromUI(nodeId);
                    updateSelectedCount();
                }
            },
            error: function() {
                showToast('Fehler beim Entfernen des Knotens', 'error');
            }
        });
    }
    
    // Add selected node to UI
    function addSelectedNodeToUI(node) {
        var emptyMessage = $('#selectedNodes .text-center.text-muted');
        if (emptyMessage.length) {
            emptyMessage.remove();
        }
        
        var dataTypeBadge = node.DataType ? 
            `<br /><span class="badge bg-info">${node.DataType}</span>` : '';
        
        var nodeHtml = `
            <div class="selected-node-item mb-2 p-2 border rounded" data-node-id="${node.NodeId}">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <strong>${node.DisplayName}</strong>
                        <br />
                        <small class="text-muted">${node.NodeId}</small>
                        ${dataTypeBadge}
                    </div>
                    <button class="btn btn-sm btn-danger remove-node" data-node-id="${node.NodeId}">
                        <i class="bi bi-x"></i>
                    </button>
                </div>
            </div>
        `;
        
        $('#selectedNodes').append(nodeHtml);
    }
    
    // Remove selected node from UI
    function removeSelectedNodeFromUI(nodeId) {
        $(`.selected-node-item[data-node-id="${nodeId}"]`).fadeOut('fast', function() {
            $(this).remove();
            
            if ($('#selectedNodes .selected-node-item').length === 0) {
                $('#selectedNodes').html(`
                    <div class="text-center text-muted">
                        <i class="bi bi-inbox" style="font-size: 3rem;"></i>
                        <p>Keine Variablen ausgewählt</p>
                    </div>
                `);
            }
        });
    }
    
    // Update selected count
    function updateSelectedCount() {
        $('#selectedCount').text(selectedNodeIds.size);
    }
    
    // Show node details
    function showNodeDetails(nodeData) {
        if (!nodeData) {
            $('#nodeDetails').html(`
                <div class="text-center text-muted">
                    <i class="bi bi-cursor" style="font-size: 3rem;"></i>
                    <p>Wählen Sie einen Knoten aus</p>
                </div>
            `);
            return;
        }
        
        var readValueButton = '';
        if (nodeData.canBeSelected) {
            readValueButton = `
                <button class="btn btn-sm btn-primary mt-3" onclick="readNodeValue('${nodeData.nodeId}')">
                    <i class="bi bi-eye-fill"></i> Wert lesen
                </button>
            `;
        }
        
        var detailsHtml = `
            <table class="table table-sm">
                <tbody>
                    <tr>
                        <td class="detail-label">Display Name:</td>
                        <td class="detail-value">${nodeData.displayName}</td>
                    </tr>
                    <tr>
                        <td class="detail-label">Browse Name:</td>
                        <td class="detail-value">${nodeData.browseName}</td>
                    </tr>
                    <tr>
                        <td class="detail-label">Node ID:</td>
                        <td class="detail-value"><code>${nodeData.nodeId}</code></td>
                    </tr>
                    <tr>
                        <td class="detail-label">Node Class:</td>
                        <td class="detail-value"><span class="badge bg-secondary">${nodeData.nodeClass}</span></td>
                    </tr>
                </tbody>
            </table>
            ${readValueButton}
            <div id="nodeValue" class="mt-3"></div>
        `;
        
        $('#nodeDetails').html(detailsHtml);
    }
    
    // Get node icon based on node class
    function getNodeIcon(nodeClass) {
        switch (nodeClass) {
            case 'Object':
                return 'jstree-folder';
            case 'Variable':
                return 'jstree-file';
            case 'Method':
                return 'jstree-file';
            default:
                return 'jstree-folder';
        }
    }
});

// Read node value (global function)
function readNodeValue(nodeId) {
    $('#nodeValue').html('<div class="spinner-border spinner-border-sm" role="status"></div> Lese Wert...');
    
    $.get('/OpcUa/ReadValue', { nodeId: nodeId }, function(response) {
        if (response.success) {
            var valueHtml = `
                <div class="card">
                    <div class="card-body">
                        <h6>Aktueller Wert:</h6>
                        <p class="mb-1"><strong>Wert:</strong> <code>${response.value}</code></p>
                        <p class="mb-1"><strong>Status:</strong> <span class="badge bg-success">${response.statusCode}</span></p>
                        <p class="mb-0"><strong>Timestamp:</strong> ${response.timestamp}</p>
                    </div>
                </div>
            `;
            $('#nodeValue').html(valueHtml);
        } else {
            $('#nodeValue').html(`
                <div class="alert alert-danger">
                    <i class="bi bi-exclamation-triangle-fill"></i> Fehler: ${response.message}
                </div>
            `);
        }
    }).fail(function() {
        $('#nodeValue').html(`
            <div class="alert alert-danger">
                <i class="bi bi-exclamation-triangle-fill"></i> Verbindungsfehler
            </div>
        `);
    });
}
