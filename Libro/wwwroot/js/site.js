// Libro — small front-end touches

document.addEventListener('DOMContentLoaded', function () {
    // Navbar gains a deeper shadow once the page scrolls,
    // like a card being lifted off the catalog drawer.
    var navbar = document.querySelector('.navbar');
    if (navbar) {
        var onScroll = function () {
            if (window.scrollY > 8) {
                navbar.classList.add('navbar-raised');
            } else {
                navbar.classList.remove('navbar-raised');
            }
        };
        document.addEventListener('scroll', onScroll, { passive: true });
        onScroll();
    }

    // Auto-dismiss success/error alerts after a few seconds.
    document.querySelectorAll('.alert-dismissible').forEach(function (alertEl) {
        setTimeout(function () {
            var closeBtn = alertEl.querySelector('.btn-close');
            if (closeBtn) closeBtn.click();
        }, 6000);
    });
});
