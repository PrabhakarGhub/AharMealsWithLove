// AHAR - Meals with Love | Custom JS

// Auto-hide alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function() {
    setTimeout(function() {
        document.querySelectorAll('.alert:not(.alert-warning):not(.alert-danger)').forEach(function(el) {
            el.style.transition = 'opacity 0.5s';
            el.style.opacity = '0';
            setTimeout(() => el.remove(), 500);
        });
    }, 5000);

    // Confirm before delete/reject actions
    document.querySelectorAll('form[data-confirm]').forEach(function(form) {
        form.addEventListener('submit', function(e) {
            if (!confirm(form.dataset.confirm)) e.preventDefault();
        });
    });

    // Active nav link highlight
    var currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-link, .dash-sidebar .nav-link').forEach(function(link) {
        if (link.getAttribute('href') && 
            currentPath.includes(link.getAttribute('href').toLowerCase().split('/').pop())) {
            link.classList.add('active');
        }
    });

    // Dashboard sidebar mobile toggle
    var sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function() {
            document.querySelector('.dash-sidebar').classList.toggle('show');
        });
    }
});

// Print function for receipts
function printSection(sectionId) {
    var content = document.getElementById(sectionId).innerHTML;
    var win = window.open('', '', 'height=800,width=1000');
    win.document.write('<html><head><title>AHAR Receipt</title>');
    win.document.write('<link rel="stylesheet" href="/css/bootstrap.min.css">');
    win.document.write('<link rel="stylesheet" href="/css/site.css">');
    win.document.write('</head><body>' + content + '</body></html>');
    win.document.close();
    setTimeout(function() { win.print(); }, 500);
}
