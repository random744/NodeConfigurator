// Site-wide JavaScript functionality

// Auto-dismiss alerts after 5 seconds
$(document).ready(function() {
    setTimeout(function() {
        $('.alert').fadeOut('slow', function() {
            $(this).remove();
        });
    }, 5000);
});

// Confirm before leaving page with unsaved changes
window.onbeforeunload = function() {
    // Can be enhanced to check for unsaved changes
    return undefined;
};

// Helper function to show toast notifications
function showToast(message, type = 'info') {
    const alertTypes = {
        'success': 'alert-success',
        'error': 'alert-danger',
        'warning': 'alert-warning',
        'info': 'alert-info'
    };
    
    const alertClass = alertTypes[type] || 'alert-info';
    const icon = type === 'success' ? 'check-circle-fill' : 
                 type === 'error' ? 'exclamation-triangle-fill' : 
                 'info-circle-fill';
    
    const alert = $(`
        <div class="alert ${alertClass} alert-dismissible fade show m-3" role="alert">
            <i class="bi bi-${icon}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `);
    
    $('main').prepend(alert);
    
    setTimeout(function() {
        alert.fadeOut('slow', function() {
            $(this).remove();
        });
    }, 5000);
}
