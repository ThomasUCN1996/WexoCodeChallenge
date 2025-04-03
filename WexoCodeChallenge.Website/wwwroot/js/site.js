$(document).ready(function () {
    let lastScrollTop = 0;
    let navbar = $(".navbar");
    let logoContainer = $(".logo-container");

    // Fade in the logo when the page loads
    logoContainer.addClass("show");

    $(window).on("scroll", function () {
        let scrollPos = $(this).scrollTop();

        // Handle the logo fading
        if (scrollPos === 0) {
            // If scrolled to the very top, make sure the logo is fully visible
            logoContainer.css("opacity", "1");
        } else if (scrollPos > lastScrollTop) {
            // Scrolling down -> Fade out logo
            logoContainer.css("opacity", Math.max(1 - scrollPos / 300, 0));
        } else {
            // Scrolling up -> Fade in logo
            logoContainer.css("opacity", Math.min(1, logoContainer.css("opacity") * 1.1));
        }

        // Handle navbar visibility based on scroll
        if (scrollPos > 100) {  // Show navbar after scrolling down a bit
            navbar.addClass("show");
        } else {
            navbar.removeClass("show");
        }

        lastScrollTop = scrollPos;
    });

    // Show navbar when mouse is near the top or hovering over the navbar
    let isHovering = false;
    let hideTimeout;

    $(document).mousemove(function (e) {
        if (e.clientY < 50 || isHovering) {  // Show if mouse is near the top or on navbar
            clearTimeout(hideTimeout); // Stop hiding
            navbar.addClass("show");
        } else {
            hideTimeout = setTimeout(() => {
                if (!isHovering) {
                    navbar.removeClass("show");
                }
            }, 500); // Delay hide by 500ms for better UX
        }
    });

    // Keep navbar visible when hovering over it
    navbar.hover(
        function () {
            isHovering = true;
            navbar.addClass("show");
        },
        function () {
            isHovering = false;
        }
    );
});
