document.addEventListener("DOMContentLoaded", function () {

    /* ============================================
       SCROLL REVEAL ANIMATION
       ============================================ */

    const revealElements =
        document.querySelectorAll(".reveal");

    const revealObserver = new IntersectionObserver(
        function (entries) {

            entries.forEach(function (entry) {

                if (entry.isIntersecting) {

                    entry.target.classList.add("visible");

                    revealObserver.unobserve(entry.target);
                }

            });

        },
        {
            threshold: 0.12
        }
    );


    revealElements.forEach(function (element) {
        revealObserver.observe(element);
    });


    /* ============================================
       SMOOTH SCROLL
       ============================================ */

    const scrollLinks =
        document.querySelectorAll('a[href^="#"]');


    scrollLinks.forEach(function (link) {

        link.addEventListener("click", function (event) {

            const targetId =
                this.getAttribute("href");

            if (targetId === "#") {
                return;
            }

            const target =
                document.querySelector(targetId);

            if (target) {

                event.preventDefault();

                target.scrollIntoView({
                    behavior: "smooth",
                    block: "start"
                });

            }

        });

    });

});